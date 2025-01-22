using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerMovement : AnimatorBrain
{

    [SerializeField] private int playerSpeed;
    [SerializeField] private float jumpPower = 5f;
    //variables vor the raycast to check if it hits the ground
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    public static PlayerMovement instance;
    //used for state machine
    private PlayerState currentState;
    private Rigidbody2D rb;
    private Animator animator;
    private DashState dashState;
    private float horizontalInput;
    private bool isFacingRight = true;
    private float layer = 0;
    private bool wasGrounded = true;

    private bool isPlayingLandingAnimation = false;
    private float landingAnimationDuration = 0.05f;

    [SerializeField] private GameObject dustPrefab;


    // Properties to access private fields
    public Rigidbody2D PlayerRb => rb;
    public Animator Anim => animator;
    public float HorizontalInput => horizontalInput;
    public int PlayerSpeed => playerSpeed;
    public float JumpPower => jumpPower;

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
        horizontalInput = Input.GetAxisRaw("Horizontal");
        currentState?.HandleInput();
        Flip();
        CheckMovementAnimations(0);
        CheckFallAndLandAnimations();
    }

    private void FixedUpdate()
    {
        currentState?.PhysicsUpdate();
    }
    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
    public void ChangeState(PlayerState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    //Is flipping the 2d character based on which side he is going
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

   
    //Checks which movement the player is doing
    private void CheckMovementAnimations(int layer)
    {
        if (isPlayingLandingAnimation) return;
        if(Input.GetKeyDown(KeyCode.F))
        {
            Play(Animations.DASH, layer,false,false);
        }
        if (!IsGrounded())
        {
            if (rb.velocity.y > 0)
            {
                Play(Animations.JUMP, layer, false, false);
            }
            
        }
        else if (Mathf.Abs(PlayerRb.velocity.x) > 0.1f && Mathf.Abs(horizontalInput) > 0.1f)
        {
            Play(Animations.RUN, layer, false, false);
        }else
        {
            Play(Animations.IDLE, layer, false, false);
        } 
    }

    
    void DefaultAnimation(int layer)
    {
        CheckMovementAnimations(0);
    }
    void CheckJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            Play(Animations.JUMP, 0, true, false);
        }
    }
    private void CheckFallAndLandAnimations()
    {
        bool isGrounded = IsGrounded();
        if (!wasGrounded && isGrounded && !isPlayingLandingAnimation)
        {
            StartCoroutine(PlayLandingAnimation());
        }

        // Play falling animation when not grounded and moving downward
        else if (!isGrounded && rb.velocity.y < 0)
        {
            Play(Animations.FALL, 0, false, false);
        }
        
        wasGrounded = isGrounded; // Update the grounded state
    }

    private IEnumerator PlayLandingAnimation()
    {
        isPlayingLandingAnimation = true;
        Play(Animations.JUMPEND, 0, true, true); // Lock the animation and bypass other locks

        // Wait for the landing animation duration
        yield return new WaitForSeconds(landingAnimationDuration);

        isPlayingLandingAnimation = false;
        SetLocked(false, 0); // Unlock the animation layer
    }

    // For the dash Function
    public void SpawnDust(Vector3 position, bool facingRight)
    {
        if (dustPrefab != null)
        {
            GameObject dust = Instantiate(dustPrefab, position, Quaternion.identity);
            Vector3 scale = dust.transform.localScale;
            scale.x = facingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x); // Flip on the X-axis
            dust.transform.localScale = scale;
            Destroy(dust, 1f);
        }
    }
}
