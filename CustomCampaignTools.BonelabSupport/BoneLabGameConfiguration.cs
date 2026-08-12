using BoneLib;
using CustomCampaignTools.Debug;
using CustomCampaignTools.GameSupport;
using CustomCampaignTools.GameSupport.BoneLab;
using CustomCampaignTools.Patching;
using CustomCampaignTools.Utilities;
using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Bonelab.SaveData;
using Il2CppSLZ.Marrow.Warehouse;
using UnityEngine;

namespace CustomCampaignTools.BonelabSupport;

public class BoneLabGameConfiguration : GameConfiguration
{
    public override LevelCrateReference MainMenu
    {
        get
        {
            return new LevelCrateReference(DataManager.ActiveSave.Progression.BeatGame ? CommonBarcodes.Maps.VoidG114 : CommonBarcodes.Maps.MainMenu);
        }
    }
    public override void OnLateInitialize()
    {
        BoneMenuCreator.CreateBoneMenu();
        BoneLabMainMenuMangler.CampaignSprite = ResourceLoader.GetSprite(SupportAssembly, "CampaignIcon.png", new Vector2(0.5f, 0.5f), 100f, true);
        if (HelperMethods.CheckIfAssemblyLoaded("BrowsingPlus"))
        {
            PatchSwipezBecauseLemonloaderKeepsFuckingFailingIfIPutThisMethodInOnLateInitializeMelonForSomeReason();
        }
    }

    private void PatchSwipezBecauseLemonloaderKeepsFuckingFailingIfIPutThisMethodInOnLateInitializeMelonForSomeReason()
    {
        SwipezPanelPatches.ManualPatch();
    }

    public override void RefreshCampaignMenu(Campaign campaign)
    {
        CampaignBoneMenu.CreateOrRefreshCampaignPage(campaign);
    }

    public override void OnBootstrapSceneLoaded()
    {
        CampaignLogger.Msg("Bonelab Bootstrapper Scene Loaded - Checking for Forced Campaign Load");
        if (ArgumentHandler.forcedCampaign)
        {
            AssetWarehouse.OnReady((Il2CppSystem.Action)(() =>
            {
                var bootstrapper = UnityEngine.Object.FindObjectOfType<SceneBootstrapper_Bonelab>();
                if (bootstrapper != null)
                {
                    Campaign c = CampaignUtilities.GetFromPallet(ArgumentHandler.campaignToLoad);
                    if (c == null)
                    {
                        CampaignLogger.Error($"Could not find campaign with the barcode {ArgumentHandler.campaignToLoad}, continuing as normal.");
                        ArgumentHandler.forcedCampaign = false;
                    }
                    bootstrapper.MenuHollowCrateRef = new LevelCrateReference(c.InitialLevel);
                    bootstrapper.VoidG114CrateRef = new LevelCrateReference(c.InitialLevel);
                }
            }));
        }
    }

    public override void OnLevelLoaded(LevelInfo info)
    {
        if (info.barcode == CommonBarcodes.Maps.VoidG114)
        {
            BoneLabMainMenuMangler.MangleMenu();
        }
    }

    public override void OnUIRigCreated()
    {
        BoneLabPlayerMenuMangler.MangleMenu();

        if (Campaign.SessionActive)
        {
            var popUpMenu = Player.UIRig.popUpMenu;

            if (Campaign.Session.RestrictDevTools && !Campaign.Session.saveData.DevToolsUnlocked)
            {
                popUpMenu.crate_SpawnGun = new GenericCrateReference(Barcode.EmptyBarcode());
                popUpMenu.crate_Nimbus = new GenericCrateReference(Barcode.EmptyBarcode());
            }
        }
    }
}