using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Labworks.Data
{
    public class LabworksSaving
    {
        private static readonly string SavePath = $"{MelonUtils.UserDataDirectory}/Labworks/LabworksSave.json";

        internal static SavePoint LoadedSavePoint { get; set; }
        internal static List<AmmoSave> LoadedAmmoSaves = new List<AmmoSave>();

        /// <summary>
        /// Saves the current loaded save data to file.
        /// </summary>
        internal static void SaveToDisk()
        {
            if (!System.IO.Directory.Exists($"{MelonUtils.UserDataDirectory}/Labworks"))
                System.IO.Directory.CreateDirectory($"{MelonUtils.UserDataDirectory}/Labworks");

            SaveData saveData = new SaveData
            {
                SavePoint = LoadedSavePoint,
                AmmoSaves = LoadedAmmoSaves
            };

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };

            string json = JsonConvert.SerializeObject(saveData, settings);

            System.IO.File.WriteAllText(SavePath, json);
        }

        /// <summary>
        /// Loads the save data from file. This should *probably* only be called at initialization.
        /// </summary>
        internal static void LoadFromDisk()
        {
            if (!System.IO.Directory.Exists($"{MelonUtils.UserDataDirectory}/Labworks"))
                System.IO.Directory.CreateDirectory($"{MelonUtils.UserDataDirectory}/Labworks");

            if (!System.IO.File.Exists(SavePath))
                return;

            string json = System.IO.File.ReadAllText(SavePath);

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };

            SaveData saveData = JsonConvert.DeserializeObject<SaveData>(json, settings);

            LoadedSavePoint = saveData.SavePoint;
            LoadedAmmoSaves = saveData.AmmoSaves;
        }

        public class SaveData
        {
            public SavePoint SavePoint { get; set; }
            public List<AmmoSave> AmmoSaves { get; set; }
        }

        public struct SavePoint(string levelBarcode, Vector3 position)
        {
            public string LevelBarcode = levelBarcode;
            public float PositionX = position.x;
            public float PositionY = position.y;
            public float PositionZ = position.z;
        }

        public struct AmmoSave
        {
            public string LevelBarcode { get; set; }
            public int LightAmmo { get; set; }
            public int MediumAmmo { get; set; }
            public int HeavyAmmo { get; set; }
        }
    }
}
