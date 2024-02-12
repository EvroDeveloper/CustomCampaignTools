using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;

namespace Labworks.Data
{
    public class LabworksSaving
    {
        private static readonly string SavePath = $"{MelonUtils.UserDataDirectory}/Labworks/LabworksSave.json";

        internal static SavePoint LoadedSavePoint { get; set; }
        internal static List<AmmoSave> LoadedAmmoSaves = new List<AmmoSave>();
        internal static bool IsFordOnlyMode = false;
        internal static bool IsClassicNullBody = false;
        internal static bool IsClasssicCorruptedNullBody = false;
        internal static bool IsClassicNullRat = false;
        internal static bool IsClassicEarlyExit = false;
        internal static bool IsArmCollisionEnabled = false;

        /// <summary>
        /// Saves the current loaded save data to file.
        /// </summary>
        internal static void SaveToDisk()
        {
            if (!Directory.Exists($"{MelonUtils.UserDataDirectory}/Labworks"))
                Directory.CreateDirectory($"{MelonUtils.UserDataDirectory}/Labworks");

            SaveData saveData = new SaveData
            {
                SavePoint = LoadedSavePoint,
                AmmoSaves = LoadedAmmoSaves,
                IsFordOnlyMode = IsFordOnlyMode,
                IsClassicNullBody = IsClassicNullBody,
                IsClasssicCorruptedNullBody = IsClasssicCorruptedNullBody,
                IsClassicNullRat = IsClassicNullRat,
                IsClassicEarlyExit = IsClassicEarlyExit,
                IsArmCollisionEnabled = IsArmCollisionEnabled
            };

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };

            string json = JsonConvert.SerializeObject(saveData, settings);

            File.WriteAllText(SavePath, json);
        }

        /// <summary>
        /// Loads the save data from file. This should *probably* only be called at initialization.
        /// </summary>
        internal static void LoadFromDisk()
        {
            if (!Directory.Exists($"{MelonUtils.UserDataDirectory}/Labworks"))
                Directory.CreateDirectory($"{MelonUtils.UserDataDirectory}/Labworks");

            if (!File.Exists(SavePath))
                return;

            string json = File.ReadAllText(SavePath);

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };

            SaveData saveData = JsonConvert.DeserializeObject<SaveData>(json, settings);

            LoadedSavePoint = saveData.SavePoint;
            LoadedAmmoSaves = saveData.AmmoSaves;
            IsFordOnlyMode = saveData.IsFordOnlyMode;
            IsClassicNullBody = saveData.IsClassicNullBody;
            IsClasssicCorruptedNullBody = saveData.IsClasssicCorruptedNullBody;
            IsClassicNullRat = saveData.IsClassicNullRat;
            IsClassicEarlyExit = saveData.IsClassicEarlyExit;
            IsArmCollisionEnabled = saveData.IsArmCollisionEnabled;
        }

        public class SaveData
        {
            public SavePoint SavePoint { get; set; }
            public List<AmmoSave> AmmoSaves { get; set; }
            public bool IsFordOnlyMode { get; set; }
            public bool IsClassicNullBody { get; set; }
            public bool IsClasssicCorruptedNullBody { get; set; }
            public bool IsClassicNullRat { get; set; }
            public bool IsClassicEarlyExit { get; set; }
            public bool IsArmCollisionEnabled { get; set; }
        }

        public struct SavePoint(string levelBarcode, Vector3 position, string backSlotBarcode, string leftSidearmBarcode, string rightSidearmBarcode, string leftShoulderBarcode, string rightShoulderBarcode, List<string> boxContainedBarcodes)
        {
            public string LevelBarcode = levelBarcode;
            public float PositionX = position.x;
            public float PositionY = position.y;
            public float PositionZ = position.z;

            public string BackSlotBarcode = backSlotBarcode;
            public string LeftSidearmBarcode = leftSidearmBarcode;
            public string RightSidearmBarcode = rightSidearmBarcode;
            public string LeftShoulderSlotBarcode = leftShoulderBarcode;
            public string RightShoulderSlotBarcode = rightShoulderBarcode;
            //public string HeadSlotBarcode = headSlotBarcode;

            public List<string> BoxContainedBarcodes = boxContainedBarcodes;
        }

        public struct AmmoSave
        {
            public string LevelBarcode { get; set; }
            public int LightAmmo { get; set; }
            public int MediumAmmo { get; set; }
            public int HeavyAmmo { get; set; }

            public int GetCombinedTotal()
            {
                return (LightAmmo + MediumAmmo + HeavyAmmo);
            }
        }
    }
}
