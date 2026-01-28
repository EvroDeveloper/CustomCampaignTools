using BoneLib;
using CustomCampaignTools.Bonemenu;
using CustomCampaignTools.Games;
using CustomCampaignTools.Games.BoneLab;
using CustomCampaignTools.Patching;
using CustomCampaignTools.Timing;
using MelonLoader;
using System.Reflection;
using UnityEngine;

namespace CustomCampaignTools
{
    internal class Main : MelonMod
    {
        public override void OnInitializeMelon()
        {
            GameManager.InitializeGameConfiguration();
            ArgumentHandler.HandleArguments(Environment.GetCommandLineArgs());
        }

        public override void OnLateInitializeMelon()
        {
            // Create Bonemenu
            BoneMenuCreator.CreateBoneMenu();

            Campaign.OnInitialize();

            Hooking.OnLevelLoaded += LevelInitialized;
            Hooking.OnLevelUnloaded += LevelUnloaded;
            Hooking.OnUIRigCreated += OnUIRigCreated;

            string resourceName = "CampaignIcon.png";
            Assembly assembly = MelonAssembly.Assembly;

            BoneLabMainMenuMangler.LoadSpriteFromEmbeddedResource(resourceName, assembly, new Vector2(0.5f, 0.5f));

            foreach (MelonMod registeredMelon in MelonMod.RegisteredMelons)
            {
                if (HelperMethods.CheckIfAssemblyLoaded("BrowsingPlus"))
                {
                    PatchSwipezBecauseLemonloaderKeepsFuckingFailingIfIPutThisMethodInOnLateInitializeMelonForSomeReason();
                }
            }
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if(buildIndex == 0)
            {
                GameManager.OnBootstrapSceneLoaded();
            }
        }

        private void PatchSwipezBecauseLemonloaderKeepsFuckingFailingIfIPutThisMethodInOnLateInitializeMelonForSomeReason()
        {
            SwipezPanelPatches.ManualPatch();
        }

        internal static void LevelInitialized(LevelInfo info)
        {
            string barcode = info.barcode;

            GameManager.OnLevelLoaded(info);

            LevelLoadingPatches.OnNextSceneLoaded.Invoke();
            LevelLoadingPatches.OnNextSceneLoaded = () => {};
        }

        private static void LevelUnloaded()
        {
            if (Campaign.SessionActive)
            {
                Campaign.Session.saveData.SaveAmmoForLevel(Campaign.lastLoadedCampaignLevel);
                LevelTiming.OnCampaignLevelUnloaded(Campaign.Session, Campaign.lastLoadedCampaignLevel);
            }
        }

        private static void OnUIRigCreated()
        {
            GameManager.ManglePlayerMenu();
            Campaign.OnUIRigCreated();
        }
    }
}
