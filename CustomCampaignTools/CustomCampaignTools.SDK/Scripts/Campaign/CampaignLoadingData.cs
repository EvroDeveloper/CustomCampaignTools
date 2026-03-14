using Il2CppSLZ.Marrow.Warehouse;
using Newtonsoft.Json;
using SimpleSerializables.Types;
using System.Reflection;

namespace CustomCampaignTools
{
    internal class CampaignLoadingData
    {
        public int Version { get; set; } = 1;
        public string Name { get; set; }
        public BarcodeSer PalletBarcode { get; set; }
        public SerializedLevelSetup IntroLevel { get; set; }
        public SerializedLevelSetup InitialLevel { get; set; }
        public List<SerializedLevelSetup> MainLevels { get; set; }
        public List<SerializedLevelSetup> ExtraLevels { get; set; }
        public ScannableRefSer<LevelCrateReference> LoadScene { get; set; }
        public ScannableRefSer<MonoDiscReference> LoadSceneMusic { get; set; }
        public bool UnlockableLevels { get; set; }
        public bool ShowInMenu { get; set; }
        public bool PrioritizeInLevelPanel { get; set; }
        public bool RestrictDevTools { get; set; }
        public AvatarRestrictionType AvatarRestrictionType { get; set; }
        public BarcodeSer CampaignAvatar { get; set; }
        public BarcodeSer BaseGameFallbackAvatar { get; set; }
        public List<BarcodeSer> WhitelistedAvatars { get; set; }
        public AvatarStatRanges AvatarStatRanges { get; set; }
        public bool SaveLevelWeapons { get; set; }
        public List<string> InventorySaveLimit { get; set; }
        public bool SaveLevelAmmo { get; set; }
        public bool UpdateSaveOnLevelEnter { get; set; }
        public ScannableRefSer<MonoDiscReference> AchievementUnlockSound { get; set; }
        public List<AchievementData> Achievements { get; set; }
        public bool LockInCampaign { get; set; }
        public List<BarcodeSer> CampaignUnlockCrates { get; set; }
        public List<BarcodeSer> HideCratesFromGachapon { get; set; }
        public string RigManagerOverride { get; set; }
        public string GameplayRigOverride { get; set; }
        public AssemblySer CampaignSupportAssembly { get; set; }
        public bool DevBuild { get; set; } = false;

        // i just made some BULLSHITTTTT
#if UNITY_EDITOR
        public static CampaignLoadingData CopyFrom(CampaignSettings campaignSettings)
        {
            return new CampaignLoadingData()
            {
                Name = campaignSettings.Name,
                IntroLevel = campaignSettings.IntroLevel.Serialize(),
                InitialLevel = campaignSettings.MainMenu.Serialize(),
                MainLevels = SerializedLevelSetup.CopyFrom(campaignSettings.MainLevels),
                ExtraLevels = SerializedLevelSetup.CopyFrom(campaignSettings.ExtraLevels),
                LoadScene = new BarcodeSer(campaignSettings.LoadScene.Barcode),
                LoadSceneMusic = new BarcodeSer(campaignSettings.LoadSceneMusic.Barcode),
                UnlockableLevels = campaignSettings.UnlockableLevels,
                ShowInMenu = campaignSettings.ShowCampaignInMenu,
                PrioritizeInLevelPanel = campaignSettings.PrioritizeInLevelPanel,
                RestrictDevTools = campaignSettings.RestrictDevTools,
                AvatarRestrictionType = campaignSettings.AvatarRestriction,
                WhitelistedAvatars = CampaignSettings.CrateArrayToBarcodes(campaignSettings.WhitelistedAvatars),
                CampaignAvatar = campaignSettings.CampaignAvatar.Barcode.ID,
                BaseGameFallbackAvatar = campaignSettings.BaseGameFallbackAvatar.Barcode.ID,
                SaveLevelWeapons = campaignSettings.SaveInventoryBetweenLevels,
                InventorySaveLimit = CampaignSettings.CrateArrayToBarcodes(campaignSettings.SaveInventoryFilter),
                SaveLevelAmmo = campaignSettings.SaveAmmoBetweenLevels,
                AchievementUnlockSound = campaignSettings.AchievementUnlockSound.Barcode.ID,
                Achievements = SerializableAchievement.ConvertToData(campaignSettings.Achievements),
                LockInCampaign = campaignSettings.LockPlayerInCampaign,
                CampaignUnlockCrates = CampaignSettings.CrateArrayToBarcodes(campaignSettings.CampaignUnlockCrates),
                HideCratesFromGachapon = CampaignSettings.CrateArrayToBarcodes(campaignSettings.HideCratesFromGachapon),
                DevBuild = campaignSettings.DevBuild,
            };
        }
#endif
    }

    public class SerializedLevelSetup
    {
        public ScannableRefSer<LevelCrateReference> levelBarcode;
        public string levelName;

        public bool IsValid()
        {
            return levelBarcode.IsValid();
        }
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
    }

    public class AssemblySer
    {
        public byte[] assemblyBytes;

        [JsonIgnore]
        private Assembly _assembly;

        public Assembly LoadAssembly()
        {
            if (!IsValid) return null;
            if(_assembly == null)
            {
                _assembly = Assembly.Load(assemblyBytes);
            }
            return _assembly;
        }

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
