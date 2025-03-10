namespace CustomCampaignTools.SDK
{
    [RegisterTypeInIl2Cpp]
    public class ObjectEnabledSaver : MonoBehaviour
    {
        public void Awake()
        {
            if(SavepointFunctions.LoadByContinue_ObjectEnabledHint)
            {
                // Horrendous ahh implementation but it should work :pray:
                SavepointFunctions.LoadByContinue_ObjectEnabledHint = false;
                foreach(ObjectEnabledSaver s in FindObjectsOfType<ObjectEnabledSaver>())
                {
                    s.RestoreActiveState();
                }
            }
        }

        public void RestoreActiveState()
        {
            gameObject.SetActive(Campain.Session.saveData.LoadedSavePoint.GetStateFromName(gameObject.name));
        }
    }
}