
using UnityEngine;

public enum Difficulty { VeryEasy, Easy, Medium, Hard, VeryHard }

[CreateAssetMenu(menuName = "SimsLike/Skill")]
public class SkillDefinition : ScriptableObject
{

    public string skilName = "Cooking";
    public Difficulty difficulty = Difficulty.Medium;
    public int xpPerLevel = 100;

}

