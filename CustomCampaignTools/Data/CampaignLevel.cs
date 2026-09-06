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

        /// <summary>
        /// Gets the current CampaignLevel, returns Null if the level is not in a campaign
        /// </summary>
        public static CampaignLevel Session;
        public static bool SessionActive => Session != null;
        public static CampaignLevelType SessionType => Session != null ? Session.type : CampaignLevelType.None;

        public CampaignLevel(Campaign campaign, string barcode, string name, CampaignLevelType type)
        {
            Barcode = new Barcode(barcode);
            _overrideName = name;
            this.type = type;
        }
        public CampaignLevel(Campaign campaign, Barcode barcode, string name, CampaignLevelType type)
        {
            Barcode = barcode;
            _overrideName = name;
            this.type = type;
        }

        public CampaignLevel(Campaign campaign, SerializedLevelSetup levelSetup, CampaignLevelType type)
        {
            Barcode = levelSetup.levelBarcode;
            _overrideName = levelSetup.levelName;
            this.type = type;
        }

        public CampaignLevel(CampaignLevel copy)
        {
            campaign = copy.campaign;
            Barcode = copy.Barcode;
            _overrideName = copy._overrideName;
            type = copy.type;
        }

        public bool IsValid()
        {
            return AssetWarehouse.Instance.HasCrate(Barcode);
        }

        public static implicit operator Barcode(CampaignLevel c) => c.Barcode;
        public static implicit operator LevelCrate(CampaignLevel c) => c.Crate;
        public static implicit operator LevelCrateReference(CampaignLevel c) => new LevelCrateReference(c.Barcode);
    }

    public class MainLevel : CampaignLevel
    {
        public int LevelIndex
        {
            get
            {
                if(_levelIndex == -1)
                {
                    _levelIndex = campaign.GetMainLevelIndex(Barcode);
                }
                return _levelIndex;
            }
        }

        private int _levelIndex = -1;

        public MainLevel(CampaignLevel copy) : base(copy)
        {
        }

        public MainLevel(Campaign campaign, SerializedLevelSetup levelSetup) : base(campaign, levelSetup, CampaignLevelType.MainLevel)
        {
        }

        public MainLevel(Campaign campaign, string barcode, string name) : base(campaign, barcode, name, CampaignLevelType.MainLevel)
        {
        }

        public MainLevel(Campaign campaign, Barcode barcode, string name) : base(campaign, barcode, name, CampaignLevelType.MainLevel)
        {
        }
    }

    public class ExtraLevel : CampaignLevel
    {
        public ExtraLevel(CampaignLevel copy) : base(copy)
        {
        }

        public ExtraLevel(Campaign campaign, SerializedLevelSetup levelSetup) : base(campaign, levelSetup, CampaignLevelType.ExtraLevel)
        {
        }

        public ExtraLevel(Campaign campaign, string barcode, string name) : base(campaign, barcode, name, CampaignLevelType.ExtraLevel)
        {
        }

        public ExtraLevel(Campaign campaign, Barcode barcode, string name) : base(campaign, barcode, name, CampaignLevelType.ExtraLevel)
        {
        }
    }

    public class MenuLevel : CampaignLevel
    {
        public MenuLevel(CampaignLevel copy) : base(copy)
        {
        }

        public MenuLevel(Campaign campaign, SerializedLevelSetup levelSetup, CampaignLevelType type) : base(campaign, levelSetup, type)
        {
        }

        public MenuLevel(Campaign campaign, string barcode, string name, CampaignLevelType type) : base(campaign, barcode, name, type)
        {
        }

        public MenuLevel(Campaign campaign, Barcode barcode, string name, CampaignLevelType type) : base(campaign, barcode, name, type)
        {
        }
    }

    public class IntroLevel : CampaignLevel
    {
        public IntroLevel(CampaignLevel copy) : base(copy)
        {
        }

        public IntroLevel(Campaign campaign, SerializedLevelSetup levelSetup, CampaignLevelType type) : base(campaign, levelSetup, type)
        {
        }

        public IntroLevel(Campaign campaign, string barcode, string name, CampaignLevelType type) : base(campaign, barcode, name, type)
        {
        }

        public IntroLevel(Campaign campaign, Barcode barcode, string name, CampaignLevelType type) : base(campaign, barcode, name, type)
        {
        }
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
