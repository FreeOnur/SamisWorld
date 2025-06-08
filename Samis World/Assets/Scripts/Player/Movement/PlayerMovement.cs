using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerMovement : AnimatorBrain
{
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameObject dustPrefab;

    public static PlayerMovement instance;
    private PlayerState currentState;
    private Rigidbody2D rb;
    private Animator animator;

    [Header("Move Info")]
    private float horizontalInput;
    private bool isFacingRight = true;
    private bool wasGrounded = true;

    [Header("Animation Info")]
    private bool isPlayingLandingAnimation = false;
    private float landingAnimationDuration = 0.05f;

    [Header("Jump Info")]
    public bool isJumping;
    public float jumpHangTimeThreshold = 4f;

    public PlayerData playerData;
    public Player playerScript;

    [HideInInspector] public bool ledgeDetected;

    [Header("Ledge Info")]
    [SerializeField] private Vector2 offset1;
    private Vector2 climbBegunPosition;
    private bool canGrabLedge = true;
    private bool canClimb;
    private bool isHangingOnLedge = false;
    [SerializeField] private LedgeDetection ledgeDetection;
    [SerializeField] private Transform ledgeGrab;
    Vector2 hangPosition;
    Vector2 ledgePos;

    [Header("Ledge Jump Info")]
    [SerializeField] private float ledgeJumpForce = 12f;
    [SerializeField] private float ledgeJumpHorizontalForce = 5f;

    [Header("Wall")]
    public bool isWallSliding;
    public float wallSlidingSpeed = 4f;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;

    // Eigenschaften für den Zugriff auf private Felder
    public Rigidbody2D PlayerRb => rb;
    public Animator Anim => animator;
    public float HorizontalInput => horizontalInput;
    public bool IsHangingOnLedge => isHangingOnLedge;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Initialize(GetComponent<Animator>().layerCount, Animations.IDLE, GetComponent<Animator>(), DefaultAnimation);
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        ChangeState(new IdleState(this));
    }

    private void Update()
    {
        if (isHangingOnLedge)
        {
            CheckForLedge();
            return;
        }

        horizontalInput = Input.GetAxisRaw("Horizontal");
        currentState?.HandleInput();
        Flip();

        // WICHTIG: Nur Animationen checken wenn nicht attackiert wird
        if (!(currentState is DashState) && !GetComponent<Combos>().isAttacking)
        {
            CheckMovementAnimations(0);
        }

        CheckFallAndLandAnimations();
        CheckForLedge();
        WallState();
    }

    private void FixedUpdate()
    {
        currentState?.PhysicsUpdate();
    }

    private void CheckForLedge()
    {
        if (ledgeDetected && canGrabLedge && !isHangingOnLedge)
        {
            canGrabLedge = false;
            isHangingOnLedge = true;
            hangPosition = ledgeGrab.position;
            Play(Animations.CLIMB, 0, false, false);
        }

        if (isHangingOnLedge)
        {
            transform.position = hangPosition;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PerformLedgeJump();
            }
        }
    }

    public void WallState()
    {
        if (IsWalled() && !IsGrounded() && rb.velocity.y < 0)
        {
            ChangeState(new WallSlideState(this));
        }
    }

    private void PerformLedgeJump()
    {
        isHangingOnLedge = false;
        rb.simulated = true;
        rb.gravityScale = 1f;

        Vector2 horizontalOffset = new Vector2(isFacingRight ? -0.2f : 0.2f, 0f);
        transform.position = (Vector2)transform.position + horizontalOffset;

        Vector2 jumpForce = new Vector2(isFacingRight ? -ledgeJumpHorizontalForce : ledgeJumpHorizontalForce, ledgeJumpForce);
        rb.velocity = jumpForce;
        Play(Animations.JUMP, 0, false, false);
        StartCoroutine(ResetLedgeGrab(0.5f));
        ledgeDetected = false;
    }

    private IEnumerator ResetLedgeGrab(float delay)
    {
        yield return new WaitForSeconds(delay);
        canGrabLedge = true;
    }

    public virtual bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    public virtual bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    public bool IsTouchingWall()
    {
        return IsWalled() && !IsGrounded();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(wallCheck.position, 0.2f);
    }

    public virtual void ChangeState(PlayerState newState)
    {
        if (isHangingOnLedge) return;
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    private void Flip()
    {
        if (isFacingRight && horizontalInput < 0f || !isFacingRight && horizontalInput > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void CheckMovementAnimations(int layer)
    {
        if (isHangingOnLedge) return;
        if (isPlayingLandingAnimation) return;

        // WICHTIG: Nicht überschreiben wenn gerade attackiert wird
        if (GetComponent<Combos>().isAttacking) return;

        if (!IsGrounded())
        {
            if (rb.velocity.y > 0)
            {
                Play(Animations.JUMP, layer, false, false);
            }
        }
        else if (Mathf.Abs(rb.velocity.x) > 0.1f && Mathf.Abs(horizontalInput) > 0.1f)
        {
            bool shouldUseSwordRun = IsUsingSwordRun();

            if (shouldUseSwordRun)
            {
                Play(Animations.RUN_SWORD, layer, false, false);
            }
            else
            {
                Play(Animations.RUN, layer, false, false);
            }
        }
        else
        {
            // Nur IDLE wenn nicht useSwordRun aktiv ist
            if (!IsUsingSwordRun())
            {
                Play(Animations.IDLE, layer, false, false);
            }
            else
            {
                Play(Animations.RUN_SWORD, layer, false, false);
            }
        }

        Debug.Log($"Velocity: {rb.velocity.x}, Input: {horizontalInput}, SwordRun: {IsUsingSwordRun()}, CurrentState: {animator.GetCurrentAnimatorStateInfo(layer).IsName("Run_Sword")}");
    }

    private void CheckFallAndLandAnimations()
    {
        bool isGrounded = IsGrounded();

        if (!wasGrounded && isGrounded && !isPlayingLandingAnimation)
        {
            StartCoroutine(PlayLandingAnimation());
        }
        else if (!isGrounded && rb.velocity.y < 0)
        {
            Play(Animations.FALL, 0, false, false);
        }

        wasGrounded = isGrounded;
    }

    private IEnumerator PlayLandingAnimation()
    {
        if (playerScript != null && playerScript.isDead) yield break;

        isPlayingLandingAnimation = true;
        Play(Animations.JUMPEND, 0, true, true);
        yield return new WaitForSeconds(landingAnimationDuration);
        isPlayingLandingAnimation = false;
        SetLocked(false, 0);
    }

    public bool IsUsingSwordRun()
    {
        return GetComponent<Combos>().useSwordRun;
    }

    public void SpawnDust(Vector3 position, bool facingRight)
    {
        if (playerScript.currentSkill <= 0) return;
        if (dustPrefab != null)
        {
            GameObject dust = Instantiate(dustPrefab, position, Quaternion.identity);
            Vector3 scale = dust.transform.localScale;
            scale.x = facingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            dust.transform.localScale = scale;
            Destroy(dust, 1f);
        }
    }

    public void DefaultAnimation(int layer)
    {
        // WICHTIG: DefaultAnimation komplett deaktivieren wenn useSwordRun aktiv ist
        if (!GetComponent<Combos>().isAttacking && !IsUsingSwordRun())
        {
            CheckMovementAnimations(0);
        }
        else if (IsUsingSwordRun())
        {
            // Sword Run forcieren auch in DefaultAnimation
            animator.SetBool("useSwordRun", true);
            animator.Play("Run_Sword", layer, 0f);
        }
    }
}