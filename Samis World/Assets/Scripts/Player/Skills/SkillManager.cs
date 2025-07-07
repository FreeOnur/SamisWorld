using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public List<SkillDefinition> allSkills;
    public List<SkillDefinition> chosenSkills = new List<SkillDefinition>();
    public PlayerAttack playerAttack;



    public void AddSkill(SkillDefinition skill)
    {
        chosenSkills.Add(skill);
        ApplySkill(skill);
    }
    void ApplySkill(SkillDefinition skill)
    {
        if (skill.skillName == "Fireball")
        {
            playerAttack.fireballUnlocked = true;
        }
        else if (skill.skillName == "Starker Schlag")
        {
            
        }
        // usw.
    }

}
