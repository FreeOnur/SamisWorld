using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class DashState : PlayerState
{
    [SerializeField] private float dashTime;
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f; // Duration of the dash
    [SerializeField] private bool isDashing;

    public DashState(PlayerMovement player) : base(player) { }
    public override void Enter()
    {
        dashTime = dashDuration;
        isDashing = true;
    }
    public override void HandleInput()
    {
        if (isDashing)
        {
            return;
        }

        if (Mathf.Abs(player.HorizontalInput) > 0.1f)
        {
            player.ChangeState(new RunState(player));
        }
        else if (Input.GetKeyDown(KeyCode.Space) && player.IsGrounded())
        {
            player.ChangeState(new JumpState(player));
        }
        else
        {
            player.ChangeState(new IdleState(player));
        }
    }
    public override void PhysicsUpdate()
    {
        if (isDashing)
        {
            rb.velocity = new Vector2(dashSpeed * rb.transform.localScale.x, rb.velocity.y);
            dashTime -= Time.fixedDeltaTime;

            if (dashTime <= 0)
            {
                isDashing = false;
            }
        }
        else
        {
            rb.velocity = new Vector2(player.PlayerSpeed * player.HorizontalInput, rb.velocity.y);
        }
    }
    public override void Exit()
    {
    }
}
