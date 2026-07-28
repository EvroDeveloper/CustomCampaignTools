using Il2CppSLZ.Marrow.Utilities;
using Il2CppSLZ.Marrow.Warehouse;

namespace CustomCampaignTools.Utilities;

internal static class RigReplacementUtility
{
    private static SpawnableCrateReference _defaultPlayerRig;
    private static SpawnableCrateReference _defaultGameplayRig;
    private static bool _initialized;

    internal static void Initialize()
    {
        _defaultPlayerRig = MarrowGame.marrowSettings.DefaultPlayerRig;
        _defaultGameplayRig = MarrowGame.marrowSettings.UIEventSystem;
        _initialized = true;
    }

    // scary field overrides
    internal static void OnLevelLoadStart(Campaign destination = null)
    {
        if(!_initialized) Initialize();
        
        if(destination == null)
        {
            if(MarrowGame.marrowSettings.DefaultPlayerRig != _defaultPlayerRig) MarrowGame.marrowSettings._defaultPlayerRig = _defaultPlayerRig;
            if(MarrowGame.marrowSettings.UIEventSystem != _defaultGameplayRig) MarrowGame.marrowSettings._uiEventSystem = _defaultGameplayRig;
            return;
        }

        MarrowGame.marrowSettings._defaultPlayerRig = destination.RigManagerOverride.TryGetCrate(out _) ? Campaign.Session.RigManagerOverride : _defaultPlayerRig;
        MarrowGame.marrowSettings._uiEventSystem = destination.GameplayRigOverride.TryGetCrate(out _) ? Campaign.Session.GameplayRigOverride : _defaultGameplayRig;
    }
}