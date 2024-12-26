using UnityEngine;

public class IdleState : PlayerState
{
    private float idleTime = 0f;

    public IdleState(PlayerMovement player) : base(player) { }

    public override void Enter()
    {
        animator.SetFloat("xVelocity", 0f);
        animator.SetBool("isJumping", false);
        idleTime = 0f;
    }

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

        // Handle waiting animation
        if (Mathf.Abs(rb.velocity.x) < 0.1f && Mathf.Abs(rb.velocity.y) < 0.1f)
        {
            idleTime += Time.deltaTime;
            animator.SetFloat("waitingTime", idleTime);
            if (idleTime > 3)
            {
                animator.SetBool("isWaiting", true);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    public override void Exit()
    {
        animator.SetBool("isWaiting", false);
    }
}