using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillBar : MonoBehaviour
{
    public Slider slider;
    public void SetMaxSkill(float skill)
    {
        slider.maxValue = skill;
        slider.value = skill;
    }
    public void SetSkill(float skill)
    {
        slider.value = skill;
    }
}
