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

        public string[] AllLevels 
        {
            get {
                List<string> list = new List<string>();
                list.AddRange(mainLevels);
                list.AddRange(extraLevels);
                list.Add(MenuLevel);
                return list.ToArray();
            }
        }

        public static Campaign Session;
        public static string lastLoadedCampaignLevel;
        public static bool SessionActive { get => Session != null; }

        public CampaignSaveData saveData;

        public static List<Campaign> LoadedCampaigns = new List<Campaign>();

        public static Campaign RegisterCampaign(string Name, string initLevel, string[] mainLevels, string[] extraLevels, string loadScene, bool restrictDevTools, bool restrictAvatars, string defaultAvatar)
        {
            Campaign campaign = new Campaign();
            campaign.Name = Name;

            campaign.mainLevels = mainLevels;
            campaign.extraLevels = extraLevels;
            campaign.saveData = new CampaignSaveData(campaign);

            if(initLevel == string.Empty) campaign.MenuLevel = mainLevels[0];
            else campaign.MenuLevel = initLevel;

            if(loadScene == string.Empty) campaign.LoadScene = CommonBarcodes.Maps.LoadMod;
            else campaign.LoadScene = loadScene;

            campaign.RestrictAvatar = restrictAvatars;
            campaign.RestrictDevTools = restrictDevTools;

            if(restrictAvatars)
                campaign.CampaignAvatar = defaultAvatar;

            LoadedCampaigns.Add(campaign);

            CampaignBoneMenu.CreateCampaignPage(BoneMenuCreator.campaignCategory, campaign);

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

            return RegisterCampaign(campaignValueHolder.Name, campaignValueHolder.InitialLevel, campaignValueHolder.MainLevels.ToArray(), campaignValueHolder.ExtraLevels.ToArray(), campaignValueHolder.LoadScene, campaignValueHolder.RestrictDevTools, campaignValueHolder.RestrictAvatar, campaignValueHolder.CampaignAvatar);
        }

        public int GetLevelIndex(string levelBarcode, CampaignLevelType levelType = CampaignLevelType.MainLevel)
        {
            return Array.IndexOf(GetBarcodeArrayOfLevelType(levelType), levelBarcode);
        }

        public string GetLevelBarcodeByIndex(int index, CampaignLevelType levelType = CampaignLevelType.MainLevel)
        {
            return GetBarcodeArrayOfLevelType(levelType)[index];
        }

        private string[] GetBarcodeArrayOfLevelType(CampaignLevelType type)
        {
            string[] output = new string[0];
            switch (type)
            {
                case CampaignLevelType.Menu:
                    output = new string[] { MenuLevel };
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

        public static Campaign GetFromName(string name)
        {
            return LoadedCampaigns.FirstOrDefault(x => x.Name == name);
        }

        public static Campaign GetFromLevel(string barcode)
        {
            return LoadedCampaigns.FirstOrDefault(x => x.AllLevels.Contains(barcode));
        }

        public static Campaign GetFromLevel(Barcode barcode) => GetFromLevel(barcode.ID);

        public static Campaign GetFromLevel(LevelCrateReference level) => GetFromLevel(level.Barcode.ID);

        public static Campaign GetFromLevel() => GetFromLevel(SceneStreamer.Session.Level.Barcode.ID);

        public static bool IsCampaignLevel(string levelBarcode, out Campaign campaign)
        {
            campaign = GetFromLevel(levelBarcode);
            return campaign != null;
        }

        public static void OnInitialize()
        {
            Hooking.OnLevelLoaded += OnLevelLoaded;
            LoadCampaignsFromMods();
        }

        public static void LoadCampaignsFromMods()
        {
            string modsFolder = Path.Combine(Application.persistentDataPath, "Mods");
            string[] modPaths = Directory.GetDirectories(modsFolder);

            foreach(string mod in modPaths)
            {
                string[] jsonPaths = Directory.GetFiles(mod, "campaign.json");
                if(jsonPaths.Length == 0) continue;
                string jsonContent = File.ReadAllText(jsonPaths[0]);
                RegisterCampaignFromJson(jsonContent);
            }
        }

        public static void OnLevelLoaded(LevelInfo info)
        {
            if(!IsCampaignLevel(info.barcode, out Session)) return;

            if(Session.RestrictDevTools && !Session.saveData.DevToolsUnlocked)
            {
                MelonLogger.Msg("Restricting Dev Tools");

                var popUpMenu = UIRig.Instance.popUpMenu;
                popUpMenu.radialPageView.onActivated = new Action(() => {popUpMenu.RemoveDevMenu()});
                popUpMenu.RemoveDevMenu();
            }

            if(Session.RestrictAvatar && !Session.saveData.AvatarUnlocked)
            {
                PullCordDevice bodyLog = Player.PhysicsRig.GetComponentInChildren<PullCordDevice>(true);
                if(bodyLog != null)
                {
                    bodyLog.gameObject.SetActive(false);
                }

                var popUpMenu = UIRig.Instance.popUpMenu;
                popUpMenu.radialPageView.onActivated = new Action(() => {popUpMenu.RemoveAvatarsMenu()});

                if(Session.CampaignAvatar != string.Empty)
                {
                    Player.RigManager.SwapAvatarCrate(new Barcode(Session.CampaignAvatar));
                }
            }

            lastLoadedCampaignLevel = info.barcode;
        }
    }

    internal class CampaignLoadingData
    {
        public string Name { get; set; }
        public string InitialLevel { get; set; }
        public List<string> MainLevels { get; set; }
        public List<string> ExtraLevels { get; set; }
        public string LoadScene { get; set; }

        public bool RestrictDevTools { get; set; }
        public bool RestrictAvatar { get; set; }
        public string CampaignAvatar { get; set; }
    }
}