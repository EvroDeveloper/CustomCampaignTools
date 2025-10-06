#if MELONLOADER
using MelonLoader;
using CustomCampaignTools;
using Il2CppUltEvents;
#else
using UltEvents;
#endif
using UnityEngine;

namespace CustomCampaignTools.SDK
{
#if MELONLOADER
    [RegisterTypeInIl2Cpp]
#else
    [AddComponentMenu("CustomCampaignTools/UltEvent Utilities/Version Check")]
#endif
    public class VersionCheck : MonoBehaviour
    {
#if MELONLOADER
        public VersionCheck(IntPtr ptr) : base(ptr) { }
#endif

        public void SetActiveIfGreaterOrEqual(GameObject target, string minVersion, bool active)
        {
#if MELONLOADER
            bool isGoodVersion = IsCurrentVersionGreaterOrEqual(minVersion);

            if (isGoodVersion)
            {
                target.SetActive(active);
            }
#endif
        }

        public void InvokeIfGreaterOrEqual(UltEventHolder target, string minVersion)
        {
#if MELONLOADER
            bool isGoodVersion = IsCurrentVersionGreaterOrEqual(minVersion);

            if (isGoodVersion)
            {
                target.Invoke();
            }
#endif
        }

        public bool IsCurrentVersionGreaterOrEqual(string targetVersion)
        {
#if MELONLOADER
            string[] parsedTargetVer = targetVersion.Split(".");
            int majorTargetVer = int.Parse(parsedTargetVer[0]);
            int minorTargetVer = int.Parse(parsedTargetVer[1]);
            int patchTargetVer = int.Parse(parsedTargetVer[2]);

            string[] parsedCurVer = BuildInfo.Version.Split(".");
            int curMajorVer = int.Parse(parsedCurVer[0]);
            int curMinorVer = int.Parse(parsedCurVer[1]);
            int curPatchVer = int.Parse(parsedCurVer[2]);

            if (curMajorVer < majorTargetVer) return false;
            else if (curMajorVer > majorTargetVer) return true; // Major Greater
            // If they are equal, test Minor ver
            if (curMinorVer < minorTargetVer) return false;
            else if (curMinorVer > minorTargetVer) return true; // Major equals and Minor Greater
            // If they are equal, test Patch ver
            if (curPatchVer < patchTargetVer) return false;
            return true; // Major equals, Minor equals, Patch not less, good!!
#else
            return false;
#endif
        }
    }
}
