using UnityEngine;

/// <summary>
/// Spawns one SkillRow prefab for each SkillRuntime known to the
/// SkillsManager, and listens for new skills created at runtime.
/// </summary>
public class SkillsPanelSpawner : MonoBehaviour
{
    [Tooltip("Drag the SkillRow prefab here")]
    public GameObject rowPrefab;          // <<< assign in Inspector

    private SkillsManager mgr;            // ⭐ keep a reference

    /*───────────────────────────────*/
    void Awake()
    {
        mgr = FindObjectOfType<SkillsManager>();
        if (mgr == null)
        {
            Debug.LogError("[SkillsPanelSpawner] No SkillsManager found.");
            enabled = false;
            return;
        }

        mgr.OnSkillCreated += SpawnRow;   // ⭐ listen for new skills
        mgr.OnSkillRemoved += OnSkillRemoved;
    }

    void Start()
    {
        foreach (var rt in mgr.AllSkills())   // built-in + previously saved
            SpawnRow(rt);
    }

    /*───────────────────────────────*/
    private void SpawnRow(SkillRuntime rt)    // ⭐ centralises row creation
    {
        var row = Instantiate(rowPrefab, transform);
        var ui = row.GetComponent<SkillUI>();
        ui.runtime = rt;
    }
    private void OnSkillRemoved(SkillRuntime rt)
    {
        foreach (Transform child in transform)
        {
            var ui = child.GetComponent<SkillUI>();
            if (ui != null && ui.runtime == rt)
            {
                Destroy(child.gameObject);
                break;
            }
        }
    }


    /*───────────────────────────────*/
    void OnDestroy()                          // ⭐ unsubscribe on scene unload
    {
        if (mgr != null)
            mgr.OnSkillCreated -= SpawnRow;
    }
}
