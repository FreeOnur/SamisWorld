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
        this.rb = player.PlayerRb;
        this.animator = player.Anim; 
    }

    public abstract void Enter();
    public abstract void HandleInput();
    public abstract void PhysicsUpdate();
    public abstract void Exit();
}
