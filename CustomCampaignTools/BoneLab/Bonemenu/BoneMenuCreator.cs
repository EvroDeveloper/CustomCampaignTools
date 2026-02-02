using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoneLib;
using BoneLib.BoneMenu;
using CustomCampaignTools.Data;
using CustomCampaignTools.Data.SimpleSerializables;
using CustomCampaignTools.Debug;
using UnityEngine;

namespace CustomCampaignTools.Bonemenu
{
    public static class BoneMenuCreator
    {
        public static Page campaignCategory;

        public static void CreateBoneMenu()
        {
            campaignCategory = Page.Root.CreatePage("Campaigns", Color.yellow);
        }
    }
}
