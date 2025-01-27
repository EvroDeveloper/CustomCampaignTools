namespace CustomCampaignTools
{
    public class MainMenuMangler
    {
        public static void OnInitialize()
        {
            BoneLib.Hooking.OnLevelLoaded += OnLevelLoaded;
        }

        public static void OnLevelLoaded(LevelInfo info)
        {
            if(info.barcode == BoneMenu.CommonBarcodes.Maps.VoidG114)
            {
                MangleMainMenu();
            }
        }

        public static void MangleMainMenu()
        {
            // Clone a menu button and edit it's text and icon
            // Clone the level select panel but strip it of it's functionality. 
            // Create a button for every campaign, with it's specified name and linked campaign
        }

        public static void LoadCampaign(Campaign campaign)
        {
            SceneStreamer.Load(new Barcode(campaign.MenuLevel), new Barcode(BoneLib.CommonBarcodes.Maps.LoadDefault));
        }
    }
}