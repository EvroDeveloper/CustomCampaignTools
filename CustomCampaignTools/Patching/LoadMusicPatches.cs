using HarmonyLib;
using Il2CppSLZ.Marrow.Audio;
using UnityEngine;

namespace CustomCampaignTools.Patching
{
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
