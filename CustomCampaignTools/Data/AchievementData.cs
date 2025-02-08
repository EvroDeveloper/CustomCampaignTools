using UnityEngine;
using MelonLoader;

namespace CustomCampaignTools
{
    public class AchievementData
    {
        public string Key { get; set; }
        public bool Hidden { get; set; }
        public string IconGUID { get; set; }
        public byte[] IconBytes { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Texture2D cachedTexture;

        public void Init()
        {
            LoadIcon();
        }

        public void LoadIcon()
        {
            if(IconBytes.Length == 0) return;
            Texture2D texture = new Texture2D(2, 2);
            if (!texture.LoadImage(IconBytes))
            {
                MelonLogger.Error("Failed to load texture from embedded resource.");
            }
            else
            {
                cachedTexture = texture;
            }
        }
    }
}