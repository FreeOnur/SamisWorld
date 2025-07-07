using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillPointManager : MonoBehaviour
{
    private int enemyKillCount;
    private int skillPoints = 0;
    private TextMeshProUGUI skillPointCountText;
    // Start is called before the first frame update
    void Start()
    {
        skillPoints = 0;
        skillPointCountText = GameObject.Find("SkillPoint").GetComponent<TextMeshProUGUI>();
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
        skillPointCountText.text = $"SP: {skillPoints}";
    }
}
