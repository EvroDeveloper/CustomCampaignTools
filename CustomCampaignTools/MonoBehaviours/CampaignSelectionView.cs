using Il2CppSLZ.Marrow.Warehouse;
using Il2CppTMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CustomCampaignTools
{
    public class CampaignSelectionView : MonoBehaviour
    {
        public TMP_Text campaignTitle;
        public TMP_Text avatar;
        public Button menuButton;
        public Button continueButton;

        public Button backButton;

        public Campaign targetCampaign;

        public void SetupButtons()
        {
            menuButton.onClick.m_PersistentCalls.Clear();
            menuButton.onClick.m_Calls.ClearPersistent();
            menuButton.onClick.m_Calls.Clear();
            menuButton.onClick.AddListener(new Action(() => { Enter(); }));

            continueButton.onClick.m_PersistentCalls.Clear();
            continueButton.onClick.m_Calls.ClearPersistent();
            continueButton.onClick.m_Calls.Clear();
            continueButton.onClick.AddListener(new Action(() => { Continue(); }));
        }

        public void Activate(Campaign campaign)
        {
            targetCampaign = campaign;

            campaignTitle.text = campaign.Name;
            
            continueButton.gameObject.SetActive(campaign.saveData.LoadedSavePoint.IsValid(out _));

        }

        public void Enter()
        {
            FadeLoader.Load(new Barcode(targetCampaign.MenuLevel), new Barcode(targetCampaign.LoadScene));
        }

        public void Continue()
        {
            targetCampaign.saveData.LoadedSavePoint.LoadContinue(targetCampaign);
        }

    }
}