using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpawnWaveManager : MonoBehaviour
{
    int waveCount = 0;
    public Transform player;
    bool waveFinished = true;
    int startEnemeyAmount = 5;
    int enemyAmount;
    int spawnPosOffset = 10;
    int enemyMultiplier = 2;
    int spawnDelay = 1;
    [Header("UI")]
    private TextMeshProUGUI waveCountText;
    [Header("Enemies")]
    public GameObject projectileEnemy;
    public GameObject meleeEnemy;
    // Start is called before the first frame update
    void Start()
    {
        waveCount = 0;
        enemyAmount = startEnemeyAmount;
        waveCountText = GameObject.Find("WaveCount").GetComponent<TextMeshProUGUI>();
    }
    public bool CheckIfAllEnemiesDead()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0)
        {
            return true;
        } else
        {
            return false;
        }
    }
    public IEnumerator SpawnMixedEnemies(int totalEnemies)
    {
        int projectileCount = Mathf.FloorToInt(totalEnemies * 0.25f);
        int meleeCount = totalEnemies - projectileCount;

        List<GameObject> spawnQueue = new List<GameObject>();

        // Füge die richtige Anzahl beider Typen zur Liste hinzu
        for (int i = 0; i < projectileCount; i++) spawnQueue.Add(projectileEnemy);
        for (int i = 0; i < meleeCount; i++) spawnQueue.Add(meleeEnemy);

        // Liste mischen (Fisher–Yates Shuffle)
        for (int i = 0; i < spawnQueue.Count; i++)
        {
            int rnd = Random.Range(i, spawnQueue.Count);
            (spawnQueue[i], spawnQueue[rnd]) = (spawnQueue[rnd], spawnQueue[i]);
        }

        // Spawnen
        foreach (var enemyPrefab in spawnQueue)
        {
            Vector3 spawnPos = player.transform.position + new Vector3(Random.Range(-spawnPosOffset, spawnPosOffset), 0, 0);
            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    IEnumerator loopDelay(int delayTime)
    {
        yield return new WaitForSeconds(delayTime);
    }
    public void SpawnWaves()
    {
        if (waveFinished)
        {
            waveCount++;
            StartCoroutine(SpawnMixedEnemies(enemyAmount));
            waveFinished = false;
            enemyAmount *= enemyMultiplier;
        }
    }
    void Update()
    {
        waveCountText.text = $"WAVES: {waveCount}";
        waveFinished = CheckIfAllEnemiesDead();   
        SpawnWaves();
    }
}
