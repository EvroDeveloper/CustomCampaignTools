using Il2CppSLZ.Marrow;
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
            if(MarrowSettings.RuntimeInstance.DefaultPlayerRig != _defaultPlayerRig) MarrowSettings.RuntimeInstance._defaultPlayerRig = _defaultPlayerRig;
            if(MarrowSettings.RuntimeInstance.UIEventSystem != _defaultGameplayRig)  MarrowSettings.RuntimeInstance._uiEventSystem = _defaultGameplayRig;
            return;
        }

        MarrowSettings.RuntimeInstance._defaultPlayerRig = destination.RigManagerOverride.TryGetCrate(out _) ? Campaign.Session.RigManagerOverride : _defaultPlayerRig;
        MarrowSettings.RuntimeInstance._uiEventSystem = destination.GameplayRigOverride.TryGetCrate(out _) ? Campaign.Session.GameplayRigOverride : _defaultGameplayRig;
    }
}