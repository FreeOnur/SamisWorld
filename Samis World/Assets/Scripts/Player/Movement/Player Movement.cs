using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private int playerSpeed;
    [SerializeField] private float jumpPower = 5f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    private PlayerState currentState;
    private Rigidbody2D rb;
    private Animator animator;
    private float horizontalInput;
    private bool isFacingRight = true;

    // Properties to access private fields
    public Rigidbody2D PlayerRb => rb;
    public Animator Anim => animator;
    public float HorizontalInput => horizontalInput;
    public int PlayerSpeed => playerSpeed;
    public float JumpPower => jumpPower;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        ChangeState(new IdleState(this));
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        currentState?.HandleInput();
        Flip();
    }

    private void FixedUpdate()
    {
        currentState?.PhysicsUpdate();
    }

    public void ChangeState(PlayerState newState)
    {
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

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
}
