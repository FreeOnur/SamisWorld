using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class RunState : PlayerState
{
    public RunState(PlayerMovement player) : base(player) { }
    private Combos combos;

    public override void Enter()
    {
        combos = player.GetComponent<Combos>();
    }
    //switches states based on conditions
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

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (playerScript.currentSkill <= 0) return;
            bool facingRight = player.transform.localScale.x > 0;
            player.SpawnDust(new Vector3(player.transform.position.x, player.transform.position.y - 0.2f), facingRight);
            player.ChangeState(new DashState(player));
            return;
        }
        if (Input.GetKeyDown(KeyCode.Mouse0) && !combos.isAttacking)
        {
            combos.Comboss();
        }

    }

    public override void PhysicsUpdate()
    {
        rb.velocity = new Vector2(player.HorizontalInput * playerData.PlayerSpeed, rb.velocity.y);
        
    }

    public override void Exit() { }
}