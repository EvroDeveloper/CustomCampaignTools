using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labworks.Utilities
{
    public class LevelParsing
    {
        private static readonly string[] _extraLevels = { "volx4.LabWorksBoneworksPort.Level.BoneworksRedactedChamber", "volx4.LabWorksBoneworksPort.Level.BoneworksMainMenu", "volx4.LabWorksBoneworksPort.Level.BoneworksLoadingScreen" };

        private static readonly string[] _levelBarcodes = {
            "volx4.LabWorksBoneworksPort.Level.Boneworks01Breakroom",
            "volx4.LabWorksBoneworksPort.Level.Boneworks02Museum",
            "volx4.LabWorksBoneworksPort.Level.Boneworks03Streets",
            "volx4.LabWorksBoneworksPort.Level.Boneworks04Runoff",
            "volx4.LabWorksBoneworksPort.Level.Boneworks05Sewers",
            "volx4.LabWorksBoneworksPort.Level.Boneworks06Warehouse",
            "volx4.LabWorksBoneworksPort.Level.Boneworks07CentralStation",
            "volx4.LabWorksBoneworksPort.Level.Boneworks08Tower",
            "volx4.LabWorksBoneworksPort.Level.Boneworks09TimeTower",
            "volx4.LabWorksBoneworksPort.Level.Boneworks10Dungeon",
            "volx4.LabWorksBoneworksPort.Level.Boneworks11Arena",
            "volx4.LabWorksBoneworksPort.Level.Boneworks12ThroneRoom" };

        /// <summary>
        /// Returns true if the level barcode is part of the Labworks campaign.
        /// </summary>
        /// <param name="palletTitle"></param>
        /// <param name="levelBarcode"></param>
        /// <returns></returns>
        [Obsolete("Switch to using generalized campaign solution")]
        public static bool IsLabworksCampaign(string palletTitle, string levelBarcode)
        {
            if (palletTitle != "LabWorksBoneworksPort")
                return false;

            if (_extraLevels.Contains(levelBarcode))
                return false;

            return true;
        }

        public static bool IsCampaignLevel(string palletTitle, string levelBarcode, out Campaign campaign)
        {
            foreach (Campaign c in Campaign.LoadedCampaigns)
            {
                if(palletTitle != c.Name) continue;
                
                if(c.mainLevels.Contains(levelBarcode)) 
                {
                    campaign = c;
                    return true;
                }
            }
            campaign = null;
            return false;
        }

        public static int GetLevelIndex(Campaign campaign, string levelBarcode)
        {
            return Array.IndexOf(campaign.mainLevels, levelBarcode);
        }

        public static string GetLevelBarcodeByIndex(Campaign campaign, int index)
        {
            return campaign.mainLevels[index];
        }
    }
}
