using BoneLib.BoneMenu;
using Il2CppSLZ.Marrow.Warehouse;
using MelonLoader;
using System.Linq;
using UnityEngine;

namespace CustomCampaignTools.Bonemenu
{
    internal class CampaignBoneMenu
    {
        public static Dictionary<Campaign, Page> campaignMenus = new();

        public static void CreateCampaignPage(Page category, Campaign c)
        {
            var campaignPage = category.CreatePage(c.Name, Color.white);

            campaignMenus.Add(c, campaignPage);

            RefreshCampaignPage(c);

#if false
            var DebugPage = campaignPage.CreatePage("Debug", Color.red);

            DebugPage.CreateBool("Restrict Dev Tools", Color.white, c.RestrictDevTools, (b) => { c.RestrictDevTools = b; });
            DebugPage.CreateFunction("Unlock Avatar", Color.white, c.saveData.UnlockAvatar);
#endif
        }

        public static void RefreshCampaignPage(Campaign c)
        {
            Page page = campaignMenus[c];
            page.RemoveAll();

            campaignPage.CreateFunction("Enter Campaign", Color.white, c.Enter);
            if(c.saveData.LoadedSavePoint.IsValid(out _))
                campaignPage.CreateFunction("Continue Campaign", Color.white, () => c.saveData.LoadedSavePoint.LoadContinue(c));
            campaignPage.CreateFunction("Reset Save", Color.red, c.saveData.ResetSave);

            if(c.Achievements != null)
            {
                var achievementPage = campaignPage.CreatePage($"{c.Name} Achievements", Color.yellow);  

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
}
