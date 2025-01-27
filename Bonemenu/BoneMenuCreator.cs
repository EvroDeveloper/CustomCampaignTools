using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoneLib;
using BoneLib.BoneMenu;
using UnityEngine;

namespace CustomCampaignTools.Bonemenu
{
    public static class BoneMenuCreator
    {
        public static void CreateBoneMenu()
        {
            Page labworksCategory = Page.Root.CreatePage("Campaigns", Color.yellow);
        }
    }
}
