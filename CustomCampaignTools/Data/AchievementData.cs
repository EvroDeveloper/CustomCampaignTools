using UnityEngine;
using MelonLoader;
using System.IO;

namespace CustomCampaignTools
{
    public class AchievementData
    {
        public string Key { get; set; }
        public bool Hidden { get; set; }
        public byte[] IconBytes { get; set; }
        public string IconGUID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Texture2D cachedTexture;
        public Sprite cachedSprite;

        public void Init()
        {
            LoadIcon();
        }

        public void LoadIcon()
        {
            if (IconBytes.Length == 0) return;

            cachedTexture = new Texture2D(2, 2);

            if (IconBytes.Length != 0)
            {
                if (!cachedTexture.LoadImage(IconBytes, false))
                {
                    MelonLogger.Error("Failed to load texture from embedded resource.");
                    return;
                }
            }
            else if (IconGUID)
            {
                byte[] fileData = File.ReadAllBytes(IconGUID);
                if (!cachedTexture.LoadImage(fileData, false))
                {
                    MelonLogger.Error("Failed to load texture from file: " + IconGUID);
                    return;
                }
            }
            else
            {
                MelonLogger.Error("No valid icon data found for achievement: " + Key);
                return;
            }

            

            cachedTexture = cachedTexture.ProperResize(336, 336);
            cachedTexture.hideFlags = HideFlags.DontUnloadUnusedAsset;

            cachedSprite = Sprite.Create(cachedTexture, new Rect(0, 0, cachedTexture.width, cachedTexture.height), new Vector2(0.5f, 0.5f));
            cachedSprite.hideFlags = HideFlags.DontUnloadUnusedAsset;
        }
    }
}