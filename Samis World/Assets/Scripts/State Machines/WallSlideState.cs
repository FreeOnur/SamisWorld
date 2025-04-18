using UnityEngine;

public class WallSlideState : PlayerState
{
    private float wallSlideSpeed;

    public WallSlideState(PlayerMovement player) : base(player) { }

    public override void Enter()
    {
        wallSlideSpeed = player.wallSlidingSpeed;
        player.isWallSliding = true;

    }

    public override void HandleInput()
    {
        // Wenn Spieler den Boden berührt → Idle oder Run
        if (player.IsGrounded())
        {
            if (Mathf.Abs(player.HorizontalInput) > 0.1f)
                player.ChangeState(new RunState(player));
            else
                player.ChangeState(new IdleState(player));
        }

        // Wenn nicht mehr an der Wand (z. B. keine Berührung) → JumpState (oder FallState wenn du hast)
        if (!player.IsTouchingWall() && !player.IsGrounded())
        {
            player.ChangeState(new JumpState(player));
        }
    }

    public override void PhysicsUpdate()
    {
        rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
    }

    public override void Exit()
    {
        player.isWallSliding = false;
    }
}
