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

        /// <summary>
        /// Returns true if the level barcode is part of the Labworks campaign.
        /// </summary>
        /// <param name="palletTitle"></param>
        /// <param name="levelBarcode"></param>
        /// <returns></returns>
        public static bool IsLabworksCampaign(string palletTitle, string levelBarcode)
        {
            if (palletTitle != "LabWorksBoneworksPort")
                return false;

            if (_extraLevels.Contains(levelBarcode))
                return false;

            return true;
        }
    }
}
