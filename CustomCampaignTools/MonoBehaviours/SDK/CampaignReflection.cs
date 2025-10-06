#if MELONLOADER
using MelonLoader;
using CustomCampaignTools;
using CustomCampaignTools.Utilities;
#endif
using UnityEngine;
using System;

namespace CustomCampaignTools.SDK
{
#if MELONLOADER
    [RegisterTypeInIl2Cpp]
#else
    [AddComponentMenu("CustomCampaignTools/UltEvent Utilities/Campaign Reflection")]
#endif
    public class CampaignReflection : MonoBehaviour
    {
#if MELONLOADER
        public CampaignReflection(IntPtr ptr) : base(ptr) { }
#endif
        public string GetName()
        {
#if MELONLOADER
            return Campaign.Session.Name;
#else
            return "";
#endif
        }

        public int GetAchievementsUnlocked()
        {
#if MELONLOADER
            return Campaign.Session.saveData.UnlockedAchievements.Count;
#else
            return 0;
#endif
        }

        public int GetAchievementsTotal()
        {
#if MELONLOADER
            return Campaign.Session.Achievements.Count;
#else
            return 0;
#endif
        }

        public int GetAmmoFromLevel(string barcode)
        {
#if MELONLOADER
            return Campaign.Session.saveData.GetSavedAmmo(barcode).GetCombinedTotal();
#else
            return 0;
#endif
        }

        public bool GetSavePointValid()
        {
#if MELONLOADER
            return Campaign.Session.saveData.LoadedSavePoint.IsValid();
#else
            return false;
#endif
        }
        public string GetSavePointLevelBarcode()
        {
#if MELONLOADER
            return Campaign.Session.saveData.LoadedSavePoint.LevelBarcode;
#else
            return "";
#endif
        }
        public string GetSavePointLevelName()
        {
        }


    }
}