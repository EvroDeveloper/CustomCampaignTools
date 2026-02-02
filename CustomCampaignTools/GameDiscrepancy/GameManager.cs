using BoneLib;
using MelonLoader;

namespace CustomCampaignTools.Games
{
    public static class GameManager
    {
        public static GameConfiguration currentGameConfiguration;

        public static void InitializeGameConfiguration()
        {
            if(MelonUtils.CurrentGameAttribute.Name == "BONELAB")
                currentGameConfiguration = new BoneLabGameConfiguration();
            else if (MelonUtils.CurrentGameAttribute.Name == "BONEWORKS")
                currentGameConfiguration = new BoneLabGameConfiguration();
            else
                return;

            currentGameConfiguration.GameSpecificPatches();
        }

        public static void OnLateInitialize()
        {
            currentGameConfiguration.OnLateInitialize();
        }

        public static void ManglePlayerMenu()
        {
            currentGameConfiguration.playerMenuMangler.MangleMenu();
        }

        public static void OnLevelLoaded(LevelInfo info)
        {
            if (info.barcode == currentGameConfiguration.mainMenuBarcode)
            {
                currentGameConfiguration.mainMenuMangler.MangleMenu();
            }
        }

        internal static void OnBootstrapSceneLoaded()
        {
            currentGameConfiguration.OnBootstrapSceneLoaded();
        }
    }
}