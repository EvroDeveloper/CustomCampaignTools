using System.Collections.Generic;
using System.Linq;
using Il2CppSLZ.Marrow.Utilities;
using Il2CppSLZ.Marrow.Warehouse;

namespace CustomCampaignTools
{
    public enum CampaignLevelType
    {
        None,
        Intro,
        Menu,
        MainLevel,
        ExtraLevel // Extra levels do not save ammo
    }

    public class CampaignLevel
    {
        public Campaign campaign;

        public Barcode Barcode;
        public string BarcodeString => Barcode.ID;

        public string Title
        {
            get{
                if(_overrideName == string.Empty) return Crate.Title;
                return _overrideName;
            }
        }
        private readonly string _overrideName = "";

        public CampaignLevelType type;

        public LevelCrate Crate
        {
            get
            {
                if (_crate != null) return _crate;
                MarrowGame.assetWarehouse.TryGetCrate(Barcode, out _crate);
                return _crate;
            }
        }
        private LevelCrate _crate;

        public bool Redacted
        {
            get
            {
                if (Crate != null) return Crate.Redacted;
                return true;
            }
        }

        public bool Unlocked
        {
            get {
                return !campaign.LockLevelsUntilEntered || campaign.saveData.UnlockedLevels.Contains(BarcodeString);
            }
        }

        public CampaignLevel(string barcode, string name, CampaignLevelType type)
        {
            Barcode = new Barcode(barcode);
            _overrideName = name;
            this.type = type;
        }
        public CampaignLevel(Barcode barcode, string name, CampaignLevelType type)
        {
            Barcode = barcode;
            _overrideName = name;
            this.type = type;
        }

        public CampaignLevel(SerializedLevelSetup levelSetup, CampaignLevelType type)
        {
            Barcode = levelSetup.levelBarcode;
            _overrideName = levelSetup.levelName;
            this.type = type;
        }

        public CampaignLevel(CampaignLevel copy)
        {
            Barcode = copy.Barcode;
            _overrideName = copy._overrideName;
            type = copy.type;
        }

        public bool IsValid()
        {
            return AssetWarehouse.Instance.HasCrate<LevelCrate>(Barcode);
        }

        public static implicit operator Barcode(CampaignLevel c) => c.Barcode;
        public static implicit operator LevelCrate(CampaignLevel c) => c.Crate;
        public static implicit operator LevelCrateReference(CampaignLevel c) => new LevelCrateReference(c.Barcode);
    }

    public static class CampaignLevelListManipulation
    {
        public static List<string> ToBarcodeStrings(this CampaignLevel[] list)
        {
            return [.. list.Select(c => c.Barcode.ID)];
        }

        public static List<Barcode> ToBarcodes(this CampaignLevel[] list)
        {
            return [.. list.Select(c => c.Barcode)];
        }

        public static List<string> ToNames(this List<CampaignLevel> list)
        {
            return [.. list.Select(c => c.Title)];
        }

        public static List<LevelCrate> ToCrates(this CampaignLevel[] list)
        {
            return [.. list.Select(c => c.Crate).Where(c => c != null)];
        }
    }
}
