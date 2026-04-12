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
        Type gameConfigurationType = AssemblyUtils.FindTypeInAssembly<GameConfiguration>(gameSupport);

        currentGameConfiguration = (GameConfiguration)Activator.CreateInstance(gameConfigurationType);
        currentGameConfiguration.SupportAssembly = gameSupport;
        AssemblyUtils.HarmonyPatchAssembly(gameSupport, "customcampaigntools.supportlibrary.patches"); // bullshit random string that means nothing to me
    }

    public static void OnLateInitialize()
    {
        currentGameConfiguration.OnLateInitialize();
    }

    public static void ManglePlayerMenu()
    {
        currentGameConfiguration.playerMenuMangler.MangleMenu();
    }

    public static void OnLevelLoaded(LevelInfo info)
    {
        if (info.barcode == currentGameConfiguration.mainMenuBarcode)
        {
            currentGameConfiguration.mainMenuMangler.MangleMenu();
        }
    }

    internal static void OnBootstrapSceneLoaded()
    {
        currentGameConfiguration.OnBootstrapSceneLoaded();
    }
}