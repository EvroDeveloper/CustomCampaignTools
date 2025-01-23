using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoneLib;
using BoneLib.BoneMenu;
using BoneLib.BoneMenu.Elements;
using Il2CppSystem;
using MelonLoader;
using SLZ.Data;
using SLZ.Marrow.Data;
using SLZ.Player;
using UnityEngine;

namespace Labworks.Bonemenu
{
    public static class BoneMenuCreator
    {
        public static void CreateBoneMenu()
        {
            Page labworksCategory = Page.Root.CreatePage("Labworks", Color.yellow);

            InventoryMenu.CreateBoneMenu(labworksCategory);
            //BWOptionsMenu.CreateBoneMenu(labworksCategory);
        }
    }
}
