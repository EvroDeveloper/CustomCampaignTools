// Originally used for BoneLib
// https://github.com/yowchap/BoneLib/blob/main/BoneLib/BoneLibUpdater/Main.cs

using MelonLoader;
using MelonLoader.Utils;

using System;
using System.Drawing;
using System.IO;
using System.Reflection;

using static MelonLoader.MelonLogger;

namespace CampaignToolsUpdater;

public struct CampaignToolsUpdaterVersion
{
    public const byte versionMajor = 1;
    public const byte versionMinor = 0;
    public const short versionPatch = 0;
}

public class CampaignToolsUpdaterPlugin : MelonPlugin
{
    public const string Name = "CampaignTools Updater";
    public const string Author = "EvroDev";
    public static readonly Version Version = new(CampaignToolsUpdaterVersion.versionMajor, CampaignToolsUpdaterVersion.versionMinor, CampaignToolsUpdaterVersion.versionPatch);

    public static CampaignToolsUpdaterPlugin Instance { get; private set; }
    public static Instance Logger { get; private set; }
    public static Assembly UpdaterAssembly { get; private set; }

    private MelonPreferences_Category _prefCategory = MelonPreferences.CreateCategory("CampaignToolsUpdater");
    private MelonPreferences_Entry<bool> _offlineModePref;

    public bool IsOffline => _offlineModePref.Value;

    public const string ModName = "CustomCampaignTools";
    public const string PluginName = "CampaignToolsUpdater";
    public const string FileExtension = ".dll";

    public static readonly string ModAssemblyPath = Path.Combine(MelonEnvironment.ModsDirectory, $"{ModName}{FileExtension}");
    public static readonly string PluginAssemblyPath = Path.Combine(MelonEnvironment.PluginsDirectory, $"{PluginName}{FileExtension}");

    public override void OnPreInitialization()
    {
        Instance = this;
        Logger = LoggerInstance;
        UpdaterAssembly = MelonAssembly.Assembly;

        _offlineModePref = _prefCategory.CreateEntry("OfflineMode", false);
        _prefCategory.SaveToFile(false);

        LoggerInstance.Msg(IsOffline ? Color.Yellow : Color.Green, IsOffline ? "Fusion Auto-Updater is OFFLINE." : "Fusion Auto-Updater is ONLINE.");

        if (IsOffline) 
        {
            if (!File.Exists(ModAssemblyPath)) 
            {
                LoggerInstance.Warning($"{ModName}{FileExtension} was not found in the Mods folder!");
                LoggerInstance.Warning("Download it from the Github or switch to ONLINE mode.");
                LoggerInstance.Warning("https://github.com/EvroDeveloper/CustomCampaignTools/releases");
            }
        }
        else 
        {
            Updater.UpdateMod();
        }
    }

    public override void OnApplicationQuit() 
    {
        Updater.UpdatePlugin();
    }
}