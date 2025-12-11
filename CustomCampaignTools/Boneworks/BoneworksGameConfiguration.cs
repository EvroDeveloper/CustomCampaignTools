#if false
namespace CustomCampaignTools.Games
{
    public class BoneworksGameConfiguration : GameConfiguration
    {
        public BoneworksGameConfiguration()
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

        PlayerMenuMangler _playerMenuMangler;
        public override IMenuMangler playerMenuMangler
        {
            get
            {
                _playerMenuMangler ??= new BoneLabPlayerMenuMangler();
                return _playerMenuMangler;
            }
        }

        public void GameSpecificPatches()
        {
            GashaponPatches.ManualPatch();
        }
    }
}
#endif