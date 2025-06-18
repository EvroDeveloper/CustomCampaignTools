using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoneLib;
using BoneLib.Notifications;
using HarmonyLib;
using Il2CppCysharp.Threading.Tasks;
using Il2CppSLZ.Marrow.Audio;
using Il2CppSLZ.Marrow.Pool;
using Il2CppSLZ.Marrow.SceneStreaming;
using Il2CppSLZ.Marrow.Warehouse;
using MelonLoader;
using UnityEngine;

namespace CustomCampaignTools.Patching
{
    [HarmonyPatch(typeof(SceneStreamer))]
    public static class LevelLoadingPatches
    {
        [HarmonyPatch(nameof(SceneStreamer.LoadAsync), [typeof(LevelCrateReference), typeof(LevelCrateReference)])]
        [HarmonyPrefix]
        public static bool LoadPrefixPatch(LevelCrateReference level, ref LevelCrateReference loadLevel, ref UniTask __result)
        {
            var destinationCampaign = CampaignUtilities.GetFromLevel(level);

            SavepointFunctions.CurrentLevelLoadedByContinue = SavepointFunctions.WasLastLoadByContinue;
            SavepointFunctions.WasLastLoadByContinue = false;

            if(Campaign.SessionActive && destinationCampaign == Campaign.Session)
            {
                if(Campaign.Session.TypeOfLevel(level.Barcode.ID) == CampaignLevelType.MainLevel && Campaign.Session.TypeOfLevel(SceneStreamer.Session.Level.Barcode.ID) == CampaignLevelType.MainLevel)
                {
                    if(Campaign.Session.GetLevelIndex(level.Barcode.ID) == Campaign.Session.GetLevelIndex(SceneStreamer.Session.Level.Barcode.ID) + 1)
                    {
                        Campaign.Session.saveData.SaveInventoryForLevel(level.Barcode.ID);
                    }
                }
            }

            if(destinationCampaign != null)
            {
                destinationCampaign.saveData.UnlockLevel(level.Barcode.ID);

                // If the level its loading into is a campaign level, force load scene to be Campaign Load scene
                if (loadLevel.Barcode.ID != destinationCampaign.LoadScene)
                {
                    loadLevel = new LevelCrateReference(new Barcode(destinationCampaign.LoadScene));
                }
                Campaign.Session = destinationCampaign;
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
}
