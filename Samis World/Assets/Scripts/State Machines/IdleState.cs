using UnityEngine;

public class IdleState : PlayerState
{
    private float idleTime = 0f;

    public IdleState(PlayerMovement player) : base(player) { }

    public override void Enter()
    {
        
        idleTime = 0f;
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
    }

    public override void PhysicsUpdate()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    public override void Exit()
    {
    }
}