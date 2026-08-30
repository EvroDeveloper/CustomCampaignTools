using System;
using System.Collections;
using System.Collections.Generic;
using BoneLib;
using CustomCampaignTools.Debug;
using HarmonyLib;
using Il2CppSLZ.Marrow.AI;
using Il2CppSLZ.Marrow.Audio;
using Il2CppSLZ.Marrow.Interaction;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppSLZ.Marrow.Zones;
using MelonLoader;
using UnityEngine;

namespace CustomCampaignTools;

/// <summary>
/// Runs stock Encounter/SpawnGroup data without calling SpawnGroup.SpawnAsync.
/// SpawnGroup retains its normal AI bookkeeping, cleanup, and events.
/// </summary>
public sealed class EncounterManager
{
    private const float SpawnResultTimeout = 30f;

    private sealed class GroupRuntime
    {
        public readonly SpawnGroup Group;
        public bool InitialDelayComplete;
        public bool Complete;
        public object Coroutine;

        public GroupRuntime(SpawnGroup group) => Group = group;
    }

    private static readonly Dictionary<IntPtr, EncounterManager> ActiveEncounters = new();

    private readonly Encounter _encounter;
    private readonly List<GroupRuntime> _groups = new();
    private TriggerRefProxy _playerProxy;
    private object _encounterCoroutine;
    private bool _running;
    private bool _stopping;

    private IntPtr Key => _encounter.Pointer;

    private EncounterManager(Encounter encounter, MarrowEntity player)
    {
        _encounter = encounter;
        SetPlayer(player);

        if (encounter.spawnGroups == null)
            return;

        foreach (SpawnGroup group in encounter.spawnGroups)
        {
            if (group != null)
                _groups.Add(new GroupRuntime(group));
        }
    }

    public static void StartEncounter(Encounter encounter, MarrowEntity player = null)
    {
        if (encounter == null)
            return;

        if (ActiveEncounters.TryGetValue(encounter.Pointer, out EncounterManager existing))
        {
            existing.Resume(player);
            return;
        }

        if (encounter._complete)
            return;

        EncounterManager manager = new(encounter, player);
        ActiveEncounters[encounter.Pointer] = manager;
        manager.Begin();
    }

    public static void PauseEncounter(Encounter encounter)
    {
        if (TryGetManager(encounter, out EncounterManager manager))
            manager.Pause();
        else if (encounter != null)
            encounter._isEncounterActive = false;
    }

    public static void ForceStopAndReset(Encounter encounter, bool killAll)
    {
        if (TryGetManager(encounter, out EncounterManager manager))
            manager.Reset(killAll);
        else
            ResetUnmanagedEncounter(encounter, killAll);
    }

    public static void ForceStopAndComplete(Encounter encounter, bool killAll)
    {
        if (TryGetManager(encounter, out EncounterManager manager))
            manager.ForceComplete(killAll);
        else
            CompleteUnmanagedEncounter(encounter, killAll);
    }

    public static bool RunOriginalAwardGroupCompletion(Encounter encounter)
    {
        if (!TryGetManager(encounter, out EncounterManager manager))
            return true;

        manager._encounter.completeCount = Math.Min(
            manager._encounter.completeCount + 1,
            manager._groups.Count);
        return false;
    }

    public static void OnEncounterCompleted(Encounter encounter)
    {
        if (!TryGetManager(encounter, out EncounterManager manager))
            return;

        manager._running = false;
        manager.StopCoroutines();
        ActiveEncounters.Remove(manager.Key);
    }

    public static void OnEncounterDestroyed(Encounter encounter)
    {
        if (!TryGetManager(encounter, out EncounterManager manager))
            return;

        manager._running = false;
        manager.StopCoroutines();
        ActiveEncounters.Remove(manager.Key);
    }

    private static bool TryGetManager(Encounter encounter, out EncounterManager manager)
    {
        manager = null;
        return encounter != null && ActiveEncounters.TryGetValue(encounter.Pointer, out manager);
    }

    private void Begin()
    {
        _encounter.completeCount = 0;
        _encounter._complete = false;
        _encounter._isEncounterActive = true;

        foreach (GroupRuntime runtime in _groups)
        {
            SpawnGroup group = runtime.Group;
            group.Setup();
            group.ResetVariables();
            group.playerProxy = _playerProxy;
            runtime.InitialDelayComplete = false;
            runtime.Complete = group.isComplete;
        }

        if (_encounter.encounterMusic != null && Audio2dPlugin.Audio2dManager != null)
        {
            Audio2dPlugin.Audio2dManager.CueOverrideMusic(
                _encounter.encounterMusic,
                _encounter.volume,
                _encounter.fadeInTime,
                _encounter.fadeOutTime,
                _encounter.loop);
        }

        StartScheduler();
    }

    private void Resume(MarrowEntity player)
    {
        SetPlayer(player);
        if (_running || _encounter._complete)
            return;

        foreach (GroupRuntime runtime in _groups)
            runtime.Group.playerProxy = _playerProxy;

        _encounter._isEncounterActive = true;
        StartScheduler();
    }

    private void SetPlayer(MarrowEntity player)
    {
        if (player != null)
        {
            _encounter.playerEntity = player;
            _playerProxy = player.GetComponentInChildren<TriggerRefProxy>();
        }
        else if (_playerProxy == null && Player.RigManager != null)
        {
            _playerProxy = Player.RigManager.GetComponentInChildren<TriggerRefProxy>();
        }
    }

    private void StartScheduler()
    {
        _running = true;
        _stopping = false;
        _encounterCoroutine = MelonCoroutines.Start(CoRunEncounter());
    }

    private IEnumerator CoRunEncounter()
    {
        if (_groups.Count == 0)
        {
            CompleteEncounterIfNecessary();
            yield break;
        }

        if (_encounter.spawnOrder == Encounter.SpawnOrder.PARALLEL)
        {
            foreach (GroupRuntime runtime in _groups)
            {
                if (!runtime.Complete)
                    runtime.Coroutine = MelonCoroutines.Start(CoRunGroup(runtime));
            }

            while (_running && HasIncompleteGroups())
                yield return null;
        }
        else
        {
            foreach (GroupRuntime runtime in _groups)
            {
                if (!_running)
                    yield break;
                if (runtime.Complete)
                    continue;

                runtime.Coroutine = MelonCoroutines.Start(CoRunGroup(runtime));
                while (_running && !runtime.Complete)
                    yield return null;
            }
        }

        if (_running)
            CompleteEncounterIfNecessary();
    }

    private IEnumerator CoRunGroup(GroupRuntime runtime)
    {
        SpawnGroup group = runtime.Group;

        if (!runtime.InitialDelayComplete)
        {
            if (group.initialGroupDelay > 0f)
                yield return new WaitForSeconds(group.initialGroupDelay);
            if (!_running)
                yield break;
            runtime.InitialDelayComplete = true;
        }

        while (_running && group.IsUnderTotalSpawned())
        {
            if (group.IsUnderMaxAlive())
            {
                int previousSpawnCount = group.SpawnCount;
                if (TrySpawn(group))
                {
                    float timeoutAt = Time.realtimeSinceStartup + SpawnResultTimeout;
                    bool timeoutLogged = false;
                    while (_running && group.SpawnCount <= previousSpawnCount)
                    {
                        if (!timeoutLogged && Time.realtimeSinceStartup >= timeoutAt)
                        {
                            CampaignLogger.Error($"Encounter '{_encounter.name}' timed out waiting for a spawn in group {IndexOf(group)}.");
                            timeoutLogged = true;
                        }
                        yield return null;
                    }
                }
            }

            if (!_running)
                yield break;

            yield return group.spawnInterval > 0f
                ? new WaitForSeconds(group.spawnInterval)
                : null;
        }

        while (_running && !group.IsAllDead())
            yield return null;

        if (!_running)
            yield break;

        group.WaitAndDespawnAllDead();
        while (_running && group._deadBrains != null && group._deadBrains.Count > 0)
            yield return null;

        if (!_running)
            yield break;

        runtime.Complete = true;
        runtime.Coroutine = null;
        if (!group.isComplete)
            group.CompleteGroup();
    }

    private bool TrySpawn(SpawnGroup group)
    {
        if (group.spawners == null || group.spawners.Length == 0)
            return false;

        int spawnerCount = group.spawners.Length;
        int startIndex = NormalizeIndex(group._spawnerIndex, spawnerCount);

        for (int offset = 0; offset < spawnerCount; offset++)
        {
            int spawnerIndex = (startIndex + offset) % spawnerCount;
            group._spawnerIndex = (spawnerIndex + 1) % spawnerCount;

            CrateSpawner spawner = group.spawners[spawnerIndex];
            if (spawner == null || !CanSpawn(spawner, group.useSpawnerToggle))
                continue;

            ConfigureSpawner(group, spawner, spawnerIndex);
            spawner.SpawnSpawnable();
            return true;
        }

        return false;
    }

    private static bool CanSpawn(CrateSpawner spawner, bool useSpawnerToggle)
    {
        if (useSpawnerToggle && spawner.TryGetComponent(out SpawnerToggle toggle) && !toggle.shouldSpawn)
        {
            return false;
        }

        return spawner.shouldSpawn == null || spawner.shouldSpawn.Invoke();
    }

    private static void ConfigureSpawner(SpawnGroup group, CrateSpawner spawner, int spawnerIndex)
    {
        if (group.encounterProfile != null
            && group.encounterProfile.npcProfileList != null
            && group.SpawnCount < group.encounterProfile.npcProfileList.Count)
        {
            NPCProfile profile = group.encounterProfile.npcProfileList[group.SpawnCount];
            if (profile != null && profile.spawnable != null && profile.spawnable.IsValid())
            {
                spawner.spawnableCrateReference = profile.spawnable.crateRef;
                spawner.policyData = profile.spawnable.policyData;

                if (group.aiSettings != null
                    && spawnerIndex < group.aiSettings.Length
                    && group.aiSettings[spawnerIndex] != null)
                {
                    group.aiSettings[spawnerIndex].overrideConfig = profile.baseConfig;
                }
                return;
            }
        }

        if (group._crateRandLookup != null
            && group._crateRandLookup.TryGetValue(spawner, out RandomizeCrate randomizer)
            && randomizer != null)
        {
            SpawnableCrateReference randomCrate = randomizer.SelectRandomCrate();
            if (randomCrate != null)
                spawner.spawnableCrateReference = randomCrate;
        }
    }

    private void Pause()
    {
        if (!_running)
            return;

        _running = false;
        _encounter._isEncounterActive = false;
        StopCoroutines();
    }

    private void Reset(bool killAll)
    {
        if (_stopping)
            return;

        _stopping = true;
        _running = false;
        StopCoroutines();

        foreach (GroupRuntime runtime in _groups)
        {
            if (killAll)
                runtime.Group.KillAll();
            runtime.Group.Cleanup();
            runtime.Group.ResetVariables();
        }

        ResetEncounterFields(_encounter);
        if (Audio2dPlugin.Audio2dManager != null)
            Audio2dPlugin.Audio2dManager.StopOverrideMusic();
        _encounter.OnEncounterReset?.Invoke();
        ActiveEncounters.Remove(Key);
    }

    private void ForceComplete(bool killAll)
    {
        if (_stopping)
            return;

        _stopping = true;
        _running = false;
        StopCoroutines();

        foreach (GroupRuntime runtime in _groups)
        {
            if (killAll)
                runtime.Group.KillAll();
            runtime.Complete = true;
            if (!runtime.Group.isComplete)
                runtime.Group.CompleteGroup();
        }

        CompleteEncounterIfNecessary();
        ActiveEncounters.Remove(Key);
    }

    private void CompleteEncounterIfNecessary()
    {
        if (!_encounter._complete)
            _encounter.CompleteEncounter();
    }

    private bool HasIncompleteGroups()
    {
        foreach (GroupRuntime runtime in _groups)
        {
            if (!runtime.Complete)
                return true;
        }
        return false;
    }

    private void StopCoroutines()
    {
        if (_encounterCoroutine != null)
        {
            MelonCoroutines.Stop(_encounterCoroutine);
            _encounterCoroutine = null;
        }

        foreach (GroupRuntime runtime in _groups)
        {
            if (runtime.Coroutine == null)
                continue;
            MelonCoroutines.Stop(runtime.Coroutine);
            runtime.Coroutine = null;
        }
    }

    private int IndexOf(SpawnGroup group)
    {
        for (int i = 0; i < _groups.Count; i++)
        {
            if (_groups[i].Group == group)
                return i;
        }
        return -1;
    }

    private static int NormalizeIndex(int index, int count)
    {
        int normalized = index % count;
        return normalized < 0 ? normalized + count : normalized;
    }

    private static void ResetEncounterFields(Encounter encounter)
    {
        encounter.completeCount = 0;
        encounter._complete = false;
        encounter._isEncounterActive = false;
        encounter.playerEntity = null;
    }

    private static void ResetUnmanagedEncounter(Encounter encounter, bool killAll)
    {
        if (encounter == null)
            return;

        if (encounter.spawnGroups != null)
        {
            foreach (SpawnGroup group in encounter.spawnGroups)
            {
                if (group == null)
                    continue;
                if (killAll)
                    group.KillAll();
                group.Cleanup();
                group.ResetVariables();
            }
        }

        ResetEncounterFields(encounter);
        if (Audio2dPlugin.Audio2dManager != null)
            Audio2dPlugin.Audio2dManager.StopOverrideMusic();
        encounter.OnEncounterReset?.Invoke();
    }

    private static void CompleteUnmanagedEncounter(Encounter encounter, bool killAll)
    {
        if (encounter == null)
            return;

        if (encounter.spawnGroups != null)
        {
            foreach (SpawnGroup group in encounter.spawnGroups)
            {
                if (group == null)
                    continue;
                if (killAll)
                    group.KillAll();
                if (!group.isComplete)
                    group.CompleteGroup();
            }
        }

        if (!encounter._complete)
            encounter.CompleteEncounter();
    }
}

[HarmonyPatch(typeof(Encounter))]
internal static class EncounterPatches
{
    [HarmonyPatch(nameof(Encounter.StartEncounter), new Type[] { })]
    [HarmonyPrefix]
    private static bool StartEncounterPrefix(Encounter __instance)
    {
        EncounterManager.StartEncounter(__instance);
        return false;
    }

    [HarmonyPatch(nameof(Encounter.StartEncounter), new[] { typeof(MarrowEntity) })]
    [HarmonyPrefix]
    private static bool StartEncounterWithActivatorPrefix(Encounter __instance, MarrowEntity activatorEntity)
    {
        EncounterManager.StartEncounter(__instance, activatorEntity);
        return false;
    }

    [HarmonyPatch(nameof(Encounter.PauseEncounter))]
    [HarmonyPrefix]
    private static bool PauseEncounterPrefix(Encounter __instance)
    {
        EncounterManager.PauseEncounter(__instance);
        return false;
    }

    [HarmonyPatch(nameof(Encounter.ForceStopAndReset))]
    [HarmonyPrefix]
    private static bool ForceStopAndResetPrefix(Encounter __instance, bool killAll)
    {
        EncounterManager.ForceStopAndReset(__instance, killAll);
        return false;
    }

    [HarmonyPatch(nameof(Encounter.ForceStopAndComplete))]
    [HarmonyPrefix]
    private static bool ForceStopAndCompletePrefix(Encounter __instance, bool killAll)
    {
        EncounterManager.ForceStopAndComplete(__instance, killAll);
        return false;
    }

    [HarmonyPatch(nameof(Encounter.AwardGroupCompletion))]
    [HarmonyPrefix]
    private static bool AwardGroupCompletionPrefix(Encounter __instance) =>
        EncounterManager.RunOriginalAwardGroupCompletion(__instance);

    [HarmonyPatch(nameof(Encounter.CompleteEncounter))]
    [HarmonyPostfix]
    private static void CompleteEncounterPostfix(Encounter __instance) =>
        EncounterManager.OnEncounterCompleted(__instance);

    [HarmonyPatch(nameof(Encounter.OnDestroy))]
    [HarmonyPrefix]
    private static void OnDestroyPrefix(Encounter __instance) =>
        EncounterManager.OnEncounterDestroyed(__instance);
}
