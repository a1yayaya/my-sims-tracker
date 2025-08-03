// SaveSystem.cs
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// Tiny static helper that loads one shared SavePayload, lets anyone
/// read / mutate it, and writes it to disk on demand.
/// </summary>
public static class SaveSystem
{
    /* ───────────────  SINGLE SOURCE OF TRUTH  ─────────────── */

    private static SavePayload _payload;     // cached after first load

    /// <summary>
    /// The live SavePayload shared by all managers.
    /// Creates a new one if the file doesn’t exist.
    /// </summary>
    public static SavePayload Current
    {
        get
        {
            if (_payload == null)
            {
                // Try to load file once; if it fails we create a fresh payload
                if (!TryLoad(out _payload) || _payload == null)
                    _payload = new SavePayload();
            }
            return _payload;
        }
    }

    /* ───────────────  PUBLIC I/O  ─────────────────────────── */

    /// <summary>Serialize <paramref name="payload"/> (or Current) to JSON and write to disk.</summary>
    public static void Save(SavePayload payload = null)
    {
        if (payload == null) payload = Current;

        // Ensure folder exists (mobile platforms sometimes clear it)
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath));

        File.WriteAllText(
            FilePath,
            JsonConvert.SerializeObject(payload, Formatting.Indented));

#if UNITY_EDITOR
        Debug.Log($"[SaveSystem] Saved to {FilePath}");
#endif
    }

    /// <summary>Try to load an existing save file. Returns false if not found.</summary>
    public static bool TryLoad(out SavePayload payload)
    {
        if (File.Exists(FilePath))
        {
            payload = JsonConvert.DeserializeObject<SavePayload>(File.ReadAllText(FilePath));
#if UNITY_EDITOR
            Debug.Log($"[SaveSystem] Loaded save from {FilePath}");
#endif
            return true;
        }

        payload = null;
        return false;
    }

    /* ───────────────  INTERNAL  ───────────────────────────── */

    private static string FilePath =>
        Path.Combine(Application.persistentDataPath, "save.json");
}