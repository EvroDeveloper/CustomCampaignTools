using System;
using FullSave.BoneMenu;
using FullSave.Data;
using Il2CppSLZ.Bonelab.SaveData;
using UnityEngine;

namespace FullSave.Utilities;

public static class SaveStack
{
    public static List<FullSavePoint> fullSavesList = [];

    public static string SaveFolder { get => Path.Combine(Application.persistentDataPath, "Saves"); }
    private static string GetSavePath(FullSavePoint savePoint) => $"{SaveFolder}/slot_FullSave_{savePoint.creationDate:s}.save.json";

    public static void LoadSavesFromFiles()
    {
        fullSavesList.Clear();

        List<string> allSavePaths = [.. Directory.GetFiles(SaveFolder).Where((s) => s.Contains("slot_FullSave_"))];
        allSavePaths.Sort(); // urhh i think it sorts from oldest to newest?

        foreach(string savePath in allSavePaths)
        {
            var savePoint = SerializerUtils.LoadObjectFromFile<FullSavePoint>(savePath);
            fullSavesList.Insert(0, savePoint);
        }
    }

    public static FullSavePoint SaveGame()
    {
        FullSavePoint savePoint = FullSavePoint.CreateSavePoint();

        fullSavesList.Insert(0, savePoint);
        BoneMenuCreator.PopulateSaveStackPage();

        SerializerUtils.SaveObjectToFile(savePoint, GetSavePath(savePoint));

        return savePoint;
    }

    public static void QuickLoad()
    {
        Load(0);
    }

    public static void Load(int index)
    {
        if(index >= fullSavesList.Count) return;
        fullSavesList[index].LoadSave();
    }
}
