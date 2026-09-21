using System.Collections.Generic;
using System.Linq;
using Il2CppSLZ.Marrow.Utilities;
using Il2CppSLZ.Marrow.Warehouse;

namespace CustomCampaignTools
{
    public class CampaignLevel
    {
        public Campaign campaign;

        public Barcode Barcode;
        public string BarcodeString => Barcode.ID;

        public string Title
        {
            get
            {
                if(_overrideName == string.Empty) return Crate.Title;
                return _overrideName;
            }
        }
        private readonly string _overrideName = "";

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

        public CampaignLevel(Campaign campaign, string barcode, string name)
        {
            this.campaign = campaign;
            Barcode = new Barcode(barcode);
            _overrideName = name;
        }
        public CampaignLevel(Campaign campaign, Barcode barcode, string name)
        {
            this.campaign = campaign;
            Barcode = barcode;
            _overrideName = name;
        }

        public CampaignLevel(Campaign campaign, SerializedLevelSetup levelSetup)
        {
            this.campaign = campaign;
            Barcode = levelSetup.levelBarcode;
            _overrideName = levelSetup.levelName;
        }

        public CampaignLevel(CampaignLevel copy)
        {
            campaign = copy.campaign;
            Barcode = copy.Barcode;
            _overrideName = copy._overrideName;
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
                    _levelIndex = campaign.GetMainLevelIndex(this);
                }
                return _levelIndex;
            }
        }

        private int _levelIndex = -1;

        public MainLevel(CampaignLevel copy) : base(copy)
        {
        }

        public MainLevel(Campaign campaign, SerializedLevelSetup levelSetup) : base(campaign, levelSetup)
        {
        }

        public MainLevel(Campaign campaign, string barcode, string name) : base(campaign, barcode, name)
        {
        }

        public MainLevel(Campaign campaign, Barcode barcode, string name) : base(campaign, barcode, name)
        {
        }
    }

    public class ExtraLevel : CampaignLevel
    {
        public ExtraLevel(CampaignLevel copy) : base(copy)
        {
        }

        public ExtraLevel(Campaign campaign, SerializedLevelSetup levelSetup) : base(campaign, levelSetup)
        {
        }

        public ExtraLevel(Campaign campaign, string barcode, string name) : base(campaign, barcode, name)
        {
        }

        public ExtraLevel(Campaign campaign, Barcode barcode, string name) : base(campaign, barcode, name)
        {
        }
    }

    public class MenuLevel : CampaignLevel
    {
        public MenuLevel(CampaignLevel copy) : base(copy)
        {
        }

        public MenuLevel(Campaign campaign, SerializedLevelSetup levelSetup) : base(campaign, levelSetup)
        {
        }

        public MenuLevel(Campaign campaign, string barcode, string name) : base(campaign, barcode, name)
        {
        }

        public MenuLevel(Campaign campaign, Barcode barcode, string name) : base(campaign, barcode, name)
        {
        }
    }

    public class IntroLevel : CampaignLevel
    {
        public IntroLevel(CampaignLevel copy) : base(copy)
        {
        }

        public IntroLevel(Campaign campaign, SerializedLevelSetup levelSetup) : base(campaign, levelSetup)
        {
        }

        public IntroLevel(Campaign campaign, string barcode, string name) : base(campaign, barcode, name)
        {
        }

        public IntroLevel(Campaign campaign, Barcode barcode, string name) : base(campaign, barcode, name)
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
