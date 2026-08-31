using HarmonyLib;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Audio;
using Il2CppSLZ.Marrow.Warehouse;
using UnityEngine;

namespace CustomCampaignTools.Patching;

[HarmonyPatch(typeof(Audio2dManager))]
public static class LoadMusicPatches
{
    private static string _loadingMusicName = "music_LoadingSplash";
    public static string loadingMusicName
    {
        get
        {
            if(string.IsNullOrEmpty(_loadingMusicName))
            {
                MarrowAssetT<AudioClip> gameLoadMusic = MarrowSettings.RuntimeInstance._loadMusic.DataCard.AudioClip;
                gameLoadMusic.LoadAsset((Il2CppSystem.Action<AudioClip>)((a) =>
                {
                    _loadingMusicName = a.name;
                    gameLoadMusic.ReleaseAsset();
                }));
                return "";
            }
            return _loadingMusicName;
        }
    }

    [HarmonyPatch(nameof(Audio2dManager.CueMusic), [typeof(AudioClip), typeof(float), typeof(float), typeof(float), typeof(bool)])]
    [HarmonyPatch(nameof(Audio2dManager.CueMusic), [typeof(double), typeof(AudioClip), typeof(float), typeof(float), typeof(float), typeof(bool)])]
    [HarmonyPatch(nameof(Audio2dManager.CueMusicInternal))]
    [HarmonyPrefix]
    public static void CueMusicPatch(Audio2dManager __instance, ref AudioClip musicClip)
    {
        SwapLoadingMusic(ref musicClip);
    }

    [HarmonyPatch(nameof(Audio2dManager.StopSpecificMusic))]
    [HarmonyPrefix]
    public static void StopMusicPatch(Audio2dManager __instance, ref AudioClip specificClip)
    {
        SwapLoadingMusic(ref specificClip);
    }

    private static void SwapLoadingMusic(ref AudioClip musicClip)
    {
        if(musicClip.name == loadingMusicName && Campaign.SessionActive && Campaign.Session.LoadSceneMusic != null)
        {
            musicClip = Campaign.Session.LoadSceneMusic;
        }
    }
}
