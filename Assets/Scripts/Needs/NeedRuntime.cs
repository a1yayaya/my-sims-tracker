using System;   // Serializable

/// <summary>
/// Runtime state for a single need (e.g. Hunger) that we save to disk.
/// </summary>
[Serializable]
public class NeedRuntime
{
    /// <summary>Unique string that links back to the NeedDefinition asset.</summary>
    public string definitionGuid;

    /// <summary>Current value between 0 and NeedDefinition.maxValue.</summary>
    public float value;

    /// <summary>UTC time in milliseconds when this value was last updated.</summary>
    public long lastTimestamp;
}