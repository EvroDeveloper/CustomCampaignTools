using System;
using System.Reflection;
using BoneLib;
using CustomCampaignTools.Utilities;
using CustomCampaignTools.Utilities.Patching;
using MelonLoader;

namespace CustomCampaignTools.GameSupport;

public static class GameManager
{
    public static GameConfiguration currentGameConfiguration { get; private set; }

    public static void OnInitialize()
    {
        string supportLibraryLoadPath;
        if (MelonUtils.CurrentGameAttribute.Name == "BONELAB")
            supportLibraryLoadPath = "CustomCampaignTools.GameSupport.Libraries.BonelabSupport.dll";
        else if (MelonUtils.CurrentGameAttribute.Name == "BONEWORKS")
            supportLibraryLoadPath = "CustomCampaignTools.GameSupport.Libraries.BoneworksSupport.dll";
        else
            return;
        
        Assembly gameSupport = AssemblyUtils.LoadEmbeddedAssembly(Main.ModAssembly, supportLibraryLoadPath);

        currentGameConfiguration = AssemblyUtils.FindInheritingTypeAndCreate<GameConfiguration>(gameSupport);
        currentGameConfiguration.SupportAssembly = gameSupport;

        currentGameConfiguration.GameDataManager = AssemblyUtils.FindInheritingTypeAndCreate<IGameDataManager>(gameSupport);

        AssemblyUtils.HarmonyPatchAssembly(gameSupport, "customcampaigntools.supportlibrary.patches"); // bullshit random string that means nothing to me
        CampaignPatcher.PatchAssembly(gameSupport);

        currentGameConfiguration.OnInitialize();
    }

    internal static void OnLateInitialize()
    {
        currentGameConfiguration.OnLateInitialize();
    }

    internal static void OnLevelLoaded(LevelInfo info)
    {
        currentGameConfiguration.OnLevelLoaded(info);
    }

    internal static void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
        currentGameConfiguration.OnSceneWasInitialized(buildIndex, sceneName);
    }
}