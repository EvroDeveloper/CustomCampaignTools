using BoneLib;

namespace CustomCampaignTools
{
    public class MainMenuMangler
    {
        public static void OnInitialize()
        {
            Hooking.OnLevelLoaded += OnLevelLoaded;
        }

        public static void OnLevelLoaded(LevelInfo info)
        {
            if(info.barcode == CommonBarcodes.Maps.VoidG114)
            {
                MangleMainMenu();
            }
        }

        public static void MangleMainMenu()
        {
            // Clone a menu button and edit it's text and icon
            // Clone the level select panel but strip it of it's functionality in place of a CampaignPanelView. 
            // CampaignPanelView.SetupButtons()
        }
    }
}