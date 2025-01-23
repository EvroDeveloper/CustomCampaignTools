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
            var bwOptionsCategory = category.CreateCategory("Settings", Color.grey);

            var playerSettingsCategory = bwOptionsCategory.CreateCategory("Player Settings", Color.yellow);
            //var fordOnlyPanel = playerSettingsCategory.CreateSubPanel("Ford Only Explained", Color.yellow);
            //fordOnlyPanel.CreateFunction("Ford Only locks the character to Ford", Color.white, null);
            //fordOnlyPanel.CreateFunction("and disables the body log inside of", Color.white, null);
            //fordOnlyPanel.CreateFunction("Labworks levels.", Color.white, null);

            playerSettingsCategory.CreateBool("Ford ONLY", Color.red, LabworksSaving.IsFordOnlyMode, (option) =>
            {
                LabworksSaving.IsFordOnlyMode = option;

                if (SceneStreamer.Session.Level.Pallet.Title == "LabWorksBoneworksPort")
                {
                    var bodyLog = Player.physicsRig.transform.GetComponentInChildren<PullCordDevice>(true);
                    bodyLog.gameObject.SetActive(!LabworksSaving.IsFordOnlyMode);

                    if (!LabworksSaving.IsFordOnlyMode)
                    {
                        Player.rigManager.bodyVitals.bodyLogEnabled = true;
                        Player.rigManager.bodyVitals.PROPEGATE();
                        bodyLog.LoadFavoriteAvatars();
                    }

                    if (Player.rigManager.AvatarCrate.Barcode.ID != "SLZ.BONELAB.Content.Avatar.FordBW")
                        Player.rigManager.SwapAvatarCrate("SLZ.BONELAB.Content.Avatar.FordBW");
                }

                LabworksSaving.SaveToDisk();
            });

            var npcSettingsCategory = bwOptionsCategory.CreateCategory("NPC Settings", Color.green);

            npcSettingsCategory.CreateBoolElement("Enable Original NullBody", Color.cyan, LabworksSaving.IsClassicNullBody, (option) =>
            {
                LabworksSaving.IsClassicNullBody = option;
                LabworksSaving.SaveToDisk();
            });
            npcSettingsCategory.CreateBoolElement("Enable Original Corrupted NullBody", Color.cyan, LabworksSaving.IsClasssicCorruptedNullBody, (option) =>
            {
                LabworksSaving.IsClasssicCorruptedNullBody = option;
                LabworksSaving.SaveToDisk();
            });
            npcSettingsCategory.CreateBoolElement("Enable Original NullRat", Color.cyan, LabworksSaving.IsClassicNullRat, (option) =>
            {
                LabworksSaving.IsClassicNullRat = option;
                LabworksSaving.SaveToDisk();
            });
            npcSettingsCategory.CreateBoolElement("Enable Original EarlyExit", Color.cyan, LabworksSaving.IsClassicEarlyExit, (option) =>
            {
                LabworksSaving.IsClassicEarlyExit = option;
                LabworksSaving.SaveToDisk();
            });
        }
    }
}
