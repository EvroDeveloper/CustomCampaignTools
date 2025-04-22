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
        public string levelName;

        public bool isUnlocked
        {
            get {
                return !campaign.LockLevelsUntilEntered || campaign.saveData.UnlockedLevels.Contains(barcode);
            }
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
    }
}
