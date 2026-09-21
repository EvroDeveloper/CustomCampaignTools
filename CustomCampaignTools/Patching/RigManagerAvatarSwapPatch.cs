using CustomCampaignTools.Utilities.Patching;
using BoneLib;
using HarmonyLib;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppSLZ.VRMK;
using CustomCampaignTools.AvatarRestriction;
using CustomCampaignTools.Debug;
using MelonLoader;
using Il2CppSLZ.Marrow.Pool;

namespace CustomCampaignTools.Patching;

public static class RigManagerAvatarSwapPatch
{
    [CampaignPatch(typeof(RigManager), nameof(RigManager.EarlyUpdate), CampaignPatchRunFlags.SessionActive)]
    [HarmonyPrefix]
    public static void SwitchAvatarPatch(RigManager __instance)
    {
        if(!Campaign.Session.ShouldRestrictAvatar) return;
        if(__instance != Player.RigManager) return;
        if(!__instance._avatarDirty) return;
        
        if(Campaign.Session.avatarRestrictor.IsAvatarAllowed(__instance._avatarOnDeck)) return;

        __instance._avatarDirty = false;
        Campaign.Session.avatarRestrictor.OnFailedAvatarSwitch();

        if(!Campaign.Session.avatarRestrictor.IsAvatarAllowed(__instance.avatar))
        {
            Campaign.Session.avatarRestrictor.ForceRigManagerAvatar(__instance);
        }
    }

    [CampaignPatch(typeof(RigManager), nameof(RigManager.Start), CampaignPatchRunFlags.SessionActive)]
    [HarmonyPostfix]
    public static void AddBarcodeCheck(RigManager __instance)
    {
        if(__instance != Player.RigManager) return;
        
        __instance.onAvatarSwapped2 += (System.Action<Barcode>)((b) =>
        {
            if(Campaign.Session.ShouldRestrictAvatar && !Campaign.Session.avatarRestrictor.IsAvatarAllowed(b) && !Campaign.Session.avatarRestrictor.IsAvatarAllowed(__instance.avatar))
                Campaign.Session.avatarRestrictor.ForceRigManagerAvatar(__instance);
        });
    }

    public static bool IsAvatarAllowed(RigManager rigManager, Avatar avatar)
    {
        Barcode barcode = GetBarcodeFromAvatarCache(rigManager, avatar);
        return Campaign.Session.avatarRestrictor.IsAvatarAllowed(rigManager._avatarOnDeck) && Campaign.Session.avatarRestrictor.IsAvatarAllowed(barcode);
    }

    public static Barcode GetBarcodeFromAvatarCache(RigManager rigManager, Avatar avatar)
    {
        if (rigManager._avatarCache != null)
        {
            foreach (var barcodeAvatarPair in rigManager._avatarCache)
            {
                if (barcodeAvatarPair.Value == avatar)
                {
                    return barcodeAvatarPair.Key;
                }
            }
            CampaignLogger.Msg($"Could not find the barcode for {avatar.gameObject.name} in the avatar cache. Odd");
        }
        return Barcode.EmptyBarcode();
    }
}