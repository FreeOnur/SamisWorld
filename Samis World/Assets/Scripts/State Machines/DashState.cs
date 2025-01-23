using UnityEngine;

public class DashState : PlayerState
{
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f; // Dauer des Dashes
    private float dashTime;
    private bool isDashing;

    public DashState(PlayerMovement player) : base(player) { }

    public override void Enter()
    {
        dashTime = dashDuration;
        isDashing = true;

        // Starte die Dash-Animation
        player.Play(Animations.DASH, 0, true, false);

        // Erstelle Staub-Effekt (optional)
        bool facingRight = player.transform.localScale.x > 0;
        player.SpawnDust(new Vector3(player.transform.position.x, player.transform.position.y - 0.2f), facingRight);
    }

    public override void HandleInput()
    {
        if (isDashing) return;

        // Wechsel in andere Zustände basierend auf den Eingaben
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
            player.PlayerRb.velocity = new Vector2(dashSpeed * Mathf.Sign(player.transform.localScale.x), player.PlayerRb.velocity.y);

            dashTime -= Time.fixedDeltaTime;

            if (dashTime <= 0)
            {
                isDashing = false;

                // Beende die Dash-Animation
                player.SetLocked(false, 0); // Animation entsperren
                HandleInput();
            }
        }
        else
        {
            player.PlayerRb.velocity = new Vector2(player.PlayerSpeed * player.HorizontalInput, player.PlayerRb.velocity.y);
        }
    }

    public override void Exit()
    {
        // Rücksetzen oder Übergang vorbereiten
        isDashing = false;
    }
}
