using CustomCampaignTools.Debug;
using HarmonyLib;
using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow.SceneStreaming;

namespace CustomCampaignTools.Games.BoneLab
{
    public static class BonelabBootstrapperPatch
    {
        public static void OnBootstrapSceneLoaded()
        {
            CampaignLogger.Msg("Bonelab Bootstrapper Scene Loaded - Checking for Forced Campaign Load");
            if(ArgumentHandler.forcedCampaign)
            {
                var bootstrapper = UnityEngine.Object.FindObjectOfType<SceneBootstrapper_Bonelab>();
                if(bootstrapper != null)
                {
                    Campaign c = CampaignUtilities.GetFromPallet(ArgumentHandler.campaignToLoad);
                    bootstrapper.MenuHollowCrateRef = new Il2CppSLZ.Marrow.Warehouse.LevelCrateReference(c.InitialLevel);
                }
            }
        }
    }
}