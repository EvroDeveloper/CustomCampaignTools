using Il2CppSLZ.Marrow.Utilities;
using Il2CppSLZ.Marrow.Warehouse;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CustomCampaignTools
{
    public class CampaignLevel
    {
        public Campaign campaign;

        public Barcode Barcode;
        public string Title
        {
            get{
                if(overrideName == string.Empty) return crate.Title;
                return overrideName;
            }
        }

        public string sBarcode => Barcode.ID;

        private string overrideName = "";

        public CampaignLevelType type;

        public LevelCrate crate
        {
            get {
                if(_crate != null) return _crate;
                MarrowGame.assetWarehouse.TryGetCrate(Barcode, out _crate);
                return _crate;
            }
        }

        private LevelCrate _crate;

        public bool isUnlocked
        {
            get {
                return !campaign.LockLevelsUntilEntered || campaign.saveData.UnlockedLevels.Contains(sBarcode);
            }
        }

        public CampaignLevel(string barcode, string name, CampaignLevelType type)
        {
            Barcode = new Barcode(barcode);
            overrideName = name;
            this.type = type;
        }

        public CampaignLevel(SerializedLevelSetup levelSetup, CampaignLevelType type)
        {
            Barcode = new Barcode(levelSetup.levelBarcode);
            overrideName = levelSetup.levelName;
            this.type = type;
        }

        public CampaignLevel(CampaignLevel copy)
        {
            Barcode = copy.Barcode;
            overrideName = copy.overrideName;
            type = copy.type;
        }

        public static implicit operator Barcode(CampaignLevel c) => c.Barcode;
        public static implicit operator LevelCrate(CampaignLevel c) => c.crate;
    }

    public static class CampaignLevelListManipulation
    {
        public static List<string> ToBarcodeStrings(this CampaignLevel[] list)
        {
            return list.Select(c => c.Barcode.ID).ToList();
        }

        public static List<Barcode> ToBarcodes(this CampaignLevel[] list)
        {
            return list.Select(c => c.Barcode).ToList();
        }

        public static List<string> ToNames(this List<CampaignLevel> list)
        {
            return list.Select(c => c.Title).ToList();
        }

        public static List<LevelCrate> ToCrates(this CampaignLevel[] list)
        {
            return list.Select(c => c.crate).Where(c => c != null).ToList();
        }
    }
}
