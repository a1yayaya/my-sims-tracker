using System;

[Serializable]
public class SkillRuntime
{
    public string id; // built in definition
    public string name;
    public Difficulty difficulty;
    public string iconId;
    public float xp = 0f;
    public int level = 0;
    public float xp2 = 0f;
}