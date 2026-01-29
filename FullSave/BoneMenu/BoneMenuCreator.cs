using System;
using BoneLib;
using BoneLib.BoneMenu;
using FullSave.Data;
using FullSave.Utilities;
using UnityEngine;

namespace FullSave.BoneMenu;

public static class BoneMenuCreator
{
    public static Page testingPage;

    public static Page saveStackPage;


    public static void CreateBoneMenu()
    {
        testingPage = Page.Root.CreatePage("FullSave", Color.green);
        

        testingPage.CreateFunction("Quick Save", Color.white, () =>
        {
            SaveStack.SaveGame();
        });

        testingPage.CreateFunction("Quick Load", Color.green, () =>
        {
            SaveStack.QuickLoad();
        });

        saveStackPage = testingPage.CreatePage("Saves", Color.white);
        PopulateSaveStackPage();
    }

    public static void PopulateSaveStackPage()
    {
        saveStackPage.RemoveAll();
        foreach(FullSavePoint fullSavePoint in SaveStack.fullSavesList)
        {
            saveStackPage.CreateFunction($"{fullSavePoint.levelName} {fullSavePoint.creationDate:G}", Color.white, fullSavePoint.LoadSave);
        }
    }
}
