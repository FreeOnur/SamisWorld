using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerAttack : MonoBehaviour
{
    [Header("Skill Related Bools For Activation")]
    public bool fireballUnlocked = false;
    [Header("Projectile Attack")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 5f;
    private Transform target;
    void Start()
    {
        target = GetNearestEnemy();
    }
    public void PerformFireballAttack()
    {
        if (!fireballUnlocked || projectilePrefab == null || firePoint == null || target == null)
            return;

        Vector2 direction = (target.position - firePoint.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * projectileSpeed;
        }
    }


    private Transform GetNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform closest = null;
        float minDist = Mathf.Infinity;

        foreach (var enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < minDist)
            {
                minDist = distance;
                closest = enemy.transform;
            }
        }

        return closest;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
