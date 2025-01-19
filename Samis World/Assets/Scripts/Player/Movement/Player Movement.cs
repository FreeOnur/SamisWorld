using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private int playerSpeed;
    private float horizontalInput;
    private bool isFacingRight = true;
    private Rigidbody2D playerRb;

    private bool isGrounded = false;
    [SerializeField] private float jumpPower = 5f;
    
    private Animator animator;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

<<<<<<< Updated upstream
    private float idleTime = 0f;
    private bool isIdle = false;
=======
    public static PlayerMovement instance;
    //used for state machine
    private PlayerState currentState;
    private Rigidbody2D rb;
    private Animator animator;
    private float horizontalInput;
    private bool isFacingRight = true;
    private float layer = 0;
    private bool wasGrounded = true;

    private bool isPlayingLandingAnimation = false;
    private float landingAnimationDuration = 0.1f;


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

>>>>>>> Stashed changes
    private void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
<<<<<<< Updated upstream
    private bool IsGrounded()
=======

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
>>>>>>> Stashed changes
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
    void Update()
    {
        
        horizontalInput = Input.GetAxisRaw("Horizontal");
        jump(); //Handles the stuff for jumping
        Flip();
        
        waitingAnim(); //Handles the isIdle bool an uses it for the waiting animation
        
    }
    private void FixedUpdate()
    {
        movement();
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
<<<<<<< Updated upstream

    private bool idleCheck() => Mathf.Abs(playerRb.velocity.x) < 0.1f && Mathf.Abs(playerRb.velocity.y) < 0.1f && Mathf.Abs(horizontalInput) < 0.1f;
    private void movement()
=======
   
    //Checks which movement the player is doing
    private void CheckMovementAnimations(int layer)
>>>>>>> Stashed changes
    {
        playerRb.velocity = new Vector2(horizontalInput * playerSpeed, playerRb.velocity.y);
        animator.SetFloat("xVelocity", Math.Abs(playerRb.velocity.x));
        animator.SetFloat("yVelocity", playerRb.velocity.y);
    }
    private void jump()
    {
        // So that sliding is not possible:
        if (Mathf.Abs(horizontalInput) < 0.1f)
        {
<<<<<<< Updated upstream
            horizontalInput = 0f;
=======
            if (rb.velocity.y > 0)
            {
                Play(Animations.JUMP, layer, false, false);
            }
>>>>>>> Stashed changes
        }
        
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, jumpPower);

        }

        if (Input.GetKeyUp(KeyCode.Space) && playerRb.velocity.y > 0f)
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, playerRb.velocity.y * 0.5f);
        }
        animator.SetBool("isJumping", !IsGrounded());
    }
    private void waitingAnim()
    {
        //looks if the player is moving or doing anything
        if (idleCheck())
        {
            idleTime += Time.deltaTime;
            animator.SetFloat("waitingTime", idleTime);
            if (idleTime > 3)
            {
                animator.SetBool("isWaiting", true);
            }

        }
        else
        {
            idleTime = 0f;
            animator.SetBool("isWaiting", false);

        }
    }
<<<<<<< Updated upstream
=======

    void DefaultAnimation(int layer)
    {
        CheckMovementAnimations(0);
    }
    private void CheckFallAndLandAnimations()
    {
        bool isGrounded = IsGrounded();
        if (!wasGrounded && isGrounded)
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
>>>>>>> Stashed changes
}

