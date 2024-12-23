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

    private float idleTime = 0f;
    private bool isIdle = false;
    private void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
    void Update()
    {
        
        horizontalInput = Input.GetAxisRaw("Horizontal");
        jump(); //Handles the stuff for jumping
        Flip();
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

    private bool idleCheck() => Mathf.Abs(playerRb.velocity.x) < 0.1f && Mathf.Abs(playerRb.velocity.y) < 0.1f && Mathf.Abs(horizontalInput) < 0.1f;
    private void movement()
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
            horizontalInput = 0f;
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
}

