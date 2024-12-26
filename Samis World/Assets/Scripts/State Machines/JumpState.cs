using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : PlayerState
{
    public JumpState(PlayerMovement player) : base(player) { }

    public override void Enter()
    {
        animator.SetBool("isJumping", true);
        rb.velocity = new Vector2(rb.velocity.x, player.JumpPower);
    }

    public override void HandleInput()
    {
        if (Input.GetKeyUp(KeyCode.Space) && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        if (player.IsGrounded() && rb.velocity.y <= 0)
        {
            if (Mathf.Abs(player.HorizontalInput) > 0.1f)
                player.ChangeState(new RunState(player));
            else
                player.ChangeState(new IdleState(player));
        }
    }

    public override void PhysicsUpdate()
    {
        rb.velocity = new Vector2(player.HorizontalInput * player.PlayerSpeed, rb.velocity.y);
        animator.SetFloat("yVelocity", rb.velocity.y);
    }

    public override void Exit()
    {
        animator.SetBool("isJumping", false);
    }
}