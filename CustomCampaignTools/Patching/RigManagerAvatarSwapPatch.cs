using System.Collections.Generic;
using BoneLib;
using HarmonyLib;
using Il2CppCysharp.Threading.Tasks;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppSLZ.VRMK;
using MelonLoader;

namespace CustomCampaignTools.Patching;

[HarmonyPatch(typeof(RigManager))]
public static class RigManagerAvatarSwapPatch
{
    [HarmonyPatch(nameof(RigManager.EarlyUpdate))]
    [HarmonyPrefix]
    public static void EarlyUpdatePrefix(RigManager __instance)
    {
        if(!Campaign.SessionActive || !Campaign.Session.ShouldRestrictAvatar) return;
        if(__instance != Player.RigManager) return;
        if(!__instance._avatarDirty) return;

        if(!IsAvatarAllowed(__instance, __instance._avatarOnDeck))
        {
            __instance._avatarDirty = false;
            Campaign.Session.avatarRestrictor.OnFailedAvatarSwitch();

            if(!IsAvatarAllowed(__instance, __instance.avatar))
            {
                Campaign.Session.avatarRestrictor.ForceRigManagerAvatar(__instance);
            }
            return;
        }
    }

    public static bool IsAvatarAllowed(RigManager rigManager, Avatar avatar)
    {
        return Campaign.Session.avatarRestrictor.IsAvatarAllowed(rigManager._avatarOnDeck) && Campaign.Session.avatarRestrictor.IsAvatarAllowed(GetBarcodeFromAvatar(rigManager, rigManager._avatarOnDeck));
    }

    public static Barcode GetBarcodeFromAvatar(RigManager rigManager, Avatar avatar)
    {
        if(rigManager._avatarCache == null) return Barcode.EmptyBarcode();
        
        foreach(var barcodeAvatarPair in rigManager._avatarCache)
        {
            if(barcodeAvatarPair.Value == avatar)
            {
                return barcodeAvatarPair.Key;
            }
        }
        
        return Barcode.EmptyBarcode();
    }
}