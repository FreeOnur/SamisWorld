using UnityEngine;

public class PlayerState
{
    protected PlayerMovement player;
    protected Rigidbody2D rb;
    protected Animator animator;
    protected AnimatorBrain animatorBrain;
    protected PlayerData playerData;
    protected Player playerScript;
    public PlayerState(PlayerMovement player)
    {
        this.playerScript = player.playerScript;
        this.player = player;
        this.rb = player.PlayerRb;
        this.animator = player.Anim; 
        this.playerData = player.playerData;
    }
    public virtual void Enter()
    {

    }
    public virtual void HandleInput()
    {

    }
    public virtual void PhysicsUpdate()
    {

    }
    public virtual void Exit()
    {

    }
}
