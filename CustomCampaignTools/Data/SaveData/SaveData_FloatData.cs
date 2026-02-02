namespace CustomCampaignTools
{
    // yeah nobody using ts :sob:
    public partial class CampaignSaveData
    {
        internal Dictionary<string, float> FloatData = [];
        
        public void SetValue(string key, float value)
        {
            if(!FloatData.ContainsKey(key))
                FloatData.Add(key, value);
            else
                FloatData[key] = value;
            
            SaveToDisk();
        }
        
        public float GetValue(string key)
        {
            if(!FloatData.ContainsKey(key))
                FloatData.Add(key, 0f);
            
            return FloatData[key];
        }
    }
}