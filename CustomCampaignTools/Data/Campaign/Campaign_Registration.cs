using System;
using System.IO;
using BoneLib.Notifications;
using CustomCampaignTools.Debug;
using CustomCampaignTools.Utilities;
using Il2CppSLZ.Marrow.Warehouse;
using Newtonsoft.Json;
using UnityEngine;

namespace CustomCampaignTools;

public partial class Campaign
{
    internal static Campaign RegisterCampaign(CampaignLoadingData data)
    {
        try
        {
            Campaign campaign = new(data);
            campaign.saveData = CampaignSaveData.LoadFromDisk(campaign);

            AssetWarehouse._onReady += new Action(campaign.LoadRequiredAssets);

            CampaignUtilities.AddCampaign(campaign);

            return campaign;
        }
        catch (Exception ex)
        {
            CampaignLogger.Error($"Failed to register campaign {data.Name}: {ex} {ex.StackTrace}");
            return null;
        }
    }

    internal static Campaign RegisterCampaignFromPallet(Pallet pallet)
    {
        if (!AssetWarehouse.Instance.TryGetPalletManifest(pallet.Barcode, out var manifest))
        {
            CampaignLogger.Error($"Failed to register campaign from pallet {pallet.Barcode}: Pallet manifest not found");
            return null;
        }

        string campaignJsonPath = Path.Combine(Path.GetDirectoryName(manifest.PalletPath), CampaignConstants.CampaignJsonFileName);
        if (!File.Exists(campaignJsonPath)) return null;
        CampaignLogger.Msg("Json FOUND for Pallet " + pallet.Title);

        string json = File.ReadAllText(campaignJsonPath);

        return RegisterCampaignFromJson(json, pallet);
    }

    internal static Campaign RegisterCampaignFromJson(string json, Pallet pallet)
    {
        var settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects
        };

        CampaignLoadingData campaignValueHolder = JsonConvert.DeserializeObject<CampaignLoadingData>(json, settings);
        if (campaignValueHolder == null)
        {
            CampaignLogger.Error($"Failed to deserialize campaign data for pallet {pallet.Title}");
            return null;
        }

        CampaignVersion deserializedVersion;
        
        if(CampaignVersion.IsValidVersionString(campaignValueHolder.Version))
            deserializedVersion = new CampaignVersion(campaignValueHolder.Version);
        else
            deserializedVersion = new CampaignVersion(0, 0, 0);
            
        if (deserializedVersion > CampaignConstants.CurrentVersion)
        {
            string campaignName = string.IsNullOrWhiteSpace(campaignValueHolder.Name) ? pallet.Title : campaignValueHolder.Name;

            CampaignLogger.Error($"Refusing to load campaign {campaignName}: campaign data version {deserializedVersion} is newer than supported version {CampaignConstants.CurrentVersion}");
            Notifier.Send(new Notification()
            {
                Title = "Campaign Tools Requires an Update",
                Message = $"{campaignName} was configured for a newer version of Custom Campaign Tools. Please update the code mod to load this campaign.",
                Type = NotificationType.Error,
                PopupLength = 4,
                ShowTitleOnPopup = true,
            });

            return null;
        }

        campaignValueHolder.PalletBarcode = new(pallet.Barcode);
        return RegisterCampaign(campaignValueHolder);
    }

    internal void LoadRequiredAssets()
    {
        if (LoadSceneMusicReference.TryGetDataCard(out var _loadMusicDatacard))
        {
            _loadMusicDatacard.AudioClip.LoadAsset(new Action<AudioClip>((a) =>
            {
                LoadSceneMusic = a;
                LoadSceneMusic.hideFlags = HideFlags.DontUnloadUnusedAsset;
            }));
        }
    }
}