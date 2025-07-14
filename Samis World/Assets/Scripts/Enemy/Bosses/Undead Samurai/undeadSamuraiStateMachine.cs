using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class undeadSamuraiMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 100f;
    public float nextWaypointDistance = 2f;
    public float stopDistance = 2f;
    [Header("BossFightRelated")]
    //Is the distance between boss and player when the boss should switch his attack from shortrange to longrange
    float attackSwitchDistance = 20f;
    public GameObject undeadEnemyPrefab;
    float spawnPosOffset = 5;
    bool spawnUndeadAttackFinished = true;
    bool swordSpinAttackFinished = true;
    public Transform attackTransform;
    float attackRange = 3f; // Setze einen Standardwert
    float damageAmount = 10f;
    public LayerMask playerLayerMask;
    private float swordSpinStartTime;
    private bool hasSpawnedUndead = false;
    [Header("PerformAttack")]
    float attackTimer;
    float attackCooldown = 2f;
    float lastAttackTime;
    [Header("SwordSpinAttack")]
    float swordSpinAttackCooldown = 0.1f; // Reduziert von 0.3f auf 0.1f für schnelleren Damage
    private float lastSpinDamageTime = 0f;
    public float spinDamageInterval = 0.1f; // Reduziert von 0.3f auf 0.1f
    private bool canDamagePlayer = true; // Neuer Flag für Damage-Kontrolle
    private float damageResetInterval = 0.2f; // Zeit zwischen Damage-Resets
    private bool startSwordSpinAttack = true;
    [Header("Grounded")]
    public float detectionRange = 5f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    [Header("MoveRightAndLeft")]
    private float moveDirection = 1f; // 1 = rechts, -1 = links
    private float moveDuration = 1f; // Zeit wie lange er in eine Richtung läuft
    private float moveTimer = 0f;

    float pathUpdateInterval = 0.5f;
    float lastPathUpdateTime;
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
        int spawnUndeadAmount = Random.Range(5, 10);

        for (int i = 0; i < spawnUndeadAmount; i++)
        {
            Vector3 spawnPos = gameObject.transform.position + new Vector3(Random.Range(-spawnPosOffset, spawnPosOffset), 0, 0);
            Instantiate(undeadEnemyPrefab, spawnPos, Quaternion.identity);
        }
    }
    void ChaseTarget()
    {
        if (Time.time - lastPathUpdateTime >= pathUpdateInterval)
        {
            UpdatePath();
            lastPathUpdateTime = Time.time;
        }

        if (path == null || currentWayPoint >= path.vectorPath.Count)
            return;

        // Richtung berechnen
        Vector2 direction = ((Vector2)path.vectorPath[currentWayPoint] - rb.position).normalized;

        // Nur horizontale Bewegung
        Vector2 force = new Vector2(direction.x, 0) * speed;

        rb.AddForce(force); // Keine Y-Kraft, kein Springen nötig


        // Nächster Wegpunkt?
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWayPoint]);
        if (distance < nextWaypointDistance)
        {
            currentWayPoint++;
        }
    }

    public bool IsGrounded()
    {
        if (groundCheck == null) return true; // Fallback
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
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
    void moveLeftAndRight()
    {
        moveTimer += Time.deltaTime;
        if(moveTimer > moveDuration)
        {
            moveDirection *= -1;
            moveTimer = 0;
        }
        Vector2 force = new Vector2(moveDirection, 0) * speed;


        rb.AddForce(force);
    }
    void swordSpinAttack()
    {
        // Prüfe kontinuierlich auf Kollision während des Spin-Attacks
        Collider2D hit = Physics2D.OverlapCircle(gameObject.transform.position, 0, playerLayerMask);

        if (hit != null && canDamagePlayer)
        {
            IDamagable damageable = hit.GetComponent<IDamagable>();
            if (damageable != null)
            {
                damageable.Damage(damageAmount);
                canDamagePlayer = false; // Verhindert sofortigen erneuten Damage
                // Setze den Damage-Flag nach kurzer Zeit zurück
                StartCoroutine(ResetDamageFlag());
            }
        }
    }

    // Coroutine zum Zurücksetzen des Damage-Flags
    private IEnumerator ResetDamageFlag()
    {
        yield return new WaitForSeconds(damageResetInterval);
        canDamagePlayer = true;
    }

    // Verbesserte swordSpinAttackExtend Methode
    void swordSpinAttackExtend()
    {
        // Führe den Spin-Attack kontinuierlich aus, nicht nur bei Timer-Intervallen
        swordSpinAttack();

        // Optional: Behalte den Timer für andere Zwecke
        attackTimer += Time.deltaTime;
        if (attackTimer >= swordSpinAttackCooldown)
        {
            lastAttackTime = Time.time;
            attackTimer = 0f;
        }
    }

    void hardAttack()
    {

    }
    void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                // wenn ich zum swordSpinAttack wechsle dann muss ich noch swordSpinStartTime = Time.time; hinzufügen also danach damit ich dan zählen kann zum Beispiel
                /* if(...) {
                 * currentState = State.Idle
                 * und attacktimer = 0; stellen
                 * swordSpinStartTime = Time.time;
                 * } */
                if (10 >= GetDistanceToTarget() && startSwordSpinAttack)
                {
                    currentState = State.swordSpinAttack;
                    swordSpinStartTime = Time.time;
                    attackTimer = 0;
                    canDamagePlayer = true; // Reset damage flag beim Start des Spin-Attacks
                    startSwordSpinAttack = false;
                }
                else if (9 <= GetDistanceToTarget())
                {
                    currentState = State.spawnUndeadAttack;
                    startSwordSpinAttack = true;
                }
                break;

            case State.Chase:
                break;

            case State.spawnUndeadAttack:
                if (!hasSpawnedUndead)
                {
                    spawnUndeadAttack();
                    hasSpawnedUndead = true;
                }

                attackTimer += Time.deltaTime;

                if (attackTimer >= attackCooldown)
                {
                    PerformAttack();
                    lastAttackTime = Time.time;
                    attackTimer = 0f;
                }

                spawnUndeadAttackFinished = CheckIfAllEnemiesDead();

                if (spawnUndeadAttackFinished)
                {
                    hasSpawnedUndead = false;
                    currentState = State.Idle;
                }
                break;

            case State.swordSpinAttack:
                swordSpinAttackExtend();
                moveLeftAndRight();
                if (Time.time - swordSpinStartTime >= 10f)
                {
                    currentState = State.Idle;
                    canDamagePlayer = true; // Reset beim Verlassen des Spin-Attacks
                }
                break;

            case State.hardAttack:

                break;
        }
    }
    private void PerformAttack()
    {
        Collider2D hit = Physics2D.OverlapCircle(attackTransform.position, attackRange, playerLayerMask);

        if (hit != null)
        {
            IDamagable damageable = hit.GetComponent<IDamagable>();
            if (damageable != null)
            {
                damageable.Damage(damageAmount);
            }
        }
    }
}