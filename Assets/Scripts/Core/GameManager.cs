// GameManager.cs  — full, self-contained
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Central controller: loads / saves the game state, applies need decay,
/// and exposes an API for UI to modify needs.
/// </summary>
public class GameManager : MonoBehaviour
{
    /* ────────────  INSPECTOR  ──────────── */
    [Header("Need Definitions (drag your ScriptableObjects here)")]
    public NeedDefinition[] needDefinitions;   // set in Inspector

    /* ────────────  RUNTIME  ───────────── */
    private readonly Dictionary<string, NeedRuntime> runtimesByGuid = new();
    private SavePayload payload;

    private const float SecondsPerMinute = 60f;
    private const float AutosaveIntervalSecs = 300f;   // 5 min

    /* ────────────  LIFECYCLE  ─────────── */
    private void Awake()
    {
        // Load or create save file
        if (!SaveSystem.TryLoad(out payload) || payload == null)
            payload = new SavePayload();

        foreach (NeedDefinition def in needDefinitions)
        {
            string key = def.GetInstanceID().ToString();

            NeedRuntime rt = payload.needs.Find(n => n.definitionGuid == key);
            if (rt == null)
            {
                rt = new NeedRuntime
                {
                    definitionGuid = key,
                    value = def.maxValue,
                    lastTimestamp = NowMs()
                };
                payload.needs.Add(rt);
            }

            ApplyOfflineDecay(rt, def);
            runtimesByGuid[key] = rt;
        }

        StartCoroutine(AutosaveLoop());
    }

    private void Update()
    {
        foreach (NeedDefinition def in needDefinitions)
        {
            NeedRuntime rt = GetRuntime(def);
            DrainWhileRunning(rt, def, Time.deltaTime);
        }
    }

    private void OnApplicationQuit() => SaveSystem.Save(payload);

    /* ────────────  PUBLIC API  ─────────── */
    public NeedRuntime GetRuntime(NeedDefinition def) =>
        runtimesByGuid[def.GetInstanceID().ToString()];

    public void AddNeedPoints(NeedDefinition def, float points)
    {
        NeedRuntime rt = GetRuntime(def);
        rt.value = Mathf.Clamp(rt.value + points, 0f, def.maxValue);
        rt.lastTimestamp = NowMs();
    }

    /* ────────────  HELPERS  ───────────── */
    private void ApplyOfflineDecay(NeedRuntime rt, NeedDefinition def)
    {
        float minutesElapsed = (NowMs() - rt.lastTimestamp) / 1000f / 60f;
        rt.value = Mathf.Clamp(
            rt.value - minutesElapsed * def.decayPerMinuteClosed,
            0f, def.maxValue);

        rt.lastTimestamp = NowMs();
    }

    private void DrainWhileRunning(NeedRuntime rt, NeedDefinition def, float dt)
    {
        rt.value = Mathf.Clamp(
            rt.value - dt / SecondsPerMinute * def.decayPerMinuteOpen,
            0f, def.maxValue);
    }

    private static long NowMs() =>
        DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    private IEnumerator AutosaveLoop()
    {
        var wait = new WaitForSeconds(AutosaveIntervalSecs);
        while (true)
        {
            yield return wait;
            SaveSystem.Save(payload);
        }
    }
}