using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class DashState : PlayerState
{
    [SerializeField] private float dashTime;
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f; // Duration of the dash
    public bool isDashing;
    [SerializeField] private GameObject dustPrefab;
    private Vector3 position;

    public DashState(PlayerMovement player) : base(player) { }
    public override void Enter()
    {
        position = new Vector3(player.transform.position.x, player.transform.position.y - 0.2f);
        dashTime = dashDuration;
        isDashing = true;
        bool facingRight = player.transform.localScale.x > 0;
        player.SpawnDust(new Vector3(player.transform.position.x, player.transform.position.y - 0.2f), facingRight);

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