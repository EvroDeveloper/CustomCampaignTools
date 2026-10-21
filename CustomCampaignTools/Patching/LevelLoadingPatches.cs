using System;
using BoneLib;
using CustomCampaignTools.Debug;
using CustomCampaignTools.Timing;
using CustomCampaignTools.Utilities;
using CustomCampaignTools.Utilities.Patching;
using HarmonyLib;
using Il2CppCysharp.Threading.Tasks;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.SceneStreaming;
using Il2CppSLZ.Marrow.Utilities;
using Il2CppSLZ.Marrow.Warehouse;

namespace CustomCampaignTools.Patching;

[HarmonyPatch(typeof(SceneStreamer))]
public static class LevelLoadingPatches
{
    public static Action OnNextSceneLoaded = ()=>{};

    // This patch is insanely sketchy on Quest. I think it causes an error, and lakatrazz avoids patching the actual UniTask method on quest. And yet, it works. Dont know why
    [HarmonyPatch(nameof(SceneStreamer.LoadAsync), [typeof(LevelCrateReference), typeof(LevelCrateReference)])]
    [HarmonyPrefix]
    public static void LoadPrefixPatch(LevelCrateReference level, ref LevelCrateReference loadLevel, ref UniTask __result)
    {
        bool loadingIntoCampaign = CampaignUtilities.TryGetCampaignLevel(level.Barcode, out var destinationCampaignLevel);

        if(loadingIntoCampaign)
        {
            CampaignLogger.Msg("Loading into Campaign: " + destinationCampaignLevel.campaign.Name);
        }
       
        SavepointFunctions.CurrentLevelLoadedByContinue = SavepointFunctions.WasLastLoadByContinue;
        SavepointFunctions.WasLastLoadByContinue = false;

        if(loadingIntoCampaign && destinationCampaignLevel.campaign == Campaign.Session)
        {
            bool wasAlreadyInCampaign = CampaignUtilities.TryGetCampaignLevel(SceneStreamer.Session.Level.Barcode, out var previousCampaignLevel);
            if(wasAlreadyInCampaign && destinationCampaignLevel is MainLevel destMainLevel && previousCampaignLevel is MainLevel prevMainLevel)
            {
                if(destMainLevel.LevelIndex == prevMainLevel.LevelIndex + 1)
                {
                    Campaign.Session.saveData.SaveInventoryForLevel(level.Barcode.ID);
                }
            }
        }

        var destinationCampaign = destinationCampaignLevel?.campaign;

        if(loadingIntoCampaign)
        {
            destinationCampaign.saveData.UnlockLevel(level.Barcode.ID);
            CampaignLogger.MsgVerbose(destinationCampaign, "Unlocked level: " + destinationCampaignLevel.Title);

            // If the level its loading into is a campaign level, force load scene to be Campaign Load scene
            if(destinationCampaign.LoadScene.IsValid())
                loadLevel = destinationCampaign.LoadScene;

            if(Campaign.Session == null)
            {
                CampaignPatcher.OnEnterCampaign();
                CampaignLogger.MsgVerbose(destinationCampaign, "CampaignPatcher Enabling Campaign Patches");
            }

            Campaign.Session = destinationCampaign;
            CampaignLevel.Session = destinationCampaignLevel;

            OnNextSceneLoaded += () =>
            {
                LevelTiming.OnCampaignLevelLoaded(destinationCampaign, level.Barcode.ID);
            };

            if(destinationCampaignLevel is MainLevel)
            {
                if (!SavepointFunctions.CurrentLevelLoadedByContinue)
                {
                    if(destinationCampaign.CreateSaveOnLevelEnter) OnNextSceneLoaded += () => destinationCampaign.saveData.SavePlayer(destinationCampaignLevel, SimpleTransform.Identity);
                    if(destinationCampaign.SaveLevelInventory) OnNextSceneLoaded += () =>
                    {
                        if(destinationCampaign.saveData.InventorySaves.ContainsKey(level.Barcode.ID))
                        {
                            destinationCampaign.saveData.InventorySaves[level.Barcode.ID].ApplyToRigManagerDelayed();
                        }
                    };
                }
            }
            if(destinationCampaign.LoadSceneMusicReference.IsValid())
                MarrowSettings.RuntimeInstance._loadMusic = destinationCampaign.LoadSceneMusicReference;
            else
                MarrowSettings.RuntimeInstance._loadMusic = Main.DefaultLoadSceneMusic;
        }
        else
        {
            if(Campaign.Session != null)
            {
                CampaignPatcher.OnExitedCampaign();
            }
            Campaign.Session = null;
            CampaignLevel.Session = null;
            MarrowSettings.RuntimeInstance._loadMusic = Main.DefaultLoadSceneMusic;
        }

        RigReplacementUtility.OnLevelLoadStart(destinationCampaign);
    }

    public static void OnLevelLoaded(LevelInfo info)
    {
        SafeActions.InvokeActionSafe(OnNextSceneLoaded);
        OnNextSceneLoaded = () => {};
    }
}
