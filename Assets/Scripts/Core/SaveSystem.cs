using System.IO;
using UnityEngine;
using Newtonsoft.Json;     // needs the package you added earlier

/// <summary>
/// Lightweight wrapper for saving / loading one SavePayload JSON file.
/// </summary>
public static class SaveSystem
{
    // File lives in OS-specific persistent-data folder.
    private static string FilePath =>
        Path.Combine(Application.persistentDataPath, "save.json");

    /// <summary>Serialize and write to disk.</summary>
    public static void Save(SavePayload payload)
    {
        // Ensure parent folder exists (mobile platforms sometimes delete it)
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
        File.WriteAllText(FilePath,
            JsonConvert.SerializeObject(payload, Formatting.Indented));
#if UNITY_EDITOR
        Debug.Log($"[SaveSystem] Saved to {FilePath}");
#endif
    }

    /// <summary>Try to load. Returns true if a file was found and parsed.</summary>
    public static bool TryLoad(out SavePayload payload)
    {
        if (File.Exists(FilePath))
        {
            string json = File.ReadAllText(FilePath);
            payload = JsonConvert.DeserializeObject<SavePayload>(json);
#if UNITY_EDITOR
            Debug.Log($"[SaveSystem] Loaded save from {FilePath}");
#endif
            return true;
        }

        payload = null;
        return false;
    }
}
