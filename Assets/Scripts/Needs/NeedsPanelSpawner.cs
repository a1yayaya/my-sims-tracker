using UnityEngine;

public class NeedsPanelSpawner : MonoBehaviour
{
    public NeedDefinition[] definitions;      // assign manually or Resources.Load
    public GameObject rowPrefab;              // HungerRow prefab

    void Start()
    {
        foreach (var def in definitions)
        {
            var row = Instantiate(rowPrefab, transform);
            var ui = row.GetComponent<NeedUI>();
            // connect runtime mapping later; for Hunger-only demo just set label
            ui.definition = def;
        }
    }
}
