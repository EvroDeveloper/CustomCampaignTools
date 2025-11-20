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
            string[] parsedCurVer = BuildInfo.Version.Split(".");
            int curMajorVer = int.Parse(parsedCurVer[0]);
            int curMinorVer = int.Parse(parsedCurVer[1]);
            int curPatchVer = int.Parse(parsedCurVer[2]);

            string[] parsedTargetVer = targetVersion.Split(".");
            int tarMajorVer = int.Parse(parsedTargetVer[0]);
            int tarMinorVer = int.Parse(parsedTargetVer[1]);
            int tarPatchVer = int.Parse(parsedTargetVer[2]);

            CampaignLogger.Msg(Campaign.Session, $"Comparing Current Version {curMajorVer}.{curMinorVer}.{curPatchVer} to Target Version {tarMajorVer}.{tarMinorVer}.{tarPatchVer}");

            if (curMajorVer < tarMajorVer) return false;
            else if (curMajorVer > tarMajorVer) return true; // Major Greater
            CampaignLogger.Msg(Campaign.Session, "Major Versions Equal, testing Minor Version");
            // If they are equal, test Minor ver
            if (curMinorVer < tarMinorVer) return false;
            else if (curMinorVer > tarMinorVer) return true; // Major equals and Minor Greater
            CampaignLogger.Msg(Campaign.Session, "Minor Versions Equal, testing Patch Version");
            // If they are equal, test Patch ver
            if (curPatchVer < tarPatchVer) return false;
            CampaignLogger.Msg(Campaign.Session, "Patch Version Greater or Equal");
            return true; // Major equals, Minor equals, Patch not less, good!!
#else
            return false;
#endif
        }
    }
}
