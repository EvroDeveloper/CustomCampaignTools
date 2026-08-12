using BoneLib;
using HarmonyLib;
using Il2CppCysharp.Threading.Tasks;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.VRMK;
using MelonLoader;

namespace CustomCampaignTools.Patching;

[HarmonyPatch(typeof(RigManager._SwapAvatarCrate_d__66))]
public static class RigManagerAvatarSwapPatch
{
    [HarmonyPatch(nameof(RigManager._SwapAvatarCrate_d__66.MoveNext))]
    [HarmonyPrefix]
    public static bool MoveNextPostfix(RigManager._SwapAvatarCrate_d__66 __instance)
    {
        if (!Campaign.SessionActive || !Campaign.Session.ShouldRestrictAvatar || __instance.__4__this != Player.RigManager) return true; // Always allow avatar swap when not in campaign or avatar is unlocked

        var barcode = __instance.barcode;
        var thisAwaiter = __instance.__u__1;
        var spawnAvatarCrateAwaiter = __instance.__u__2;
        var avatarCache = __instance.__4__this._avatarCache;

        // Catch invalid avatar barcodes on the first MoveNext
        if(!Campaign.Session.avatarRestrictor.IsAvatarAllowed(barcode))
        {
            thisAwaiter.task = new UniTask<bool>(false);
            __instance.callback?.Invoke(false);
            Campaign.Session.avatarRestrictor.OnFailedAvatarSwitch();
            return false;
        }

        // Catch invalid avatars if they are already cached by the first MoveNext.
        if(avatarCache != null && avatarCache.ContainsKey(barcode))
        {
            MelonLogger.Msg("Checking Avatar Cache");
            if(avatarCache.TryGetValue(barcode, out var avatarToSwapTo) && !Campaign.Session.avatarRestrictor.IsAvatarAllowed(avatarToSwapTo))
            {
                thisAwaiter.task = new UniTask<bool>(false);
                __instance.callback?.Invoke(false);
                Campaign.Session.avatarRestrictor.OnFailedAvatarSwitch();
                return false;
            }
        }
#if false

        // Catch invalid avatars if they have to be spawned in.
        if (spawnAvatarCrateAwaiter != null && spawnAvatarCrateAwaiter.IsCompleted && spawnAvatarCrateAwaiter.task != null && spawnAvatarCrateAwaiter.task.result != null && !Campaign.Session.avatarRestrictor.IsAvatarAllowed(spawnAvatarCrateAwaiter.task.result.GetComponent<Avatar>()))
        {
            thisAwaiter.task = new UniTask<bool>(false);
            __instance.callback?.Invoke(false);
            Campaign.Session.avatarRestrictor.OnFailedAvatarSwitch();
            return false;
        }
#endif

        // Death
        if (__instance._avatarToSwapTo_5__2 != null && !Campaign.Session.avatarRestrictor.IsAvatarAllowed(__instance._avatarToSwapTo_5__2))
        {
            thisAwaiter.task = new UniTask<bool>(false);
            __instance.callback?.Invoke(false);
            Campaign.Session.avatarRestrictor.OnFailedAvatarSwitch();
            return false;
        }

        return true;
    }
}