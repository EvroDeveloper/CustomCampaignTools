using BoneLib;
using CustomCampaignTools.Debug;
using CustomCampaignTools.SDK;
using CustomCampaignTools.Timing;
using HarmonyLib;
using Il2CppCysharp.Threading.Tasks;
using Il2CppSLZ.Marrow.Audio;
using Il2CppSLZ.Marrow.Data;
using Il2CppSLZ.Marrow.Pool;
using Il2CppSLZ.Marrow.SceneStreaming;
using Il2CppSLZ.Marrow.Utilities;
using Il2CppSLZ.Marrow.Warehouse;
using UnityEngine;

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
                if (loadLevel.Barcode != destinationCampaign.LoadScene)
                {
                    loadLevel = new LevelCrateReference(destinationCampaign.LoadScene);
                }
                Campaign.Session = destinationCampaign;

                OnNextSceneLoaded += () =>
                {
                    LevelTiming.OnCampaignLevelLoaded(destinationCampaign, level.Barcode.ID);
                };

                if(destinationCampaign.GetLevel(level.Barcode).type == CampaignLevelType.MainLevel)
                {
                    if (!SavepointFunctions.CurrentLevelLoadedByContinue)
                    {
                        OnNextSceneLoaded += () =>
                        {
                            destinationCampaign.saveData.SavePlayer(level.Barcode, SimpleTransform.Identity);
                            if(destinationCampaign.SaveLevelInventory && destinationCampaign.saveData.InventorySaves.ContainsKey(level.Barcode.ID))
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

    [HarmonyPatch(typeof(Audio2dManager))]
    public static class LoadMusicPatches
    {
        [HarmonyPatch(nameof(Audio2dManager.CueMusicInternal))]
        [HarmonyPrefix]
        public static void CueMusicPatch(Audio2dManager __instance, ref AudioClip musicClip)
        {
            if(musicClip.name == "music_LoadingSplash" && Campaign.SessionActive && Campaign.Session.LoadSceneMusic != null)
            {
                musicClip = Campaign.Session.LoadSceneMusic;
            }
        }

        [HarmonyPatch(nameof(Audio2dManager.StopSpecificMusic))]
        [HarmonyPrefix]
        public static void StopMusicPatch(Audio2dManager __instance, ref AudioClip specificClip)
        {
            if (specificClip.name == "music_LoadingSplash" && Campaign.SessionActive && Campaign.Session.LoadSceneMusic != null)
            {
                specificClip = Campaign.Session.LoadSceneMusic;
            }
        }
    }

    //[HarmonyPatch(typeof(AssetSpawner._SpawnAsync_d__15))]
    [HarmonyPatch(typeof(StreamSession))]
    public static class RigReplacerPatches
    {
        //[HarmonyPatch(nameof(AssetSpawner._SpawnAsync_d__15.MoveNext))]
        //[HarmonyPrefix]
        // public static void OnSpawnableSpawned(AssetSpawner._SpawnAsync_d__15 __instance)
        // {
        //     if(!Campaign.SessionActive) return;

        //     if (__instance.spawnable.crateRef.Barcode == MarrowGame.marrowSettings.UIEventSystem.Barcode && Campaign.Session.GameplayRigOverride.IsValid())
        //     {
        //         __instance.spawnable = new Spawnable()
        //         {
        //             crateRef = new SpawnableCrateReference(Campaign.Session.GameplayRigOverride),
        //             policyData = __instance.spawnable.policyData
        //         };
        //         CampaignLogger.Msg("Swapped UI Event System to spawnable: " + __instance.spawnable.crateRef.Crate.Title);
        //     }
        //     else if(__instance.spawnable.crateRef.Barcode == MarrowGame.marrowSettings.DefaultPlayerRig.Barcode && Campaign.Session.RigManagerOverride.IsValid())
        //     {
        //         __instance.spawnable = new Spawnable()
        //         {
        //             crateRef = new SpawnableCrateReference(Campaign.Session.RigManagerOverride),
        //             policyData = __instance.spawnable.policyData
        //         };
        //         CampaignLogger.Msg("Swapped RigManager to spawnable: " + __instance.spawnable.crateRef.Crate.Title);
        //     }
        // }

        [HarmonyPatch(nameof(StreamSession.RegisterPlayerMarker))]
        [HarmonyPrefix]
        public static bool OnPlayerMarkerRegistered(StreamSession __instance, PlayerMarker playerMarker)
        {
            SpawnableCrateReference rigManSpawn = MarrowGame.marrowSettings.DefaultPlayerRig;
            SpawnableCrateReference gameplayRigSpawn = MarrowGame.marrowSettings.UIEventSystem;

            bool shouldCancelOriginalCall = false;

            if(!Campaign.SessionActive)
            {
                if(Campaign.Session.RigManagerOverride.TryGetCrate(out _))
                {
                    rigManSpawn = Campaign.Session.RigManagerOverride;
                    shouldCancelOriginalCall = true;
                }
                if(Campaign.Session.GameplayRigOverride.TryGetCrate(out _))
                {
                    gameplayRigSpawn = Campaign.Session.GameplayRigOverride;
                    shouldCancelOriginalCall = true;
                }
            }
    
            if(playerMarker.TryGetComponent<CampaignPlayerMarkerOverride>(out var overrider))
            {
                if(overrider.RigManagerOverride.TryGetCrate(out _))
                {
                    rigManSpawn = Campaign.Session.RigManagerOverride;
                    shouldCancelOriginalCall = true;
                }
                if(overrider.GameplayRigOverride.TryGetCrate(out _))
                {
                    gameplayRigSpawn = Campaign.Session.GameplayRigOverride;
                    shouldCancelOriginalCall = true;
                }
            }

            if(shouldCancelOriginalCall)
            {
                HelperMethods.SpawnCrate(
                    rigManSpawn, 
                    playerMarker.transform.position, 
                    playerMarker.transform.rotation,
                    spawnAction: playerMarker.OnPlayerSpawned, 
                    despawnAction: playerMarker.OnPlayerDespawn);
                
                HelperMethods.SpawnCrate(
                    gameplayRigSpawn,
                    playerMarker.transform.position,
                    playerMarker.transform.rotation
                );
            }

            return !shouldCancelOriginalCall;
        }
    }
}
