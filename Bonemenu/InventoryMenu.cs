using BoneLib.BoneMenu;
using CustomCampaignTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomCampaignTools.Bonemenu
{
    internal class InventoryMenu
    {
        public static void CreateCampaignPage(Page category, Campaign c)
        {
            var inventoryCategory = category.CreatePage(c.Name, Color.white);
            inventoryCategory.CreateFunction("Reset Ammo", Color.yellow, () => c.saveData.ClearAmmoSave());
        }
    }
}
