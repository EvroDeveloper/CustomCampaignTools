using BoneLib.BoneMenu;
using CustomCampaignTools;
using Il2CppSLZ.Marrow.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomCampaignTools.Bonemenu
{
    internal class CampaignBoneMenu
    {
        public static void CreateCampaignPage(Page category, Campaign c)
        {
            var inventoryCategory = category.CreatePage(c.Name, Color.white);

            inventoryCategory.CreateFunction("Enter Campaign", Color.white, () => FadeLoader.Load(new Barcode(c.MenuLevel), new Barcode(c.LoadScene)));
            if(c.saveData.LoadedSavePoint.IsValid(out _))
                inventoryCategory.CreateFunction("Continue Campaign", Color.white, () => c.saveData.LoadedSavePoint.LoadContinue(new Barcode(c.LoadScene)));
            inventoryCategory.CreateFunction("Reset Save", Color.red, () => c.saveData.ClearAmmoSave());
        }
    }
}
