using Il2CppSLZ.Marrow.SceneStreaming;
using Il2CppSLZ.Marrow.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using BoneLib;
using System.IO;
using UnityEngine;
using MelonLoader;
using Il2CppSLZ.Bonelab;
using CustomCampaignTools.Bonemenu;
using CustomCampaignTools.SDK;
using BoneLib.Notifications;
using static UnityEngine.UI.StencilMaterial;
using System.Reflection;
using Il2CppSLZ.Marrow.VoidLogic;
using System.Security.Policy;

namespace CustomCampaignTools
{
    public enum CampaignLevelType
    {
        None,
        Menu,
        MainLevel,
        ExtraLevel // Extra levels do not save ammo
    }

    public class Campaign
    {
        public string Name;
        public string PalletBarcode;
        public string MenuLevel;
        public string[] mainLevels;
        public string[] extraLevels;
        public string LoadScene;

        public bool RestrictDevTools;
        public bool RestrictAvatar;
        public string CampaignAvatar;

        public bool SaveLevelWeapons;
        public bool SaveLevelAmmo;
        public List<AchievementData> Achievements;

        public string[] AllLevels 
        {
            get {
                List<string> list = [.. mainLevels, .. extraLevels, MenuLevel];
                return list.ToArray();
            }
        }
        
        public CampaignSaveData saveData;

        public static Campaign Session;
        public static string lastLoadedCampaignLevel;
        public static bool SessionActive { get => Session != null; }


        internal static Campaign RegisterCampaign(CampaignLoadingData data)
        {
            Campaign campaign = new Campaign();
            try
            {
                campaign.Name = data.Name;

                campaign.mainLevels = data.MainLevels.ToArray();
                campaign.extraLevels = data.ExtraLevels.ToArray();
                campaign.saveData = new CampaignSaveData(campaign);

                if (data.InitialLevel == string.Empty) campaign.MenuLevel = data.MainLevels[0];
                else campaign.MenuLevel = data.InitialLevel;

                if (data.LoadScene == string.Empty) campaign.LoadScene = CommonBarcodes.Maps.LoadMod;
                else campaign.LoadScene = data.LoadScene;

                campaign.RestrictAvatar = data.RestrictAvatar;
                campaign.RestrictDevTools = data.RestrictDevTools;

                if (data.RestrictAvatar)
                    campaign.CampaignAvatar = data.CampaignAvatar;

                campaign.SaveLevelWeapons = data.SaveLevelWeapons;
                campaign.SaveLevelAmmo = data.SaveLevelAmmo;
                campaign.Achievements = data.Achievements;

                if(campaign.Achievements != null)
                {
                    foreach (AchievementData ach in campaign.Achievements)
                    {
                        ach.Init();
                    }
                }

                CampaignUtilities.LoadedCampaigns.Add(campaign);

                CampaignBoneMenu.CreateCampaignPage(BoneMenuCreator.campaignCategory, campaign);

            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Failed to register campaign {data.Name}: {ex.Message}");
            }

            return campaign;
        }

        public static Campaign RegisterCampaignFromJson(string json)
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };

            CampaignLoadingData campaignValueHolder = JsonConvert.DeserializeObject<CampaignLoadingData>(json, settings);

            return RegisterCampaign(campaignValueHolder);
        }

        public int GetLevelIndex(string levelBarcode, CampaignLevelType levelType = CampaignLevelType.MainLevel)
        {
            MelonLogger.Msg($"Index of {levelBarcode} is {Array.IndexOf(GetBarcodeArrayOfLevelType(levelType), levelBarcode)}");
            return Array.IndexOf(GetBarcodeArrayOfLevelType(levelType), levelBarcode);
        }

        public string GetLevelBarcodeByIndex(int index, CampaignLevelType levelType = CampaignLevelType.MainLevel)
        {
            return GetBarcodeArrayOfLevelType(levelType)[index];
        }

        private string[] GetBarcodeArrayOfLevelType(CampaignLevelType type)
        {
            string[] output = [];
            switch (type)
            {
                case CampaignLevelType.Menu:
                    output = [MenuLevel];
                    break;
                case CampaignLevelType.MainLevel:
                    output = mainLevels;
                    break;
                case CampaignLevelType.ExtraLevel:
                    output = extraLevels;
                    break;
                default:
                    output = AllLevels;
                    break;
            }
            return output;
        }

        public CampaignLevelType TypeOfLevel(string barcode)
        {
            if (MenuLevel == barcode) return CampaignLevelType.Menu;
            else if (mainLevels.Contains(barcode)) return CampaignLevelType.MainLevel;
            else return CampaignLevelType.ExtraLevel;
        }

        public static void OnInitialize()
        {
            Hooking.OnLevelLoaded += OnLevelLoaded;
            try
            {
                LoadCampaignsFromMods();
            }
            catch (Exception ex)
            {
                MelonLogger.Error("Coudnt load the FUCKING campaigns from the FUCKING MODS FOLDER: " + ex.Message);
            }
        }

        public static void LoadCampaignsFromMods()
        {
            string modsFolder = Path.Combine(Application.persistentDataPath, "Mods");

            foreach (MelonMod registeredMelon in MelonMod.RegisteredMelons)
            {
                if (registeredMelon.Info.Name == "RedirectModsFolder")
                {
                    modsFolder = GetRedirectModsFolder(modsFolder);
                    break;
                }
            }
            string[] modPaths = Directory.GetDirectories(modsFolder);

            foreach (string mod in modPaths)
            {
                string[] jsonPaths = Directory.GetFiles(mod, "campaign.json");

                if (jsonPaths.Length != 0)
                {
                    string jsonContent = File.ReadAllText(jsonPaths[0]);
                    RegisterCampaignFromJson(jsonContent);
                    continue;
                }

                string[] jsonPaths2 = Directory.GetFiles(mod, "campaign.json.bundle");

                if (jsonPaths2.Length != 0)
                {
                    string jsonContent2 = File.ReadAllText(jsonPaths2[0]);
                    RegisterCampaignFromJson(jsonContent2);
                }
                
            }
        }

        public static string GetRedirectModsFolder(string normalModsFolder)
        {
            Type type = typeof(RedirectModsFolder.Core);

            FieldInfo fieldInfo = type.GetField("pathEntry", BindingFlags.NonPublic | BindingFlags.Static);
            MelonPreferences_Entry<string> fieldValue = (MelonPreferences_Entry<string>)(fieldInfo?.GetValue(null));

            if (fieldValue.Value != "default" && fieldValue.Value != string.Empty)
                return fieldValue.Value;

            return normalModsFolder;
        }

        public static void OnLevelLoaded(LevelInfo info)
        {
            if(!CampaignUtilities.IsCampaignLevel(info.barcode, out Session, out _)) return;

            lastLoadedCampaignLevel = info.barcode;

            if (Session.RestrictDevTools && !Session.saveData.DevToolsUnlocked)
            {
                var popUpMenu = UIRig.Instance.popUpMenu;
                popUpMenu.radialPageView.onActivated += (Il2CppSystem.Action<PageView>)((p) => {
                    popUpMenu.radialPageView.buttons[5].m_Data.m_Callback = (Il2CppSystem.Action)(() => { Notifier.Send(new Notification { Title = Session.Name, Message = $"{Session.Name} does not allow dev tools until campaign is complete." }); });
                });
            }

            if(Session.RestrictAvatar && !Session.saveData.AvatarUnlocked)
            {
                PullCordDevice bodyLog = Player.PhysicsRig.GetComponentInChildren<PullCordDevice>(true);
                if(bodyLog != null)
                {
                    bodyLog.gameObject.SetActive(false);
                }

                var popUpMenu = UIRig.Instance.popUpMenu;
                popUpMenu.radialPageView.buttons[7].m_Data.m_Callback = (Il2CppSystem.Action)(() => { Notifier.Send(new Notification { Title = Session.Name, Message = $"{Session.Name} does not allow custom avatars until campaign is complete." }); });

                if (Session.CampaignAvatar != string.Empty)
                {
                    Player.RigManager.SwapAvatarCrate(new Barcode(Session.CampaignAvatar));
                }
            }
        }

        public static void OnUIRigCreated()
        {
            var popUpMenu = UIRig.Instance.popUpMenu;

            if (Session.RestrictDevTools && !Session.saveData.DevToolsUnlocked)
            {
                // popUpMenu.radialPageView.onActivated += (Il2CppSystem.Action<PageView>)((p) => {
                //     popUpMenu.radialPageView.buttons[5].m_Data.m_Callback = (Il2CppSystem.Action)(() => { Notifier.Send(new Notification { Title = Session.Name, Message = $"{Session.Name} does not allow dev tools until campaign is complete." }); });
                // });

                popUpMenu.radialPageView.buttons[5].m_Data.m_Callback = (Il2CppSystem.Action)(() => { Notifier.Send(new Notification { Title = Session.Name, Message = $"{Session.Name} does not allow dev tools until campaign is complete." }); });

            }

            if(Session.RestrictAvatar && !Session.saveData.AvatarUnlocked)
            {
                popUpMenu.radialPageView.buttons[7].m_Data.m_Callback = (Il2CppSystem.Action)(() => { Notifier.Send(new Notification { Title = Session.Name, Message = $"{Session.Name} does not allow custom avatars until campaign is complete." }); });

                if (Session.CampaignAvatar != string.Empty)
                {
                    Player.RigManager.SwapAvatarCrate(new Barcode(Session.CampaignAvatar));
                }
            }
        }
    }

    internal class CampaignLoadingData
    {
        public string Name { get; set; }
        public string InitialLevel { get; set; }
        public List<string> MainLevels { get; set; }
        public List<string> ExtraLevels { get; set; }
        public string LoadScene { get; set; }
        public bool ShowInMenu { get; set; }
        public bool RestrictDevTools { get; set; }
        public bool RestrictAvatar { get; set; }
        public string CampaignAvatar { get; set; }
        public bool SaveLevelWeapons { get; set; }
        public bool SaveLevelAmmo { get; set; }
        public List<AchievementData> Achievements { get; set; }
    }

    public class AchievementData
    {
        public string Key { get; set; }
        public bool Hidden { get; set; }
        public string IconGUID { get; set; }
        public byte[] IconBytes { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        private Texture2D _cachedTexture;

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
                Debug.LogError("Failed to load texture from embedded resource.");
            }
        }
    }
}