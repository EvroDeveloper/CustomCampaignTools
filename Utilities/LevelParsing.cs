using CustomCampaignTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCampaignTools.Utilities
{
    public class LevelParsing
    {
        [Obsolete("Use CampaignUtilities")]
        public static bool IsCampaignLevel(string levelBarcode, out Campaign campaign, out CampaignLevelType levelType)
        {
            campaign = CampaignUtilities.GetFromLevel(levelBarcode);

            if (campaign != null)
                levelType = campaign.TypeOfLevel(levelBarcode);

            else levelType = CampaignLevelType.None;

            return campaign != null;
        }
    }
}
