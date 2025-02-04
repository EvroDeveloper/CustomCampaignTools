using BoneLib.BoneMenu;
using CustomCampaignTools;
using Il2CppSLZ.Marrow.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomCampaignTools.Bonemenu
{
    internal class CampaignBoneMenu
    {
        public static void CreateCampaignPage(Page category, Campaign c)
        {
            var campaignPage = category.CreatePage(c.Name, Color.white);

            campaignPage.CreateFunction("Enter Campaign", Color.white, () => FadeLoader.Load(new Barcode(c.MenuLevel), new Barcode(c.LoadScene)));
            if(c.saveData.LoadedSavePoint.IsValid(out _))
                campaignPage.CreateFunction("Continue Campaign", Color.white, () => c.saveData.LoadedSavePoint.LoadContinue());
            campaignPage.CreateFunction("Reset Save", Color.red, () => c.saveData.ResetSave());

            var achievementPage = campaignPage.CreatePage($"{c.name} Achievements", Color.yellow);

            foreach(string key in c.saveData.UnlockedAchievements)
            {
                try
                {
                    achievementPage.CreateFunction(c.Achievements.First(a => a.Key == key).Name, Color.yellow, null);
                }
                catch
                {
                    MelonLogger.Error($"Unlocked Achievement {key} could not be found in {c.Name}'s Achievements");
                }
            }
        }
    }
}
