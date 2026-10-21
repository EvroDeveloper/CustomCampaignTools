using BoneLib;
using CustomCampaignTools.GameSupport;
using CustomCampaignTools.Patching;
using CustomCampaignTools.Timing;
using CustomCampaignTools.Utilities;
using CustomCampaignTools.Utilities.Patching;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Warehouse;
using MelonLoader;
using System;
using System.Reflection;
using UnityEngine;

namespace CustomCampaignTools;

internal class Main : MelonMod
{
    public static Assembly ModAssembly;
    public override void OnInitializeMelon()
    {
        ModAssembly = MelonAssembly.Assembly;
        CampaignPatcher.PatchAssembly(ModAssembly);
        GameManager.OnInitialize();
        ArgumentHandler.HandleArguments(Environment.GetCommandLineArgs());
    }

    public override void OnLateInitializeMelon()
    {
        GameManager.OnLateInitialize();

        Hooking.OnLevelLoaded += OnLevelInitialized;
        Hooking.OnLevelUnloaded += OnLevelUnloaded;
        Hooking.OnMarrowGameStarted += OnMarrowGameStarted;
    }

    public override void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
        GameManager.OnSceneWasInitialized(buildIndex, sceneName);
        CampaignForcing.OnSceneWasInitialized(buildIndex);
    }

    internal static void OnLevelInitialized(LevelInfo info)
    {
        GameManager.OnLevelLoaded(info);
        Campaign.OnLevelLoaded(info);
        LevelLoadingPatches.OnLevelLoaded(info);
    }

    private static void OnLevelUnloaded()
    {
        if (Campaign.SessionActive)
        {
            if (Campaign.lastLoadedCampaignLevel == null) return;
            Campaign.Session.saveData.SaveAmmoForLevel(Campaign.lastLoadedCampaignLevel);
            LevelTiming.OnCampaignLevelUnloaded(Campaign.Session, Campaign.lastLoadedCampaignLevel.BarcodeString);
        }
    }

    internal static DataCardReference<MonoDisc> DefaultLoadSceneMusic { get; private set; }
    private static void OnMarrowGameStarted()
    {
        DefaultLoadSceneMusic = MarrowSettings.RuntimeInstance._loadMusic;
    }
}
