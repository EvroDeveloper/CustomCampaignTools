#if MELONLOADER
using Il2CppSLZ.Marrow.Warehouse;
using CustomCampaignTools.Debug;
#else
using SLZ.Marrow.Warehouse;
using CustomCampaignTools.SDK;
#endif
using Newtonsoft.Json;
using SimpleSerializables.Types;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using CustomCampaignTools.Utilities;

namespace CustomCampaignTools
{
    internal class CampaignLoadingData
    {
        public int Version { get; set; } = 0; // Default version to 0 if Version doesnt exist in Json
#region 1.0.0
        public string Name { get; set; }
        public SerializedLevelSetup InitialLevel { get; set; }
        public List<SerializedLevelSetup> MainLevels { get; set; }
        public List<SerializedLevelSetup> ExtraLevels { get; set; }
        public LevelCrateRefSer LoadScene { get; set; }
        public MonoDiscRefSer LoadSceneMusic { get; set; }
        public bool UnlockableLevels { get; set; }
        public bool ShowInMenu { get; set; }
        public bool RestrictDevTools { get; set; }
        public AvatarRestrictionType AvatarRestrictionType { get; set; }
        public AvatarCrateRefSer CampaignAvatar { get; set; } = new AvatarCrateRefSer();
        public AvatarCrateRefSer BaseGameFallbackAvatar { get; set; } = new AvatarCrateRefSer();
        public List<AvatarCrateRefSer> WhitelistedAvatars { get; set; } = new List<AvatarCrateRefSer>();
        public bool SaveLevelWeapons { get; set; } = false;
        public List<SpawnableCrateRefSer> InventorySaveLimit { get; set; } = new List<SpawnableCrateRefSer>();
        public bool SaveLevelAmmo { get; set; } = true;
        public List<AchievementData> Achievements { get; set; } = new List<AchievementData>();
        public bool LockInCampaign { get; set; } = false;
        public List<SpawnableCrateRefSer> CampaignUnlockCrates { get; set; } = new List<SpawnableCrateRefSer>();
#endregion

#region 1.1.0
        public BarcodeSer PalletBarcode { get; set; } // Set by Campaign Registerer
        public SerializedLevelSetup IntroLevel { get; set; }
        public bool PrioritizeInLevelPanel { get; set; } = true;
        public MonoDiscRefSer AchievementUnlockSound { get; set; } = new MonoDiscRefSer();
        public List<SpawnableCrateRefSer> HideCratesFromGachapon { get; set; } = new List<SpawnableCrateRefSer>();
        public bool DevBuild { get; set; } = false;
#endregion

#region 1.2.0
        public AvatarStatRanges AvatarStatRanges { get; set; }
        public bool UpdateSaveOnLevelEnter { get; set; } = true;
        public SpawnableCrateRefSer RigManagerOverride { get; set; } = new SpawnableCrateRefSer();
        public SpawnableCrateRefSer GameplayRigOverride { get; set; } = new SpawnableCrateRefSer();
        public AssemblySer CampaignSupportAssembly { get; set; } = new AssemblySer();
#endregion

        // i just made some BULLSHITTTTT
#if UNITY_EDITOR
        public static CampaignLoadingData CopyFrom(CampaignSettings campaignSettings)
        {
            return new CampaignLoadingData()
            {
                Version = 2,
                Name = campaignSettings.Name,
                IntroLevel = campaignSettings.IntroLevel.Serialize(),
                InitialLevel = campaignSettings.MainMenu.Serialize(),
                MainLevels = SerializedLevelSetup.CopyFrom(campaignSettings.MainLevels),
                ExtraLevels = SerializedLevelSetup.CopyFrom(campaignSettings.ExtraLevels),
                LoadScene = new LevelCrateRefSer(campaignSettings.LoadScene.Barcode),
                LoadSceneMusic = new MonoDiscRefSer(campaignSettings.LoadSceneMusic.Barcode),
                UnlockableLevels = campaignSettings.UnlockableLevels,
                ShowInMenu = campaignSettings.ShowCampaignInMenu,
                PrioritizeInLevelPanel = campaignSettings.PrioritizeInLevelPanel,
                RestrictDevTools = campaignSettings.RestrictDevTools,
                AvatarRestrictionType = campaignSettings.AvatarRestriction,
                WhitelistedAvatars = campaignSettings.WhitelistedAvatars.Select(a => { return new AvatarCrateRefSer(a); }).ToList(),
                CampaignAvatar = new AvatarCrateRefSer(campaignSettings.CampaignAvatar),
                BaseGameFallbackAvatar = new AvatarCrateRefSer(campaignSettings.BaseGameFallbackAvatar),
                AvatarStatRanges = new AvatarStatRanges(campaignSettings.AvatarHeightRange, campaignSettings.AvatarMassRange, campaignSettings.AvatarArmRange),
                SaveLevelWeapons = campaignSettings.SaveInventoryBetweenLevels,
                InventorySaveLimit = campaignSettings.SaveInventoryFilter.Select(a => { return new SpawnableCrateRefSer(a); }).ToList(),
                SaveLevelAmmo = campaignSettings.SaveAmmoBetweenLevels,
                UpdateSaveOnLevelEnter = campaignSettings.UpdateSaveOnLevelEnter,
                AchievementUnlockSound = new MonoDiscRefSer(campaignSettings.AchievementUnlockSound),
                Achievements = SerializableAchievement.ConvertToData(campaignSettings.Achievements),
                LockInCampaign = campaignSettings.LockPlayerInCampaign,
                CampaignUnlockCrates = campaignSettings.CampaignUnlockCrates.Select(a => { return new SpawnableCrateRefSer(a); }).ToList(),
                HideCratesFromGachapon = campaignSettings.HideCratesFromGachapon.Select(a => { return new SpawnableCrateRefSer(a); }).ToList(),
                RigManagerOverride = new(campaignSettings.RigManagerOverride),
                GameplayRigOverride = new(campaignSettings.GameplayRigOverride),
                CampaignSupportAssembly = new(campaignSettings.CampaignSupportAssembly),
                DevBuild = campaignSettings.DevBuild,
            };
        }
#endif
    }

    public class SerializedLevelSetup
    {
        public LevelCrateRefSer levelBarcode;
        public string levelName;

        public bool IsValid()
        {
            return levelBarcode.IsValid();
        }

#if UNITY_EDITOR
        public static List<SerializedLevelSetup> CopyFrom(LevelSetup[] levelSetups)
        {
            List<SerializedLevelSetup> list = new();
            foreach (var level in levelSetups)
            {
                list.Add(level.Serialize());
            }
            return list;
        }
#endif
    }

    [Flags]
    public enum AvatarRestrictionType
    {
        None = 0,
        DisableBodyLog = 1,
        RestrictAvatar = 2,
        EnforceWhitelist = 4,
        EnforceStatRange = 8
    }

    public class AvatarStatRanges
    {
        public float heightRangeLow;
        public float heightRangeHigh;

        public float massRangeLow;
        public float massRangeHigh;
        
        public float armRangeLow;
        public float armRangeHigh;

        public AvatarStatRanges() { }

        public AvatarStatRanges(Vector2 heightRange, Vector2 massRange, Vector2 armRange)
        {
            heightRangeLow = heightRange.x;
            heightRangeHigh = heightRange.y;

            massRangeLow = massRange.x;
            massRangeHigh = massRange.y;

            armRangeLow = armRange.x;
            armRangeHigh = armRange.y;
        }
    }


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

    public class AssemblySer
    {
        public byte[] assemblyBytes;

        [JsonIgnore]
        private Assembly _assembly;

        public AssemblySer() { }

        public AssemblySer(TextAsset assemblyTextAsset)
        {
            assemblyBytes = assemblyTextAsset.bytes;
        }

#if MELONLOADER
        public Assembly LoadAssembly(Action<Assembly> callback)
        {
            if (!IsValid) return null;
            if(_assembly == null)
            {
                try
                {
                    _assembly = Assembly.Load(assemblyBytes);
                    callback.Invoke(_assembly);
                }
                catch (Exception ex)
                {
                    CampaignLogger.Error($"Unable to load Assembly: {ex}");
                }
            }
            return _assembly;
        }
#endif

        public bool IsValid
        {
            get
            {
                if (assemblyBytes == null || assemblyBytes.Length == 0) return false;
                return true;
            }
        }
    }
}
