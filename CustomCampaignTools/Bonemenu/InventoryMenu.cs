using BoneLib.BoneMenu;
using Il2CppSLZ.Marrow.Warehouse;
using MelonLoader;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomCampaignTools.Bonemenu
{
    internal class CampaignBoneMenu
    {
        public static Dictionary<Campaign, Page> campaignMenus = new();

        public static void CreateCampaignPage(Campaign c)
        {
            if (!campaignMenus.ContainsKey(c))
            {
                var campaignPage = BoneMenuCreator.campaignCategory.CreatePage(c.Name, Color.white);
                campaignMenus.Add(c, campaignPage);
            }

            RefreshCampaignPage(c);
        }

        public static void RefreshCampaignPage(Campaign c)
        {
            Page campaignPage = campaignMenus[c];
            campaignPage.RemoveAll();

            campaignPage.CreateFunction("Enter Campaign", Color.white, c.Enter);
            if (c.saveData.LoadedSavePoint.IsValid(out _))
                campaignPage.CreateFunction("Continue Campaign", Color.white, () => c.saveData.LoadedSavePoint.LoadContinue(c));
            campaignPage.CreateFunction("Reset Save", Color.red, c.saveData.ResetSave);

            if (c.Achievements != null)
            {
                var achievementPage = campaignPage.CreatePage($"{c.Name} Achievements", Color.yellow);
                var achievementSubPage = achievementPage;

                for (int i = 0; i < c.saveData.UnlockedAchievements.Count; i++)
                {
                    if (i % 10 == 0)
                    {
                        achievementSubPage = achievementPage.CreatePage($"Achievements {i + 1}-{Mathf.Min(i + 10, c.saveData.UnlockedAchievements.Count)}", Color.yellow);
                    }

                    string key = c.saveData.UnlockedAchievements[i];
                    try
                    {
                        achievementSubPage.CreateFunction(c.Achievements.First(a => a.Key == key).Name, Color.yellow, null);
                    }
                    catch
                    {
                        MelonLogger.Error($"Unlocked Achievement {key} could not be found in {c.Name}'s Achievements");
                    }
                }

                foreach (string key in c.saveData.UnlockedAchievements)
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

            if (c.DEVMODE)
            {
                var DebugPage = campaignPage.CreatePage("Debug", Color.red);

                DebugPage.CreateBool("Restrict Dev Tools", Color.white, c.RestrictDevTools, (b) => { c.RestrictDevTools = b; });
                DebugPage.CreateFunction("Unlock Avatar", Color.white, c.saveData.UnlockAvatar);
            }
        }
    }
}
