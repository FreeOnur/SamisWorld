using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerMovement : AnimatorBrain
{
    // Existierende Variablen bleiben unverändert

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

    // Neue Variable für Ledge-Jump-Kraft
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
            return; // Wichtig: Early Return verhindert alle anderen Inputs während des Hängens
        }

        // Spieler-Input einholen
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Aktuellen Zustand behandeln
        currentState?.HandleInput();

        // Spieler drehen, wenn nötig
        Flip();

        // Bewegung und Animation prüfen
        if (!(currentState is DashState)) // DashState soll Animationen exklusiv steuern
        {
            CheckMovementAnimations(0);
        }

        CheckFallAndLandAnimations();
        CheckForLedge();
        WallState();
    }

    private void FixedUpdate()
    {
        // Physik-Updates des aktuellen Zustands
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

        // Kraftvollen Jump ausführen
        Vector2 jumpForce = new Vector2(isFacingRight ? -ledgeJumpHorizontalForce : ledgeJumpHorizontalForce, ledgeJumpForce);
        rb.velocity = jumpForce;
        Play(Animations.JUMP, 0, false, false);
        StartCoroutine(ResetLedgeGrab(0.5f));

        // Ledge-Status zurücksetzen
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
        return Physics2D.OverlapCircle(wallCheck.position,0.2f, wallLayer);
    }
    public bool IsTouchingWall()
    {
        // dein eigener Wand-Raycast oder Collider check
        return IsWalled() && !IsGrounded(); // falls du sowas wie `isWalled` hast
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(wallCheck.position, 0.2f);
    }

    public virtual void ChangeState(PlayerState newState)
    {
        if (isHangingOnLedge) return;

        // Alten Zustand beenden und neuen Zustand starten
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

        if (playerScript != null && playerScript.isDead)
        {
            animator.SetTrigger("isDead");
            this.enabled = false;
            return; // Verhindert, dass andere Animationen abgespielt werden
        }

        if (isPlayingLandingAnimation) return;

        if (!IsGrounded())
        {
            if (rb.velocity.y > 0)
            {
                Play(Animations.JUMP, layer, false, false);
            }
        }
        else if (Mathf.Abs(rb.velocity.x) > 0.1f && Mathf.Abs(horizontalInput) > 0.1f)
        {
            Play(Animations.RUN, layer, false, false);
        }
        else
        {
            Play(Animations.IDLE, layer, false, false);
        }
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
        if (playerScript != null && playerScript.isDead) yield break; // Don't play landing animation if dead

        isPlayingLandingAnimation = true;
        Play(Animations.JUMPEND, 0, true, true); // Play landing animation

        yield return new WaitForSeconds(landingAnimationDuration);

        isPlayingLandingAnimation = false;
        SetLocked(false, 0); // Unlock animation
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
        CheckMovementAnimations(0);
    }
}