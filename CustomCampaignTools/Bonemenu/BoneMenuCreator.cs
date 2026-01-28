using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoneLib;
using BoneLib.BoneMenu;
using CustomCampaignTools.Data;
using CustomCampaignTools.Data.SavePoints;
using CustomCampaignTools.Data.SimpleSerializables;
using CustomCampaignTools.Debug;
using UnityEngine;

namespace CustomCampaignTools.Bonemenu
{
    public static class BoneMenuCreator
    {
        public static Page campaignCategory;

#if DEBUG
        public static Page testingPage;
        public static FullSavePoint lastSavedPoint;
#endif
        
        public static void CreateBoneMenu()
        {
            campaignCategory = Page.Root.CreatePage("Campaigns", Color.yellow);
#if DEBUG
            testingPage = Page.Root.CreatePage("Evro Testing", Color.red);
            testingPage.CreateFunction("Test Create Save", Color.red, () =>
            {
                lastSavedPoint = FullSavePoint.CreateSavePoint();

                CampaignLogger.Msg(SerializerUtils.SerializeObject(lastSavedPoint));
            });

            testingPage.CreateFunction("Test Load Save", Color.green, () =>
            {
                if(lastSavedPoint != null)
                {
                    lastSavedPoint.sceneEntityData.RestoreAllLevelEntities();
                }
            });
#endif
        }
    }
}
