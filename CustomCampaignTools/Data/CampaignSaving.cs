namespace CustomCampaignTools
{
    public class CampaignSaveData
    {
        public Campaign campaign;

        internal SavePoint LoadedSavePoint;
        internal List<AmmoSave> LoadedAmmoSaves = new List<AmmoSave>();

        public string SaveFolder { get => return $"{MelonUtils.UserDataDirectory}/Campaigns/{campaign.Name}"; }
        public string SavePath { get => return $"{SaveFolder}/save.json"; }

        public CampaignSaveData(Campaign c)
        {
            campaign = c;
            LoadFromDisk();
        }

        public bool DoesSavedAmmoExist(string levelBarcode)
        {
            if (LoadedAmmoSaves == null)
                return false;

            if (LoadedAmmoSaves.Count == 0)
                return false;

            if (LoadedAmmoSaves.Any(x => x.LevelBarcode == levelBarcode))
                return true;

            return false;
        }

        /// <summary>
        /// Saves the current loaded save data to file.
        /// </summary>
        internal void SaveToDisk()
        {
            if (!Directory.Exists(SaveFolder))
                Directory.CreateDirectory(SaveFolder);

            SaveData saveData = new SaveData
            {
                SavePoint = LoadedSavePoint,
                AmmoSaves = LoadedAmmoSaves,
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
        internal void LoadFromDisk()
        {
            if (!Directory.Exists(SaveFolder))
                Directory.CreateDirectory(SaveFolder);

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
        }

        public class SaveData
        {
            public SavePoint SavePoint { get; set; }
            public List<AmmoSave> AmmoSaves { get; set; }
        }

        public struct SavePoint(string levelBarcode, Vector3 position, string backSlotBarcode, string leftSidearmBarcode, string rightSidearmBarcode, string leftShoulderBarcode, string rightShoulderBarcode, List<string> boxContainedBarcodes, Vector3 boxContainerPosition)
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

            public List<string> BoxContainedBarcodes = boxContainedBarcodes;

            public float BoxContainedX = boxContainerPosition.x;
            public float BoxContainedY = boxContainerPosition.y;
            public float BoxContainedZ = boxContainerPosition.z;

            /// <summary>
            /// Returns true if the save point has a level barcode.
            /// </summary>
            /// <param name="hasSpawnPoint"></param>
            /// <returns></returns>
            public bool IsValid(out bool hasSpawnPoint)
            {
                if (new Vector3(PositionX, PositionY, PositionZ) == Vector3.zero)
                    hasSpawnPoint = false;
                else
                    hasSpawnPoint = true;

                if (savePoint.LevelBarcode == string.Empty)
                    return false;

                return true;
            }
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