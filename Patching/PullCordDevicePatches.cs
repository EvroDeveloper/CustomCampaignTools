using HarmonyLib;
using Il2CppSLZ.Bonelab;

namespace Labworks.Patching
{
    [HarmonyPatch(typeof(PullCordDevice))]
    internal class PullCordDevicePatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(PullCordDevice.OnEnable))]
        public static void OnEnable(PullCordDevice __instance)
        {
        }
    }
}
