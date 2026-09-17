#if MELONLOADER
using Il2CppSLZ.Marrow.Warehouse;
using CustomCampaignTools.Debug;
#else
using SLZ.Marrow.Warehouse;
using CustomCampaignTools.SDK;
#endif
using UnityEngine;
using CustomCampaignTools.Utilities;

namespace CustomCampaignTools
{
    public struct AchievementData
    {
        public string Key { get; set; }
        public bool Hidden { get; set; }
        public byte[] IconBytes { get; set; }
        public string IconGUID { get; set; }
#if MELONLOADER
        private MarrowAssetT<Texture2D> _iconAsset;
        
        public MarrowAssetT<Texture2D> IconAsset
        {
            get
            {
                if(_iconAsset == null)
                {
                    _iconAsset = new MarrowAssetT<Texture2D>(IconGUID);
                }
                return _iconAsset;
            }
        }
#endif
        public string Name { get; set; }
        public string Description { get; set; }

#if MELONLOADER
        public Texture2D cachedTexture;
        public Sprite cachedSprite;

        public void Init()
        {
            LoadIcon();
        }

        public void LoadIcon()
        {
            if (IconBytes.Length == 0) return;

            var loadingTexture = new Texture2D(2, 2);

            if (IconBytes.Length != 0)
            {
                if (loadingTexture.LoadImage(IconBytes, false))
                {
                    OnIconLoaded(loadingTexture);
                }
                else
                {
                    CampaignLogger.Error("Failed to load texture from embedded resource.");
                    return;
                }
            }
            else if (IconGUID != "")
            {
                IconAsset.LoadAsset((Il2CppSystem.Action<Texture2D>)OnIconLoaded);
            }
            else
            {
                CampaignLogger.Error("No valid icon data found for achievement: " + Key);
                return;
            }
        }

        private void OnIconLoaded(Texture2D loadedTexture)
        {
            cachedTexture = loadedTexture;

            cachedTexture = cachedTexture.ProperResize(336, 336);
            cachedTexture.hideFlags = HideFlags.DontUnloadUnusedAsset;

            cachedSprite = Sprite.Create(cachedTexture, new Rect(0, 0, cachedTexture.width, cachedTexture.height), new Vector2(0.5f, 0.5f));
            cachedSprite.hideFlags = HideFlags.DontUnloadUnusedAsset;
        }
#endif
    }
}
