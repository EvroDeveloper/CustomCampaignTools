namespace CustomCampaignTools.Games
{
    public class GameConfiguration
    {
        public virtual IMenuMangler mainMenuMangler { get; }
        public virtual string mainMenuBarcode { get; }
        public virtual IMenuMangler playerMenuMangler { get; }

        public virtual void GameSpecificPatches()
        {
            
        }
    }
}