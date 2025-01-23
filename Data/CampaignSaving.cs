namespace CustomCampaignTools
{
    public class CampaignSaveData
    {
        public Campaign campaign;

        internal List<AmmoSave> LoadedAmmoSaves = new List<AmmoSave>();

        public string SavePath { get => return $"{MelonUtils.UserDataDirectory}/Campaigns/{campaign.Name}/save.json"; }

        public CampaignSaveData(Campaign c)
        {
            campaign = c;

        }

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
        }
    }
}