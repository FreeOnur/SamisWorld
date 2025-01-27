using System.Collections;
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
    private float horizontalInput;
    private bool isFacingRight = true;
    private bool wasGrounded = true;
    private bool isPlayingLandingAnimation = false;
    private float landingAnimationDuration = 0.05f;
    public bool isJumping;
    public float jumpHangTimeThreshold = 4f;
    
    public PlayerData playerData;
    

    // Eigenschaften für den Zugriff auf private Felder
    public Rigidbody2D PlayerRb => rb;
    public Animator Anim => animator;
    public float HorizontalInput => horizontalInput;
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
    }

    private void FixedUpdate()
    {
        // Physik-Updates des aktuellen Zustands
        currentState?.PhysicsUpdate();
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    public void ChangeState(PlayerState newState)
    {
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
        isPlayingLandingAnimation = true;
        Play(Animations.JUMPEND, 0, true, true); // Animation sperren und abspielen

        yield return new WaitForSeconds(landingAnimationDuration);

        isPlayingLandingAnimation = false;
        SetLocked(false, 0); // Animation entsperren
    }

    public void SpawnDust(Vector3 position, bool facingRight)
    {
        if (dustPrefab != null)
        {
            GameObject dust = Instantiate(dustPrefab, position, Quaternion.identity);
            Vector3 scale = dust.transform.localScale;
            scale.x = facingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            dust.transform.localScale = scale;
            Destroy(dust, 1f);
        }
    }

    void DefaultAnimation(int layer)
    {
        CheckMovementAnimations(0);
    }
}
