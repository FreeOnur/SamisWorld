using UnityEngine;

public abstract class PlayerState
{
    protected PlayerMovement player;
    protected Rigidbody2D rb;
    protected Animator animator;
    protected AnimatorBrain animatorBrain;

    public PlayerState(PlayerMovement player)
    {
        this.player = player;
        this.rb = player.PlayerRb;  // You'll need to add this property
        this.animator = player.Anim;  // You'll need to add this property
    }
    public abstract void Enter();
    public abstract void HandleInput();
    public abstract void PhysicsUpdate();
    public abstract void Exit();
}
