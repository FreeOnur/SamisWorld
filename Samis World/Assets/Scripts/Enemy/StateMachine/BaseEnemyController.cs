using UnityEngine;
using Pathfinding;

public abstract class BaseEnemyController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 200f;
    public float nextWaypointDistance = 2f;
    public float stopDistance = 2f;
    public float jumpForce = 10f;

    [Header("Detection")]
    public float detectionRange = 5f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    [Header("Attack")]
    public float attackRange = 1f;
    public float attackCooldown = 2f;
    public float damageAmount = 10f;
    public LayerMask playerLayerMask;

    // Components
    [HideInInspector] public Transform target;
    [HideInInspector] public Seeker seeker;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Path path;
    [HideInInspector] public int currentWayPoint = 0;
    [HideInInspector] public bool reachedEndOfPath = false;
    [HideInInspector] public float lastAttackTime;
    [HideInInspector] public float facingDirection = 1f;
    public Transform dodgeCheckPoint;
    public Vector2 dodgeAngle;
    public float dodgeForce;
    public float dodgeDetectDistance;
    public float dodgeCooldown;
    public float dodgeChance = 0.5f; // 50% Chance
    [HideInInspector] public float lastDodgeTime;



    // State Machine
    protected EnemyStateMachine stateMachine;
    [HideInInspector] public EnemyIdleState idleState;
    [HideInInspector] public EnemyChaseState chaseState;
    [HideInInspector] public EnemyAttackState attackState;
    [HideInInspector] public EnemyDodgeState dodgeState;

    protected virtual void Awake()
    {
        // Get components
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Debug missing components
        if (seeker == null) Debug.LogError("Seeker component missing on " + gameObject.name);
        if (rb == null) Debug.LogError("Rigidbody2D component missing on " + gameObject.name);
        if (target == null) Debug.LogError("Player with tag 'Player' not found!");

        // Fix rotation
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Verhindert Rotation durch Physik

        // Initialize state machine
        stateMachine = new EnemyStateMachine();

        // Create states - make sure attackState is created first
        attackState = CreateAttackState();
        idleState = new EnemyIdleState(stateMachine, this);
        chaseState = new EnemyChaseState(stateMachine, this);
        dodgeState = new EnemyDodgeState(stateMachine, this);

        // Debug state creation
        Debug.Log($"States created - Attack: {attackState != null}, Idle: {idleState != null}, Chase: {chaseState != null}");
    }

    protected virtual void Start()
    {
        // Null check before initializing
        if (stateMachine == null)
        {
            Debug.LogError("StateMachine is null in Start()!");
            return;
        }

        if (idleState == null)
        {
            Debug.LogError("IdleState is null in Start()!");
            return;
        }

        stateMachine.Initialize(idleState);
        InvokeRepeating("UpdatePath", 0f, 1f);

        Debug.Log("State machine initialized successfully");
    }

    void Update()
    {
        // Null check before calling Update
        if (stateMachine == null)
        {
            Debug.LogError("StateMachine is null in Update()!");
            return;
        }

        stateMachine.Update();
    }

    void FixedUpdate()
    {
        // Fix rotation every frame
        transform.rotation = Quaternion.identity; // Stellt sicher, dass keine Rotation stattfindet

        // Null check before calling FixedUpdate
        if (stateMachine == null)
        {
            Debug.LogError("StateMachine is null in FixedUpdate()!");
            return;
        }

        stateMachine.FixedUpdate();
    }

    // Abstract method for creating attack state - implemented by derived classes
    protected abstract EnemyAttackState CreateAttackState();

    #region Helper Methods
    public bool IsGrounded()
    {
        if (groundCheck == null) return true; // Fallback
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    public float GetDistanceToTarget()
    {
        if (target == null) return float.MaxValue;
        return Vector2.Distance(transform.position, target.position);
    }

    public bool CheckIfDodge()
    {
        // Prüfe den Cooldown
        if (Time.time < lastDodgeTime + dodgeCooldown)
        {
            return false;
        }

        // Raycast zum Spieler
        RaycastHit2D hitPlayer = Physics2D.Raycast(dodgeCheckPoint.position, facingDirection == 1 ? Vector2.right : Vector2.left, dodgeDetectDistance, playerLayerMask);
        // Prüfe, ob der Spieler aggressiv ist
        bool isPlayerAggressive = facingDirection > 0 && Input.GetAxis("Horizontal") < 0 || facingDirection < 0 && Input.GetAxis("Horizontal") > 0;

        // Wenn Raycast trifft und Spieler aggressiv ist, prüfe die Wahrscheinlichkeit
        if (hitPlayer && isPlayerAggressive)
        {
            bool shouldDodge = Random.value < dodgeChance;
            Debug.Log($"Dodge Chance Check: {shouldDodge} (Chance: {dodgeChance * 100}%)");
            return shouldDodge;
        }

        return false;
    }

    public bool CanAttack()
    {
        bool cooldownReady = Time.time - lastAttackTime >= attackCooldown;
        bool inRange = GetDistanceToTarget() <= attackRange;
        return cooldownReady && inRange;
    }

    public void Flip(float directionX)
    {
        if (directionX > 0 && facingDirection != 1f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f); // Face right
            facingDirection = 1f;
            Debug.Log("Flipped to right");
        }
        else if (directionX < 0 && facingDirection != -1f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f); // Face left
            facingDirection = -1f;
            Debug.Log("Flipped to left");
        }
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
    #endregion

    protected virtual void OnDrawGizmosSelected()
    {
        // Detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}