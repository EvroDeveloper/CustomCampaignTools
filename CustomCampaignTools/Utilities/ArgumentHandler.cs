using CustomCampaignTools.Debug;
using Il2CppSLZ.Marrow.Warehouse;
using UnityEngine;

namespace CustomCampaignTools.Utilities;

public static class ArgumentHandler
{
    public static bool forcedCampaign = false;
    public static bool overrideUnlocks = false;
    public static Barcode campaignToLoad;

    public static void HandleArguments(string[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            string arg = args[i].ToLower();
            if(arg == "-customcampaigntools.forcedcampaign" && i+1 < args.Length)
            {
                campaignToLoad = new(args[i+1]);
                forcedCampaign = true;
                i++;
            }
            if (arg == "-customcampaigntools.overrideunlocks")
            {
                overrideUnlocks = true;
            }
        }
    }
}