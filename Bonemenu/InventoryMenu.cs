using BoneLib.BoneMenu.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Labworks.Bonemenu
{
    internal class InventoryMenu
    {
        public static void CreateBoneMenu(Page category)
        {
            var inventoryCategory = category.CreatePage("Inventory", Color.white);
            inventoryCategory.CreateFunction("Reset Ammo", Color.yellow, () => AmmoFunctions.ClearAmmo());
        }
    }
}
