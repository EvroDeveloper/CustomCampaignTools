using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Il2CppSLZ.Marrow.SceneStreaming;
using Il2CppSLZ.Marrow.Warehouse;

namespace CustomCampaignTools.Patching
{
    [HarmonyPatch(typeof(SceneStreamer))]
    public static class LevelLoadingPatches
    {
        [HarmonyPatch(nameof(SceneStreamer.Load), [typeof(Barcode), typeof(Barcode)])]
        [HarmonyPrefix]
        public static bool LoadPrefixPatch(Barcode levelBarcode, ref Barcode loadLevelBarcode)
        {
            var campaign = CampaignUtilities.GetFromLevel(levelBarcode);

            // If logic here is kinda weird but this should work.
            if(Campaign.SessionLocked)
            {
                if(campaign != null && Campaign.Session == campaign)
                {
                    // Session is locked, and campaign matches session. Allow level change, ensure load scene blah;
                    campaign.saveData.UnlockLevel(levelBarcode.ID);
                }
                else
                {
                    // Session is locked, and campaign is either null, nor not the current session. Don't allow level change
                    return false;
                }
            }

            if(campaign != null)
            {
                if (loadLevelBarcode.ID != campaign.LoadScene)
                {
                    loadLevelBarcode = new Barcode(campaign.LoadScene);
                }
                Campaign.Session = campaign;
            }

            return true;
        }
    }
}
