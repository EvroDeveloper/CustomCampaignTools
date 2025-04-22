using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CustomCampaignTools
{
    public class CampaignLevel
    {
        public Campaign campaign;

        public Barcode levelBarcode;
        public string levelName
        {
            get{
                if(overrideName == string.Empty) return crate.Title;
                return overrideName;
            }
        };

        private string overrideName = "";

        public CampaignLevelType type;

        public LevelCrate crate
        {
            get {
                if(_crate != null) return _crate;
                MarrowGame.assetWarehouse.TryGetCrate(levelBarcode, out _crate);
                return _crate;
            }
        }

        private LevelCrate _crate;

        public bool isUnlocked
        {
            get {
                return !campaign.LockLevelsUntilEntered || campaign.saveData.UnlockedLevels.Contains(barcode);
            }
        }

        public CampaignLevel(string barcode, string name, CampaignLevelType type)
        {
            levelBarcode = new Barcode(barcode);
            overrideName = name;
            this.type = type;
        }

        public CampaignLevel(SerializedLevelSetup levelSetup, CampaignLevelType type)
        {
            levelBarcode = new Barcode(levelSetup.levelBarcode);
            overrideName = levelSetup.levelName;
            this.type = type;
        }
    }

    public static class CampaignLevelListManipulation
    {
        public static List<string> ToBarcodeStrings(this List<CampaignLevel> list)
        {
            return list.Select(c => c.levelBarcode.ID);
        }

        public static List<Barcode> ToBarcodes(this List<CampaignLevel> list)
        {
            return list.Select(c => c.levelBarcode);
        }

        public static List<string> ToNames(this List<CampaignLevel> list)
        {
            return list.Select(c => c.levelName);
        }

        public static List<LevelCrate> ToCrates(this List<CampaignLevel> list)
        {
            return list.Select(c => c.crate).Where(c => c != null);
        }
    }
}
