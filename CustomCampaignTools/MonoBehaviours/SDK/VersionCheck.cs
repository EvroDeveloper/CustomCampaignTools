using UnityEngine;
using MelonLoader;
using CustomCampaignTools;
using Il2CppUltEvents;

namespace CustomCampaignTools.SDK
{
    [RegisterTypeInIl2Cpp]
    public class VersionCheck : MonoBehaviour
    {
        public VersionCheck(IntPtr ptr) : base(ptr) { }

        public void SetActiveIfGreaterOrEqual(GameObject target, string minVersion, bool active)
        {
            bool isGoodVersion = IsCurrentVersionGreaterOrEqual(minVersion);

            if(isGoodVersion)
            {
                target.SetActive(active);
            }

        }

        public void InvokeIfGreaterOrEqual(UltEventHolder target, string minVersion)
        {
            bool isGoodVersion = IsCurrentVersionGreaterOrEqual(minVersion);

            if(isGoodVersion)
            {
                target.Invoke();
            }
        }

        public bool IsCurrentVersionGreaterOrEqual(string targetVersion)
        {
            string[] parsedTargetVer = targetVersion.Split(".");
            int majorTargetVer = int.Parse(parsedTargetVer[0]);
            int minorTargetVer = int.Parse(parsedTargetVer[1]);
            int patchTargetVer = int.Parse(parsedTargetVer[2]);

            string[] parsedCurVer = BuildInfo.Version.Split(".");
            int curMajorVer = int.Parse(parsedCurVer[0]);
            int curMinorVer = int.Parse(parsedCurVer[1]);
            int curPatchVer = int.Parse(parsedCurVer[2]);

            if(curMajorVer < majorTargetVer) return false;
            else if(curMajorVer > majorTargetVer) return true; // Major Greater
            // If they are equal, test Minor ver
            if(curMinorVer < minorTargetVer) return false;
            else if (curMinorVer > minorTargetVer) return true; // Major equals and Minor Greater
            // If they are equal, test Patch ver
            if(curPatchVer < patchTargetVer) return false;
            return true; // Major equals, Minor equals, Patch not less, good!!
        }
    }
}
