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
        public static void LoadPrefixPatch(Barcode levelBarcode, Barcode loadLevelBarcode)
        {
            var campaign = CampaignUtilities.GetFromLevel(levelBarcode);

            if(campaign != null)
            {
                if (loadLevelBarcode.ID != campaign.LoadScene)
                {
                    loadLevelBarcode = new Barcode(campaign.LoadScene);
                }
                Campaign.Session = campaign;
            }
        }
    }
}
