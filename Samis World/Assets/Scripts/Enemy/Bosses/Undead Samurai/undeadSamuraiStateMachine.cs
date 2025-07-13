using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class undeadSamuraiMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 200f;
    public float nextWaypointDistance = 2f;
    public float stopDistance = 2f;
    public float jumpForce = 10f;
    [Header("BossFightRelated")]
    //Is the distance between boss and player when the boss should switch his attack from shortrange to longrange
    float attackSwitchDistance = 20f;
    public GameObject undeadEnemyPrefab;
    float spawnPosOffset = 5;
    bool spawnUndeadAttackFinished = true;
    bool swordSpinAttackFinished = true;
    public Transform attackTransform;
    float attackRange;
    float damageAmount = 10f;
    public LayerMask playerLayerMask;
    private float swordSpinStartTime;
    [Header("PerformAttack")]
    float attackTimer;
    float attackCooldown = 2f;
    float lastAttackTime;
    [Header("SwordSpinAttack")]
    float swordSpinAttackCooldown = 0.3f;

    public enum State
    {
        Idle,
        Chase,
        spawnUndeadAttack,
        hardAttack,
        swordSpinAttack,
    }
    public State currentState;

    [HideInInspector] public Transform target;
    [HideInInspector] public Seeker seeker;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Path path;
    [HideInInspector] public int currentWayPoint = 0;
    [HideInInspector] public bool reachedEndOfPath = false;
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Debug missing components
        if (seeker == null) Debug.LogError("Seeker component missing on " + gameObject.name);
        if (rb == null) Debug.LogError("Rigidbody2D component missing on " + gameObject.name);
        if (target == null) Debug.LogError("Player with tag 'Player' not found!");

        // Fix rotation
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        currentState = State.Idle;
    }
    // Is the Distance Between Boss and Target
    public float GetDistanceToTarget()
    {
        if (target == null) return float.MaxValue;
        return Vector2.Distance(transform.position, target.position);
    }

    public void UpdatePath()
    {
        if (target != null && seeker != null && seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    public void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWayPoint = 0;
        }
    }

    public void StopMovement()
    {
        if (rb != null)
            rb.velocity = Vector2.zero;
    }

    void spawnUndeadAttack()
    {
        spawnUndeadAttackFinished = false;
        Vector3 spawnPos = gameObject.transform.position + new Vector3(Random.Range(-spawnPosOffset, spawnPosOffset), 0, 0);
        int spawnUndeadAmount = Random.Range(5, 10);
        for (int i = 0; i < spawnUndeadAmount; i++)
        {
            Instantiate(undeadEnemyPrefab, gameObject.transform.position, Quaternion.identity);
        }
    }

    public bool CheckIfAllEnemiesDead()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    void swordSpinAttack()
    {
        swordSpinAttackFinished = false;
        Debug.Log("Perform Sword Spin Attack called!");

        Debug.Log($"Checking attack at position: {gameObject.transform.position}, Range: {attackRange}");

        Collider2D hit = Physics2D.OverlapCircle(gameObject.transform.position, attackRange, playerLayerMask);

        if (hit != null)
        {
            Debug.Log($"Hit detected: {hit.name}");
            IDamagable damageable = hit.GetComponent<IDamagable>();
            if (damageable != null)
            {
                Debug.Log($"Dealing {damageAmount} damage!");
                damageable.Damage(damageAmount);
            }
            else
            {
                Debug.LogError("No IDamagable component found on " + hit.name);
            }
        }
        else
        {
            Debug.Log("No hit detected in attack range!");
        }
    }
    void swordSpinAttackExtend()
    {
        attackTimer += Time.deltaTime;

        // Perform attack
        if (attackTimer >= swordSpinAttackCooldown)
        {
            swordSpinAttack();
            lastAttackTime = Time.time;
            attackTimer = 0f;
        }
    }
    
    void hardAttack()
    {

    }
    void Update()
    {
        switch(currentState)
        {
            case State.Idle:
                // wenn ich zum swordSpinAttack wechsle dann muss ich noch swordSpinStartTime = Time.time; hinzufügen also danach damit ich dan zählen kann zum Beispiel
                /* if(...) {
                 * currentState = State.Idle
                 * und attacktimer = 0; stellen
                 * swordSpinStartTime = Time.time;
                 * } */
                break;

            case State.Chase:
 
                break;

            case State.spawnUndeadAttack:
                UpdatePath();

                attackTimer += Time.deltaTime;

                // Perform attack
                if (attackTimer >= attackCooldown)
                {
                    PerformAttack();
                    lastAttackTime = Time.time;
                    attackTimer = 0f;
                }
                spawnUndeadAttack();
                spawnUndeadAttackFinished = CheckIfAllEnemiesDead();
                if(spawnUndeadAttackFinished)
                {
                    currentState = State.Idle;
                }
                break;

            case State.swordSpinAttack:
                swordSpinAttackExtend();
                if (Time.time - swordSpinStartTime >= 2f)
                {
                    currentState = State.Idle;
                }
                break;

            case State.hardAttack:

                break;
        }
    }
    private void PerformAttack()
    {
        Debug.Log("PerformAttack called!");

        if (attackTransform == null)
        {
            Debug.LogError("Attack Transform is null!");
            return;
        }

        Debug.Log($"Checking attack at position: {attackTransform.position}, Range: {attackRange}");

        Collider2D hit = Physics2D.OverlapCircle(attackTransform.position, attackRange, playerLayerMask);

        if (hit != null)
        {
            Debug.Log($"Hit detected: {hit.name}");
            IDamagable damageable = hit.GetComponent<IDamagable>();
            if (damageable != null)
            {
                Debug.Log($"Dealing {damageAmount} damage!");
                damageable.Damage(damageAmount);
            }
            else
            {
                Debug.LogError("No IDamagable component found on " + hit.name);
            }
        }
        else
        {
            Debug.Log("No hit detected in attack range!");
        }
    }
}
