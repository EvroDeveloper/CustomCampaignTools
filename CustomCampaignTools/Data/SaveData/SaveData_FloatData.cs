namespace CustomCampaignTools
{
    public partial class CampaignSaveData
    {
        internal List<FloatData> LoadedFloatDatas = [];
        
        public void SetValue(string key, float value)
        {
            GetFloatDataEntry(key).Value = value;
            SaveToDisk();
        }
        public float GetValue(string key)
        {
            return GetFloatDataEntry(key).Value;
        }
        private FloatData GetFloatDataEntry(string key)
        {
            FloatData found = null;
            try
            {
                found = LoadedFloatDatas.First(f => f.Key == key);
            }
            catch
            {
                found = new FloatData(key);
                LoadedFloatDatas.Add(found);
            }
            return found;
        }
    }
}