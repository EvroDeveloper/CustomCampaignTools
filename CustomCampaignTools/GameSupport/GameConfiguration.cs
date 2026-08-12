using BoneLib;
using Il2CppSLZ.Marrow.Warehouse;
using System;
using System.Reflection;

namespace CustomCampaignTools.GameSupport
{
    public class GameConfiguration
    {
        public Assembly SupportAssembly { get; internal set; }

        public IGameDataManager GameDataManager { get; internal set; }

        public virtual LevelCrateReference MainMenu { get; }

        public virtual void OnInitialize() { }

        public virtual void OnLateInitialize() { }

        public virtual void RefreshCampaignMenu(Campaign campaign) {}

        public virtual void OnBootstrapSceneLoaded() { }

        public virtual void OnLevelLoaded(LevelInfo info) { }

        public virtual void OnUIRigCreated() { }
    }
}