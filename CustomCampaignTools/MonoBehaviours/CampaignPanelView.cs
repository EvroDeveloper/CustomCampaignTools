using System;

namespace CustomCampaignTools
{
    [RegisterTypeInIl2Cpp]
    public class CampaignPanelView : MonoBehaviour
    {
        public CampaignPanelView(IntPtr ptr) : base(ptr) {}

        private int _currentPage;
        private int _lastPage => Mathf.Floor(Campaign.LoadedCampaigns.Length / Buttons.Length);

        public GameObject[] Buttons;

        public GameObject backButton;
        public GameObject nextButton;

        public TextMeshPro pageText;

        public void SetupButtons()
        {
            for(int i = 0; i < Buttons.Length; i++)
            {
                Button button = Buttons[i].GetComponent<Button>();

                // TODO: Clear button's current stuff
                button.onClick.AddListener(new Action(() => { Select(i); }));
            }
        }

        public void NextPage()
        {
            _currentPage = Mathf.Min(_lastPage, _currentPage + 1);
            UpdateVisualization();
        }

        public void BackPage()
        {
            _currentPage = Mathf.Max(0, _currentPage - 1);
            UpdateVisualization();
        }

        public void Select(int index)
        {
            int campaignIndex = (Buttons.Length * _currentPage) + index;
            Campaign c = Campaign.LoadedCampaigns[campaignIndex];

            SceneStreamer.Load(new Barcode(c.MenuLevel), new Barcode(c.LoadScene));
        }

        private void UpdateVisualization()
        {
            if(_currentPage <= 0) backButton.SetActive(false);
            else backButton.SetActive(true);

            if(_currentPage >= _lastPage) nextButton.SetActive(false);
            else nextButton.SetActive(true);

            pageText = $"{_currentPage+1}/{_lastPage+1}";

            for(int i = 0; i < Buttons.Length; i++)
            {
                GameObject currentButton = Buttons[i];
                int campaignIndex = (Buttons.Length * _currentPage) + i;

                if(campaignIndex < Campaign.LoadedCampaigns.Length)
                {
                    currentButton.GetComponentInChildren<TextMeshPro>().text = Campaign.LoadedCampaigns[campaignIndex].Name;
                    currentButton.SetActive(true);
                }
                else
                {
                    currentButton.SetActive(false);
                }
            }
        }
    }
}