using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillPointManager : MonoBehaviour
{
    private int enemyKillCount;
    private int skillPoints = 0;
    // Start is called before the first frame update
    void Start()
    {
        skillPoints = 0;
    }

    public void OnEnemyKilled()
    {
        enemyKillCount++;
        float dynamicChance = 0.025f + 0.01f * enemyKillCount; 

        if (Random.value <= dynamicChance)
        {
            skillPoints++;
            enemyKillCount = 0;
            Debug.Log("Skillpoint achieved => SkillCount: " + skillPoints);
        } else
        {
            Debug.Log("Not achieved Skillpoint");
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
