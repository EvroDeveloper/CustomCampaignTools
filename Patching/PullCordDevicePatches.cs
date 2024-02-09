using HarmonyLib;
using Labworks.Data;
using SLZ.Marrow.SceneStreaming;
using SLZ.Marrow.Warehouse;
using SLZ.Props;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labworks.Patching
{
    [HarmonyPatch(typeof(PullCordDevice))]
    internal class PullCordDevicePatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(PullCordDevice.OnEnable))]
        public static void OnEnable(PullCordDevice __instance)
        {
            if (SceneStreamer.Session.Level.Pallet.Title == "LabWorksBoneworksPort" && LabworksSaving.IsFordOnlyMode)
            {
                __instance.gameObject.SetActive(false);
            }
        }
    }
}
