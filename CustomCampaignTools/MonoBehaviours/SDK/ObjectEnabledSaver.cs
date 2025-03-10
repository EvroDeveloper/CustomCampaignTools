namespace CustomCampaignTools.SDK
{
    [RegisterTypeInIl2Cpp]
    public class ObjectEnabledSaver : MonoBehaviour
    {
        public void Awake()
        {
            if(SavepointFunctions.CurrentLevelLoadedByContinue)
            {
                RestoreActiveState();
            }
        }

        public void RestoreActiveState()
        {
            gameObject.SetActive(Campain.Session.saveData.LoadedSavePoint.GetEnabledStateFromName(gameObject.name, gameObject.activeSelf));
        }
    }
}