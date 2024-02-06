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
        public static void CreateBoneMenu(MenuCategory category)
        {
            var inventoryCategory = category.CreateCategory("Inventory", Color.white);
            inventoryCategory.CreateFunctionElement("Reset Ammo", Color.yellow, () => AmmoFunctions.ClearAmmo());
        }
    }
}
