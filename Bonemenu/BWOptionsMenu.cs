using BoneLib.BoneMenu.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Labworks.Data;
using BoneLib;
using SLZ.Props;
using SLZ.Marrow.SceneStreaming;

namespace Labworks.Bonemenu
{
    internal class BWOptionsMenu
    {
        public static void CreateBoneMenu(MenuCategory category)
        {
            var bwOptionsCategory = category.CreateCategory("BW Options", Color.grey);

            var fordOnlyPanel = bwOptionsCategory.CreateSubPanel("Ford Only Explained", Color.yellow);
            fordOnlyPanel.CreateFunctionElement("Ford Only locks the character to Ford", Color.white, null);
            fordOnlyPanel.CreateFunctionElement("and disables the body log inside of", Color.white, null);
            fordOnlyPanel.CreateFunctionElement("Labworks levels.", Color.white, null);

            bwOptionsCategory.CreateBoolElement("Ford ONLY", Color.red, LabworksSaving.IsFordOnlyMode, (option) =>
            {
                LabworksSaving.IsFordOnlyMode = option;

                if (SceneStreamer.Session.Level.Pallet.Title == "LabWorksBoneworksPort")
                {
                    Player.physicsRig.transform.GetComponentInChildren<PullCordDevice>(true).gameObject.SetActive(!LabworksSaving.IsFordOnlyMode);
                    if (Player.rigManager.AvatarCrate.Barcode.ID != "SLZ.BONELAB.Content.Avatar.FordBW")
                        Player.rigManager.SwapAvatarCrate("SLZ.BONELAB.Content.Avatar.FordBW");
                }

                LabworksSaving.SaveToDisk();
            });
        }
    }
}
