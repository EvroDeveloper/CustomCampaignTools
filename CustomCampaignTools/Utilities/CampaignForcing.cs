using Il2CppSLZ.Marrow.Warehouse;

namespace CustomCampaignTools.Utilities;

public static class CampaignForcing
{
    public static bool forcedCampaign = false;
    public static Barcode campaignToLoad;
    public static bool overrideUnlocks = false;

    [CampaignArgument("-customcampaigntools.forcedcampaign", extraArgs: 1)]
    public static void OnForcedCampaign(string[] args)
    {
        campaignToLoad = new(args[0]);
        forcedCampaign = true;
    }

    [CampaignArgument("-customcampaigntools.overrideunlocks")]
    public static void OnOverrideUnlocks()
    {
        overrideUnlocks = true;
    }
}