using BoneLib.BoneMenu;
using Il2CppSLZ.Marrow.Warehouse;
using MelonLoader;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomCampaignTools.BonelabSupport.Patches;

namespace CustomCampaignTools.BonelabSupport
{
    internal class CampaignBoneMenu
    {
        public static Dictionary<Campaign, Page> campaignMenus = [];

        public static void CreateOrRefreshCampaignPage(Campaign c)
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

                if(c.saveData.UnlockedAchievements.Count > 10)
                {
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
                }
                else
                {
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
            }

            if(c.avatarRestrictor == null || c.saveData.AvatarUnlocked)
            {
                campaignPage.CreateBool("Enable Bodylog", Color.white, c.saveData.ManualBodylogToggle, b => {
                    c.saveData.SetManualBodylogToggle(b);

                    if(Campaign.Session == c)
                        BodylogToggler.ForceSetBodylog(b);
                });
            }

            if (c.DEVMODE)
            {
                var DebugPage = campaignPage.CreatePage("Debug", Color.red);

                DebugPage.CreateBool("Restrict Dev Tools", Color.white, c.RestrictDevTools, (b) => { c.RestrictDevTools = b; });
                DebugPage.CreateFunction("Unlock Avatar", Color.white, c.saveData.UnlockAvatar);
                DebugPage.CreateFunction("Unlock All Levels", Color.white, () =>
                {
                    foreach (var level in c.AllLevels)
                    {
                        c.saveData.UnlockLevel(level.BarcodeString);
                    }
                });
            }
        }
    }
}
