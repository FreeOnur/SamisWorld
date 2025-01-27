using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class JumpState : PlayerState
{
    private Combos combos;
    public JumpState(PlayerMovement player) : base(player) { }
    
    public override void Enter()
    {
        combos = player.GetComponent<Combos>();
        player.isJumping = true;
        rb.velocity = new Vector2(rb.velocity.x, playerData.JumpPower);
    }
    //switches states based on conditions
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

        if (Input.GetKeyDown(KeyCode.F))
        {
            player.ChangeState(new DashState(player));
            return;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            rb.gravityScale = playerData.GravityScale * playerData.FastFallGravityMult;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -playerData.MaxFallSpeed));
        }
        if (Input.GetKeyDown(KeyCode.Mouse0) && !combos.atacando)
        {
            combos.Comboss();
        }

    }

    public override void PhysicsUpdate()
    {
        rb.velocity = new Vector2(player.HorizontalInput * playerData.PlayerSpeed, rb.velocity.y);
        if(rb.velocity.y > 0f)
        {
            rb.gravityScale = playerData.GravityScale * playerData.GravityMultiplier;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -playerData.MaxFallSpeed));
        }

        if(player.isJumping && Mathf.Abs(rb.velocity.y)< player.jumpHangTimeThreshold)
        {
            rb.gravityScale = playerData.GravityScale * playerData.jumpHangGravityMultiplier;


        }        
    }

    public override void Exit()
    {
        player.isJumping = false;
    }
}