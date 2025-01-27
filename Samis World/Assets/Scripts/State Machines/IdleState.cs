using Unity.Burst.Intrinsics;
using UnityEngine;

public class IdleState : PlayerState
{
    private Combos combos;

    public IdleState(PlayerMovement player) : base(player) { }

    public override void Enter()
    {
        combos = player.GetComponent<Combos>();

    }

    //switches states based on conditions
    public override void HandleInput()
    {
        if (Mathf.Abs(player.HorizontalInput) > 0.1f)
        {
            player.ChangeState(new RunState(player));
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) && player.IsGrounded())
        {
            player.ChangeState(new JumpState(player));
            return;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            player.ChangeState(new DashState(player));
            return;
        }
        if (Input.GetKeyDown(KeyCode.Mouse0) && !combos.atacando)
        {
            combos.Comboss();
        }
    }

    public override void PhysicsUpdate()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    public override void Exit()
    {
    }
}