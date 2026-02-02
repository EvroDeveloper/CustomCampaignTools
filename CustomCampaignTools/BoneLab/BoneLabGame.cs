using BoneLib;
using CustomCampaignTools.Debug;
using CustomCampaignTools.Games.BoneLab;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppSLZ.Bonelab;
using CustomCampaignTools.Bonemenu;

namespace CustomCampaignTools.Games
{
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

        public override void GameSpecificPatches()
        {
            GashaponPatches.ManualPatch();
        }

        public override void OnLateInitialize()
        {
            BoneMenuCreator.CreateBoneMenu();
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
}