using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public List<SkillDefinition> allSkills;
    public List<SkillDefinition> chosenSkills = new List<SkillDefinition>();

    public void ChooseRandomSkill(int count)
    {
        List<SkillDefinition> skillOptions = new List<SkillDefinition>();

        while(skillOptions.Count < count)
        {
            SkillDefinition skill = allSkills[Random.Range(0, allSkills.Count)];
            if (!skillOptions.Contains(skill) && !chosenSkills.Contains(skill))
            {
                skillOptions.Add(skill);
            }
        }

    }

    public void AddSkill(SkillDefinition skill)
    {
        chosenSkills.Add(skill);
        ApplySkill(skill);
    }
    void ApplySkill(SkillDefinition skill)
    {
        if (skill.skillName == "Fireball")
        {
            // Fireball aktivieren
        }
        else if (skill.skillName == "Damage Up")
        {
            // Damage erhöhen
        }
        // usw.
    }

}
