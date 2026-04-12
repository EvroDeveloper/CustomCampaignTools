using System;
using CustomCampaignTools.Debug;
using CustomCampaignTools.Timing;
using CustomCampaignTools.Utilities;
using HarmonyLib;
using Il2CppCysharp.Threading.Tasks;
using Il2CppSLZ.Marrow.SceneStreaming;
using Il2CppSLZ.Marrow.Utilities;
using Il2CppSLZ.Marrow.Warehouse;

namespace CustomCampaignTools.Patching
{
    [HarmonyPatch(typeof(SceneStreamer))]
    public static class LevelLoadingPatches
    {
        public static Action OnNextSceneLoaded = ()=>{};

        [HarmonyPatch(nameof(SceneStreamer.LoadAsync), [typeof(LevelCrateReference), typeof(LevelCrateReference)])]
        [HarmonyPrefix]
        public static bool LoadPrefixPatch(LevelCrateReference level, ref LevelCrateReference loadLevel, ref UniTask __result)
        {
            bool loadingIntoCampaign = CampaignUtilities.TryGetFromLevel(level.Barcode, out var destinationCampaign);

            SavepointFunctions.CurrentLevelLoadedByContinue = SavepointFunctions.WasLastLoadByContinue;
            SavepointFunctions.WasLastLoadByContinue = false;

            if(Campaign.SessionActive && destinationCampaign == Campaign.Session)
            {
                if(Campaign.Session.GetLevel(level.Barcode).type == CampaignLevelType.MainLevel && Campaign.Session.GetLevel(SceneStreamer.Session.Level.Barcode).type == CampaignLevelType.MainLevel)
                {
                    if(Campaign.Session.GetMainLevelIndex(level.Barcode) == Campaign.Session.GetMainLevelIndex(SceneStreamer.Session.Level.Barcode) + 1)
                    {
                        Campaign.Session.saveData.SaveInventoryForLevel(level.Barcode.ID);
                    }
                }
            }

            if(loadingIntoCampaign)
            {
                destinationCampaign.saveData.UnlockLevel(level.Barcode.ID);

                // If the level its loading into is a campaign level, force load scene to be Campaign Load scene
                if(destinationCampaign.LoadScene.IsValid())
                    loadLevel = destinationCampaign.LoadScene;

                Campaign.Session = destinationCampaign;

                OnNextSceneLoaded += () =>
                {
                    LevelTiming.OnCampaignLevelLoaded(destinationCampaign, level.Barcode.ID);
                };

                if(destinationCampaign.GetLevel(level.Barcode).type == CampaignLevelType.MainLevel)
                {
                    if (!SavepointFunctions.CurrentLevelLoadedByContinue)
                    {
                        if(destinationCampaign.CreateSaveOnLevelEnter) OnNextSceneLoaded += () => destinationCampaign.saveData.SavePlayer(level.Barcode, SimpleTransform.Identity);
                        if(destinationCampaign.SaveLevelInventory) OnNextSceneLoaded += () =>
                        {
                            if(destinationCampaign.saveData.InventorySaves.ContainsKey(level.Barcode.ID))
                            {
                                destinationCampaign.saveData.InventorySaves[level.Barcode.ID].ApplyToRigManagerDelayed();
                            }
                        };
                    }
                }
            }

            return true;
        }
    }
}
