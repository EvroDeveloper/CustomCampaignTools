using BoneLib;
using HarmonyLib;
using Labworks.Data;
using MelonLoader;
using SLZ.Marrow.SceneStreaming;
using SLZ.Props;
using SLZ.Rig;
using SLZ.VRMK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labworks.Patching
{
    [HarmonyPatch(typeof(RigManager))]
    internal class RigManagerPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(RigManager.SwapAvatarCrate))]
        public static void OnAvatarCrateChanged(ref string barcode, bool cache = false, Il2CppSystem.Action<bool> callback = null)
        {
            if (SceneStreamer.Session.Level.Pallet.Title == "LabWorksBoneworksPort" && LabworksSaving.IsFordOnlyMode)
            {
                barcode = "SLZ.BONELAB.Content.Avatar.FordBW";
            }
        }
    }
}
