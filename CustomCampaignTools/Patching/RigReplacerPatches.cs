using BoneLib;
using CustomCampaignTools.SDK;
using HarmonyLib;
using Il2CppSLZ.Marrow.SceneStreaming;
using Il2CppSLZ.Marrow.Utilities;
using Il2CppSLZ.Marrow.Warehouse;

namespace CustomCampaignTools.Patching
{
    [HarmonyPatch(typeof(StreamSession))]
    public static class RigReplacerPatches
    {
        [HarmonyPatch(nameof(StreamSession.RegisterPlayerMarker))]
        [HarmonyPrefix]
        public static bool OnPlayerMarkerRegistered(StreamSession __instance, PlayerMarker playerMarker)
        {
            SpawnableCrateReference rigManSpawn = MarrowGame.marrowSettings.DefaultPlayerRig;
            SpawnableCrateReference gameplayRigSpawn = MarrowGame.marrowSettings.UIEventSystem;

            bool shouldCancelOriginalCall = false;

            if(Campaign.SessionActive)
            {
                if(Campaign.Session.RigManagerOverride.TryGetCrate(out _))
                {
                    rigManSpawn = Campaign.Session.RigManagerOverride;
                    shouldCancelOriginalCall = true;
                }
                if(Campaign.Session.GameplayRigOverride.TryGetCrate(out _))
                {
                    gameplayRigSpawn = Campaign.Session.GameplayRigOverride;
                    shouldCancelOriginalCall = true;
                }
            }
    
            if(playerMarker.TryGetComponent<CampaignPlayerMarkerOverride>(out var overrider))
            {
                if(overrider.RigManagerOverride.TryGetCrate(out _))
                {
                    rigManSpawn = Campaign.Session.RigManagerOverride;
                    shouldCancelOriginalCall = true;
                }
                if(overrider.GameplayRigOverride.TryGetCrate(out _))
                {
                    gameplayRigSpawn = Campaign.Session.GameplayRigOverride;
                    shouldCancelOriginalCall = true;
                }
            }

            if(shouldCancelOriginalCall)
            {
                HelperMethods.SpawnCrate(
                    rigManSpawn, 
                    playerMarker.transform.position, 
                    playerMarker.transform.rotation,
                    spawnAction: playerMarker.OnPlayerSpawned, 
                    despawnAction: playerMarker.OnPlayerDespawn);
                
                HelperMethods.SpawnCrate(
                    gameplayRigSpawn,
                    playerMarker.transform.position,
                    playerMarker.transform.rotation
                );
            }

            return !shouldCancelOriginalCall;
        }
    }
}
