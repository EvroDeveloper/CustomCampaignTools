using System;
using System.Reflection;
using BoneLib;
using CustomCampaignTools.Utilities;
using MelonLoader;

namespace CustomCampaignTools.GameSupport;

public static class GameManager
{
    public static GameConfiguration currentGameConfiguration;

    public static void InitializeGameConfiguration()
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

        currentGameConfiguration.OnInitialize();
    }

    public static void OnLateInitialize()
    {
        currentGameConfiguration.OnLateInitialize();
    }

    public static void OnUIRigCreated()
    {
        currentGameConfiguration.OnUIRigCreated();
    }

    public static void OnLevelLoaded(LevelInfo info)
    {
        currentGameConfiguration.OnLevelLoaded(info);
    }

    internal static void OnBootstrapSceneLoaded()
    {
        currentGameConfiguration.OnBootstrapSceneLoaded();
    }
}