using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class PlayerAttack : MonoBehaviour
{
    [Header("Skill Related Bools For Activation")]
    public bool fireballUnlocked = false;
    public bool strongHitUnlocked = false;
    public bool doubleJumpUnlocked = false;
    public bool bloodyDamageUnlocked = true;
    [Header("Projectile Attack")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 5f;
    private Transform target;
    [Header("Double Jump")]
    public int maxJumpCount = 10;
    public int jumpCountStart = 0;
    [Header("Strong Attack")]
    private bool isCharging = false;
    private float chargeStartTime;
    public float maxChargeTime = 3f;
    public float maxDamageMultiplier = 3f;
    public Transform attackPoint;
    public LayerMask enemyLayers;
    [SerializeField] private float attackRange = 2f;
    [Header("Instances")]
    private EnemyHealth enemyHealth;
    private Combos combos;
    public PlayerData playerData;
    private PlayerMovement player;
    private Player playerScript;
    public float originalDamage;

    void Start()
    {
        target = GetNearestEnemy();
        combos = GetComponent<Combos>();
        player = GetComponent<PlayerMovement>();
        playerScript = GetComponent<Player>();
        originalDamage = combos.damageAmount;
    }
    public void PerformFireballAttack()
    {
        if (target == null)
        {
            target = GetNearestEnemy();
        }

        if (!fireballUnlocked || projectilePrefab == null || firePoint == null || target == null)
            return;

        Vector2 direction = (target.position - firePoint.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        projectile.GetComponent<Projectile>().isEnemyProjectile = false;
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * projectileSpeed;
        }
    }

    public void PerformStrongAttack()
    {
        if (!strongHitUnlocked)
            return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            isCharging = true;
            chargeStartTime = Time.time;
        }

        if (Input.GetKeyUp(KeyCode.Mouse0) && isCharging)
        {
            isCharging = false;
            float heldTime = Mathf.Clamp(Time.time - chargeStartTime, 0f, maxChargeTime);
            float chargeRatio = heldTime / maxChargeTime;
            float finalMultiplier = Mathf.Lerp(1f, maxDamageMultiplier, chargeRatio);

            float baseDamage = combos.damageAmount;
            float finalDamage = baseDamage * finalMultiplier;
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
            foreach (Collider2D enemy in hitEnemies)
            {
                EnemyHealth eh = enemy.GetComponent<EnemyHealth>();
                if (eh != null)
                {
                    eh.Damage(finalDamage);
                }
            }

            Debug.Log($"Strong Attack: {finalDamage} Damage to {hitEnemies.Length} enemies");
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

    public float GetDamage()
    {
        float healthPercentage = playerScript.CurrentHealth / playerData.maxHealth;
        float damageMultiplier = 1;
        int healthStage = Mathf.FloorToInt(healthPercentage * 4);
        switch (healthStage)
        {
            case 0:  // 0-25% HP
                damageMultiplier = 2.0f;
                break;
            case 1:  // 25-50% HP
                damageMultiplier = 1.5f;
                break;
            case 2:  // 50-75% HP
                damageMultiplier = 1.25f;
                break;
            case 3:  // 75-100% HP
            default:
                damageMultiplier = 1.0f;
                break;
        }
        float finalDamage = originalDamage * damageMultiplier;
        Debug.Log("Damage is " + finalDamage);
        return finalDamage;

    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            PerformFireballAttack();
        }
        PerformStrongAttack();
    }
}
