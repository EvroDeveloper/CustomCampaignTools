#if MELONLOADER
using MelonLoader;
using CustomCampaignTools;
using Il2CppSLZ.Marrow.Utilities;
using Il2CppSLZ.Marrow.Warehouse;
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
            return Campaign.Session.saveData.GetSavedAmmo(new Barcode(barcode)).Total;
#else
            return 0;
#endif
        }

        public bool GetSavePointValid()
        {
#if MELONLOADER
            return Campaign.Session.saveData.TryGetSavePoint(out _);
#else
            return false;
#endif
        }
        public string GetSavePointLevelBarcode()
        {
#if MELONLOADER
            if(Campaign.Session.saveData.TryGetSavePoint(out var savepoint))
                return savepoint.LevelBarcode;
            else
                return "";
#else
            return string.Empty;
#endif
        }
        public string GetSavePointLevelName()
        {
#if MELONLOADER
            if(Campaign.Session.saveData.TryGetSavePoint(out var savepoint) && Campaign.Session.TryGetLevel(savepoint.LevelBarcode, out var level))
                return level.Title;
            else
                return "";
#else
            return string.Empty;
#endif
        }


    }
}
