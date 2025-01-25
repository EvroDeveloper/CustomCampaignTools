using CustomCampaignTools;
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

        public static bool IsCampaignLevel(string palletTitle, string levelBarcode, out Campaign campaign)
        {
            foreach (Campaign c in Campaign.LoadedCampaigns)
            {
                if(palletTitle != c.Name && palletTitle != string.Empty) continue;
                
                if(c.mainLevels.Contains(levelBarcode)) 
                {
                    campaign = c;
                    return true;
                }
            }
            campaign = null;
            return false;
        }
    }
}
