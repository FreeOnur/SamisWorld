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
    [SerializeField] private float swordSpinAttackRange = 0.1f;
    [Header("Grounded")]
    public float detectionRange = 5f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    [Header("MoveRightAndLeft")]
    private float moveDirection = 1f; // 1 = rechts, -1 = links
    private float moveDuration = 1f; // Zeit wie lange er in eine Richtung läuft
    private float moveTimer = 0f;
    [Header("DashAttack")]
    bool isDashing;
    public float dashTime;
    public float dashSpeed = 20f;
    public float dashDuration = 0.5f;
    public bool startDashAttack;
    public bool startUndeadAttack;
    private float dashStartTime; // Neue Variable für Dash-Timing
    [Header("WaveAttack")]
    float pathUpdateInterval = 0.5f;
    float lastPathUpdateTime;
    public GameObject wavePrefab;
    public float waveSpeed = 5f;
    public enum State
    {
        Idle,
        Chase,
        spawnUndeadAttack,
        dashAttack,
        swordSpinAttack,
        waveAttack, //spawnt eine schwarze welle die so damage macht usw keine ahnung
        teleportAttack // er geht in boden und taucht beim spieler auf und macht damage sobald er auftaucht
        //!NOTIZ! Es soll noch einen rage modus geben wo er mehrere attacken schnell kombiniert ab 30% unter leben wird er sauer und stärker usw

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
        dashTime = dashDuration;
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Debug missing components
        if (seeker == null) Debug.LogError("Seeker component missing on " + gameObject.name);
        if (rb == null) Debug.LogError("Rigidbody2D component missing on " + gameObject.name);
        if (target == null) Debug.LogError("Player with tag 'Player' not found!");

        // Fix rotation und Collision Detection
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // Gegen Tunneling
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
        if (moveTimer > moveDuration)
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
        Collider2D hit = Physics2D.OverlapCircle(gameObject.transform.position, swordSpinAttackRange, playerLayerMask);

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

    void dashAttack()
    {
        if (isDashing)
        {
            Debug.Log("Dashing...");
            Vector2 dashDirection = (target != null) ? ((target.position - transform.position).normalized) : Vector2.right;
            rb.velocity = new Vector2(dashDirection.x * dashSpeed, rb.velocity.y);

            if (Time.time - dashStartTime >= dashDuration)
            {
                Debug.Log("Dash finished.");
                isDashing = false;
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
            Collider2D hit = Physics2D.OverlapCircle(transform.position, 0.5f, playerLayerMask);
            if (hit != null && canDamagePlayer)
            {
                IDamagable damagable = hit.GetComponent<IDamagable>();
                if (damagable != null)
                {
                    damagable.Damage(damageAmount);
                    canDamagePlayer = false;
                    StartCoroutine(ResetDamageFlag());
                    Debug.Log("Player hit during dash!");
                }
            }
        }
    }

    // Coroutine zum Zurücksetzen des Damage-Flags
    private IEnumerator ResetDamageFlag()
    {
        yield return new WaitForSeconds(damageResetInterval);
        canDamagePlayer = true;
    }

    void swordSpinAttackExtend()
    {
        swordSpinAttack();
        attackTimer += Time.deltaTime;
        if (attackTimer >= swordSpinAttackCooldown)
        {
            lastAttackTime = Time.time;
            attackTimer = 0f;
        }
    }

    void waveAttack()
    {
        if (wavePrefab == null || target == null)
            return;
        Vector2 direction = (target.position - transform.position).normalized;
        GameObject projectile = Object.Instantiate(wavePrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if(rb != null)
        {
            rb.velocity = direction * waveSpeed;
        }
    }
    void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                if (startSwordSpinAttack)
                {
                    currentState = State.swordSpinAttack;
                    swordSpinStartTime = Time.time;
                    attackTimer = 0;
                    canDamagePlayer = true;
                    startSwordSpinAttack = false;
                    startDashAttack = true;
                }
                else if (9 <= GetDistanceToTarget() && startUndeadAttack)
                {
                    currentState = State.spawnUndeadAttack;
                    canDamagePlayer = true;
                    startDashAttack = true;
                }
                else if (5 <= GetDistanceToTarget() && startDashAttack)
                {
                    Debug.Log("Switching to Dash Attack State");
                    canDamagePlayer = true;
                    currentState = State.dashAttack;
                    isDashing = true; // Setze isDashing hier
                    dashStartTime = Time.time; // Setze den Startzeitpunkt
                    startDashAttack = false; // Deaktiviere nach Transition
                    startUndeadAttack = true; // Optional, je nach Logik
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
                    canDamagePlayer = true;
                }
                break;

            case State.dashAttack:
                dashAttack();
                if (!isDashing && Time.time - dashStartTime >= dashDuration)
                {
                    Debug.Log("Dash Attack finished, returning to Idle");
                    currentState = State.Idle;
                    startDashAttack = false; // Erlaube erneuten Dash
                }
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