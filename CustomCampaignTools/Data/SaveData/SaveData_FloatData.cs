namespace CustomCampaignTools
{
    public partial class CampaignSaveData
    {
        internal List<FloatDataPair> FloatData = [];
        
        public void SetValue(string key, float value)
        {
            GetFloatDataEntry(key).Value = value;
            SaveToDisk();
        }
        public float GetValue(string key)
        {
            return GetFloatDataEntry(key).Value;
        }
        private FloatDataPair GetFloatDataEntry(string key)
        {
            FloatDataPair found = null;
            try
            {
                found = FloatData.First(f => f.Key == key);
            }
            catch
            {
                found = new FloatDataPair(key);
                FloatData.Add(found);
            }
            return found;
        }
    }
}