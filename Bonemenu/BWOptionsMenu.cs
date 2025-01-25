
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BoneLib;
using Il2CppSLZ.Props;
using Il2CppSLZ.Marrow.SceneStreaming;
using BoneLib.BoneMenu;
using Il2CppSLZ.Marrow;

namespace Labworks.Bonemenu
{
    internal class BWOptionsMenu
    {
        public static void CreateBoneMenu(Page category)
        {
            var bwOptionsCategory = category.CreatePage("Settings", Color.grey);

            var playerSettingsCategory = bwOptionsCategory.CreatePage("Player Settings", Color.yellow);
            //var fordOnlyPanel = playerSettingsCategory.CreateSubPanel("Ford Only Explained", Color.yellow);
            //fordOnlyPanel.CreateFunction("Ford Only locks the character to Ford", Color.white, null);
            //fordOnlyPanel.CreateFunction("and disables the body log inside of", Color.white, null);
            //fordOnlyPanel.CreateFunction("Labworks levels.", Color.white, null);

            //playerSettingsCategory.CreateBool("Ford ONLY", Color.red, LabworksSaving.IsFordOnlyMode, (option) =>
            //{
            //    LabworksSaving.IsFordOnlyMode = option;

            //    if (SceneStreamer.Session.Level.Pallet.Title == "LabWorksBoneworksPort")
            //    {
            //        var bodyLog = Player.PhysicsRig.transform.GetComponentInChildren<Grip>(true);
            //        bodyLog.gameObject.SetActive(!LabworksSaving.IsFordOnlyMode);

            //        if (!LabworksSaving.IsFordOnlyMode)
            //        {
            //            Player.RigManager.bodyVitals.bodyLogEnabled = true;
            //            Player.RigManager.bodyVitals.PROPEGATE();
            //            bodyLog.LoadFavoriteAvatars();
            //        }

            //        if (Player.rigManager.AvatarCrate.Barcode.ID != "SLZ.BONELAB.Content.Avatar.FordBW")
            //            Player.rigManager.SwapAvatarCrate("SLZ.BONELAB.Content.Avatar.FordBW");
            //    }

            //    LabworksSaving.SaveToDisk();
            //});

            //var npcSettingsCategory = bwOptionsCategory.CreateCategory("NPC Settings", Color.green);

            //npcSettingsCategory.CreateBoolElement("Enable Original NullBody", Color.cyan, LabworksSaving.IsClassicNullBody, (option) =>
            //{
            //    LabworksSaving.IsClassicNullBody = option;
            //    LabworksSaving.SaveToDisk();
            //});
            //npcSettingsCategory.CreateBoolElement("Enable Original Corrupted NullBody", Color.cyan, LabworksSaving.IsClasssicCorruptedNullBody, (option) =>
            //{
            //    LabworksSaving.IsClasssicCorruptedNullBody = option;
            //    LabworksSaving.SaveToDisk();
            //});
            //npcSettingsCategory.CreateBoolElement("Enable Original NullRat", Color.cyan, LabworksSaving.IsClassicNullRat, (option) =>
            //{
            //    LabworksSaving.IsClassicNullRat = option;
            //    LabworksSaving.SaveToDisk();
            //});
            //npcSettingsCategory.CreateBoolElement("Enable Original EarlyExit", Color.cyan, LabworksSaving.IsClassicEarlyExit, (option) =>
            //{
            //    LabworksSaving.IsClassicEarlyExit = option;
            //    LabworksSaving.SaveToDisk();
            //});
        }
    }
}
