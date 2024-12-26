using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunState : PlayerState
{
    public RunState(PlayerMovement player) : base(player) { }

    public override void Enter()
    {
        animator.SetBool("isWaiting", false);
    }

    public override void HandleInput()
    {
        if (Mathf.Abs(player.HorizontalInput) < 0.1f)
        {
            player.ChangeState(new IdleState(player));
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) && player.IsGrounded())
        {
            player.ChangeState(new JumpState(player));
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        rb.velocity = new Vector2(player.HorizontalInput * player.PlayerSpeed, rb.velocity.y);
        animator.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));
    }

    public override void Exit() { }
}