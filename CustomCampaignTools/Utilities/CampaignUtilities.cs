using CustomCampaignTools.Debug;
using CustomCampaignTools.GameSupport;
using Il2CppSLZ.Marrow.SceneStreaming;
using Il2CppSLZ.Marrow.Warehouse;
using System.Collections.Generic;
using System.Linq;

namespace CustomCampaignTools.Utilities;

public static class CampaignUtilities
{
    public static List<Campaign> LoadedCampaigns = [];

    private static Dictionary<string, Campaign> levelToCampaignRegistry = [];
    private static Dictionary<string, CampaignLevel> levelToCampaignLevel = [];

    internal static void AddCampaign(Campaign campaign)
    {
        CampaignLogger.Msg("Adding Campaign: " + campaign.Name);
        bool loaded = false;
        foreach (var c in LoadedCampaigns)
        {
            if (c.PalletBarcode == campaign.PalletBarcode)
            {
                CampaignLogger.Msg("Campaign Already Found, Replacing");
                LoadedCampaigns.Remove(c);
                c.saveData.SaveToDisk();
                LoadedCampaigns.Add(campaign);
                loaded = true;
                break;
            }
        }
        if (!loaded) 
        { 
            LoadedCampaigns.Add(campaign);
        }
        foreach (CampaignLevel c in campaign.AllLevels)
        {
            levelToCampaignRegistry[c.Barcode.ID] = campaign;
            levelToCampaignLevel[c.Barcode.ID] = c;
        }
        _menuCampaigns = null;
        GameManager.currentGameConfiguration.RefreshCampaignMenu(campaign);
    }

    public static List<Campaign> CampaignsToShowInMenu
    {
        get
        {
            _menuCampaigns ??= [.. LoadedCampaigns.Where(c => c.ShowInMenu)];
            return _menuCampaigns;
        }
    }
    private static List<Campaign> _menuCampaigns;


    public static Campaign GetFromPallet(Barcode pallet)
    {
        if (pallet == null) return null;
        return LoadedCampaigns.FirstOrDefault(x => x.PalletBarcode == pallet);
    }

    public static Campaign GetFromPallet(PalletReference pallet) => GetFromPallet(pallet.Barcode);

    public static bool TryGetCampaign(Barcode barcode, out Campaign campaign)
    {
        return levelToCampaignRegistry.TryGetValue(barcode.ID, out campaign);
    }

    public static bool TryGetCampaignLevel(Barcode barcode, out CampaignLevel campaignLevel)
    {
        return levelToCampaignLevel.TryGetValue(barcode.ID, out campaignLevel);
    }

    public static Campaign GetCampaign(Barcode barcode)
    {
        if (!levelToCampaignRegistry.ContainsKey(barcode.ID)) return null;
        return levelToCampaignRegistry[barcode.ID];
    }

    public static Campaign GetCampaign(LevelCrateReference level) => GetCampaign(level.Barcode);

    
    public static Campaign GetCampaign() => GetCampaign(SceneStreamer.Session.Level.Barcode);
}
