using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Skills/Skill")]
public class SkillDefinition : ScriptableObject
{
    public string skillName;
    public string description;
    public Sprite icon;
    public SkillType type;
    public float value;

    public enum SkillType { Passive, Active }
}
