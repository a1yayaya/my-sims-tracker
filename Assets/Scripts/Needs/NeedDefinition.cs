using UnityEngine;

[CreateAssetMenu(menuName = "SimsLike/Need")]
public class NeedDefinition : ScriptableObject
{
    public string needName;
    public float maxValue = 100;
    public float decayPerMinuteOpen = 0.5f;  // drain while app is open
    public float decayPerMinuteClosed = 0.25f; // drain while closed
}
