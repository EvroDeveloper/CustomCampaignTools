using System.Reflection;

namespace CustomCampaignTools.GameSupport
{
    public class GameConfiguration
    {
        public virtual IMenuMangler mainMenuMangler { get; }
        public virtual string mainMenuBarcode { get; }
        public virtual IMenuMangler playerMenuMangler { get; }
        public Assembly SupportAssembly;

        public virtual void OnInitialize() { }

        public virtual void OnLateInitialize() { }

        public virtual void RefreshCampaignMenu(Campaign campaign) {}

        public virtual void OnBootstrapSceneLoaded() { }
    }
}