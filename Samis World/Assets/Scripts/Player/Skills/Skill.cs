using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public string skillName;
    public int skillID;
    public int skillLevel;
    public int skillMaxLevel;
    public bool isUnlocked;

    public Skill(string name, int id, int maxLevel)
    {
        skillName = name;
        skillID = id;
        skillLevel = 0;
        skillMaxLevel = maxLevel;
        isUnlocked = false;
    }

    public void LevelUp ()
    {
        if (skillLevel < skillMaxLevel)
        {
            skillLevel++;
            if (skillLevel == 1)
            {
                isUnlocked = true;
            }
        } else
        {
            Debug.Log("Max level erreicht");
        }
    }

}
