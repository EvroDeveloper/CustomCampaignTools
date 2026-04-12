using CustomCampaignTools.GameSupport;
using BoneLib;
using CustomCampaignTools.Debug;
using CustomCampaignTools.GameSupport.BoneLab;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppSLZ.Bonelab;
using UnityEngine;
using CustomCampaignTools.Utilities;

namespace CustomCampaignTools.BonelabSupport;

public class BoneLabGameConfiguration : GameConfiguration
{
    public BoneLabGameConfiguration()
    {
        _mainMenuMangler = new BoneLabMainMenuMangler();
        _playerMenuMangler = new BoneLabPlayerMenuMangler();
    }
    BoneLabMainMenuMangler _mainMenuMangler;
    public override IMenuMangler mainMenuMangler
    {
        get
        {
            _mainMenuMangler ??= new BoneLabMainMenuMangler();
            return _mainMenuMangler;
        }
    }

    public override string mainMenuBarcode => CommonBarcodes.Maps.VoidG114;

    BoneLabPlayerMenuMangler _playerMenuMangler;
    public override IMenuMangler playerMenuMangler
    {
        get
        {
            _playerMenuMangler ??= new BoneLabPlayerMenuMangler();
            return _playerMenuMangler;
        }
    }

    public override void OnLateInitialize()
    {
        BoneMenuCreator.CreateBoneMenu();
        BoneLabMainMenuMangler.CampaignSprite = ResourceLoader.GetSprite(SupportAssembly, "CampaignIcon.png", new Vector2(0.5f, 0.5f), 100f, true);
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
}