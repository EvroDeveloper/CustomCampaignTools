using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var campaign = CampaignUtilities.GetFromLevel(level);

            SavepointFunctions.CurrentLevelLoadedByContinue = SavepointFunctions.WasLastLoadByContinue;
            SavepointFunctions.WasLastLoadByContinue = false;

            if(level.Barcode.ID == CommonBarcodes.Maps.VoidG114) return true;

            // If loading into a different campaign than the session, and it's levels are locked, and the desired level is locked
            if(Campaign.Session != campaign && campaign.LockLevelsUntilEntered && !campaign.saveData.UnlockedLevels.Conatins(level.Barcode.ID))
            {
                Notifier.Send(new Notification()
                {
                    Title = campaign.Name,
                    Message = "This level has not yet been unlocked",
                    Type = NotificationType.Error,
                    ShowTitleOnPopup = true,
                });

                __result = new UniTask<Poolee>(null);
                return false;
            }

            
            // If logic here is kinda weird but this should work.
            if(Campaign.SessionLocked)
            {
                // Currently in Locked Campaign
                if(campaign != null && Campaign.Session == campaign)
                {
                    // Loading into same campaign
                    // Make sure the target level is unlocked and continue
                    campaign.saveData.UnlockLevel(level.Barcode.ID);
                }
                else
                {
                    // Session is locked, and campaign is either null, nor not the current session. Don't allow level change
                    Notifier.Send(new Notification()
                    {
                        Title = Campaign.Session.Name,
                        Message = "You are currently locked in the campaign. To exit, please click Exit Campaign in your menu.",
                        Type = NotificationType.Error,
                        ShowTitleOnPopup = true,
                    });

                    __result = new UniTask<Poolee>(null);
                    return false;
                }
            }
            else if(campaign != null)
            {
                // Not locked, but loading into a campaign.
                // If loading into a different campaign than the session, and it's levels are locked, and the desired level is locked
                if(Campaign.Session != campaign && campaign.LockLevelsUntilEntered && !campaign.saveData.UnlockedLevels.Conatins(level.Barcode.ID))
                {
                    Notifier.Send(new Notification()
                    {
                        Title = campaign.Name,
                        Message = "This level has not yet been unlocked",
                        Type = NotificationType.Error,
                        ShowTitleOnPopup = true,
                    });

                    __result = new UniTask<Poolee>(null);
                    return false;
                }
                
                // If the level its loading into is a campaign level, force load scene to be Campaign Load scene
                if (loadLevel.Barcode.ID != campaign.LoadScene)
                {
                    loadLevel = new LevelCrateReference(new Barcode(campaign.LoadScene));
                }
                Campaign.Session = campaign;
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
