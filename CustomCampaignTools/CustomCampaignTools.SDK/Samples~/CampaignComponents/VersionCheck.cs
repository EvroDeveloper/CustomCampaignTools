#if MELONLOADER
using MelonLoader;
using CustomCampaignTools;
using Il2CppUltEvents;
using CustomCampaignTools.Debug;
#else
using UltEvents;
#endif
using UnityEngine;
using System;

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
            CampaignVersion targetCampaignVersion = new CampaignVersion(targetVersion);
            return !(CampaignConstants.CurrentVersion < targetCampaignVersion);
#else
            return false;
#endif
        }
    }
}
