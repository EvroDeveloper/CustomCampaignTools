using UnityEngine;
using MelonLoader;
using System;
using CustomCampaignTools;
using CustomCampaignTools.Utilities;

namespace CustomCampaignTools.SDK
{
    [RegisterTypeInIl2Cpp]
    public class CampaignReflection : MonoBehaviour
    {
        public CampaignReflection(IntPtr ptr) : base(ptr) { }

        public string GetName() => Campaign.Session.Name;

        public int GetAchievementsUnlocked() => Campaign.Session.saveData.UnlockedAchievements.Count;
        public int GetAchievementsTotal() => Campaign.Session.Achievements.Count;

        public int GetAmmoFromLevel(string barcode) => Campaign.Session.saveData.GetSavedAmmo(barcode).GetCombinedTotal();

        public bool GetSavePointValid() => Campaign.Session.saveData.LoadedSavePoint.IsValid();
        public string GetSavePointLevelBarcode() => Campaign.Session.saveData.LoadedSavePoint.LevelBarcode;
        public string GetSavePointLevelName () { }


    }
}