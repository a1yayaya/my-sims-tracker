using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Central controller for player Needs: loads data, applies decay, and
/// exposes AddNeedPoints for the UI.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Need Definitions (drag your ScriptableObjects here)")]
    public NeedDefinition[] needDefinitions;

    /* runtime */
    private readonly Dictionary<string, NeedRuntime> runtimesByGuid = new();
    private SavePayload payload;                // shared reference

    private const float SecondsPerMinute = 60f;
    private const float AutosaveIntervalSecs = 300f;  // 5 min

    /*──────────────────────────────────────────────*/

    void Awake()
    {
        payload = SaveSystem.Current;           // ← shared object

        // Ensure every NeedDefinition has a runtime entry
        foreach (NeedDefinition def in needDefinitions)
        {
            string key = def.GetInstanceID().ToString();

            if (!runtimesByGuid.TryGetValue(key, out NeedRuntime rt))
            {
                rt = payload.needs.Find(n => n.definitionGuid == key);
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
                runtimesByGuid[key] = rt;
            }

            ApplyOfflineDecay(rt, def);
        }

        StartCoroutine(AutosaveLoop());
    }

    void Update()
    {
        foreach (NeedDefinition def in needDefinitions)
            DrainWhileRunning(GetRuntime(def), def, Time.deltaTime);
    }

    void OnApplicationQuit() => SaveSystem.Save();   // one final write

    /*──────────────── public API ────────────────*/

    public NeedRuntime GetRuntime(NeedDefinition def) =>
        runtimesByGuid[def.GetInstanceID().ToString()];

    public void AddNeedPoints(NeedDefinition def, float points)
    {
        NeedRuntime rt = GetRuntime(def);
        rt.value = Mathf.Clamp(rt.value + points, 0f, def.maxValue);
        rt.lastTimestamp = NowMs();
        SaveSystem.Save();                          // immediate persist
    }

    /*──────────── helper methods ───────────────*/

    void ApplyOfflineDecay(NeedRuntime rt, NeedDefinition def)
    {
        float minutes = (NowMs() - rt.lastTimestamp) / 1000f / 60f;
        rt.value = Mathf.Clamp(
            rt.value - minutes * def.decayPerMinuteClosed,
            0f, def.maxValue);
        rt.lastTimestamp = NowMs();
    }

    void DrainWhileRunning(NeedRuntime rt, NeedDefinition def, float dt)
    {
        rt.value = Mathf.Clamp(
            rt.value - dt / SecondsPerMinute * def.decayPerMinuteOpen,
            0f, def.maxValue);
    }

    static long NowMs() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    IEnumerator AutosaveLoop()
    {
        var wait = new WaitForSeconds(AutosaveIntervalSecs);
        while (true) { yield return wait; SaveSystem.Save(); }
    }
}