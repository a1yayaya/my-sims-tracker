using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles skill XP, level-ups, and user-created skills.
/// </summary>
public class SkillsManager : MonoBehaviour
{
    [Header("Built-in Skill Definitions")]
    public SkillDefinition[] baseDefinitions;

    private readonly Dictionary<string, SkillRuntime> runtimeById = new();
    private SavePayload payload;              // shared reference

    public event Action<SkillRuntime> OnSkillCreated;

    /*────────────────────────────────────────────*/

    void Awake()
    {
        payload = SaveSystem.Current;         // ← shared object

        // 1. Convert built-in definitions → runtime objects
        if (baseDefinitions != null)
        {
            foreach (SkillDefinition def in baseDefinitions)
            {
                string id = def.GetInstanceID().ToString();
                SkillRuntime rt = payload.skills.Find(s => s.id == id);
                if (rt == null)
                {
                    rt = new SkillRuntime
                    {
                        id = id,
                        name = def.name,   // use field in SO
                        difficulty = def.difficulty
                    };
                    payload.skills.Add(rt);
                }
                runtimeById[id] = rt;
            }
        }

        // 2. Index any user-created skills already saved
        foreach (var rt in payload.skills)
            runtimeById[rt.id] = rt;

        StartCoroutine(Autosave());
    }

    /*──────────── public API ───────────────────*/

    public IEnumerable<SkillRuntime> AllSkills() => payload.skills;

    public void AddHours(SkillRuntime rt, float hours)
    {
        float xpGain = HoursToXp(hours, rt.difficulty);
        rt.xp += xpGain;

        while (rt.xp >= XpForLevel(rt.level, rt.difficulty))
        {
            rt.xp -= XpForLevel(rt.level, rt.difficulty);
            rt.level++;
        }
        SaveSystem.Save();                    // persist immediately
    }

    public SkillRuntime CreateUserSkill(string name, Difficulty diff, string iconId)
    {
        var rt = new SkillRuntime
        {
            id = Guid.NewGuid().ToString(),
            name = name,
            difficulty = diff,
            iconId = iconId
        };
        payload.skills.Add(rt);
        runtimeById[rt.id] = rt;

        SaveSystem.Save();                    // persist
        OnSkillCreated?.Invoke(rt);           // notify UI
        return rt;
    }

    public float XpNeeded(int level, Difficulty diff) =>
        XpForLevel(level, diff);

    /*──────────── math helpers ────────────────*/

    float HoursToXp(float hrs, Difficulty d) => hrs * d switch
    {
        Difficulty.VeryEasy => 50,
        Difficulty.Easy => 30,
        Difficulty.Medium => 20,
        Difficulty.Hard => 12,
        _ => 8          // VeryHard
    };

    float XpForLevel(int lvl, Difficulty d)
        => (lvl + 1) * 100 * (1f + (int)d * 0.25f);

    /*──────────── autosave ────────────────────*/

    IEnumerator Autosave()
    {
        var wait = new WaitForSeconds(300);
        while (true) { yield return wait; SaveSystem.Save(); }
    }
}