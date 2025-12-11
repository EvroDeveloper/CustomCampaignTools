using BoneLib;
using CustomCampaignTools.Games.BoneLab;

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
    }
}