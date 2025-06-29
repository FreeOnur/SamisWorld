using UnityEngine;

public class EnemyDodgeState : EnemyState
{
    private float endDodgeForce = 5f;
    private float endDodgeTime = 0.2f;
    private bool dodgeEnded;
    private bool dodgeStarted;
    private float dodgeRequestTime = -1f;

    public EnemyDodgeState(EnemyStateMachine stateMachine, BaseEnemyController enemy) : base(stateMachine, enemy) { }

    public override void Enter()
    {
        base.Enter();
        enemy.StopMovement();
        dodgeEnded = false;
        dodgeStarted = false;
        dodgeRequestTime = Time.time;
        Debug.Log("Entering Dodge State");
    }

    public override void Exit()
    {
        base.Exit();
        enemy.lastDodgeTime = Time.time; // Setze den Dodge-Cooldown
        Debug.Log("Exiting Dodge State");
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    private void StartDodge()
    {
        if (!dodgeStarted)
        {
            enemy.rb.velocity = new Vector2(enemy.dodgeAngle.x * -enemy.facingDirection, enemy.dodgeAngle.y) * enemy.dodgeForce;
            dodgeStarted = true;
            Debug.Log("Dodge started with velocity: " + enemy.rb.velocity);
        }
    }

    private void EndDodge()
    {
        if (!dodgeEnded)
        {
            enemy.rb.velocity = new Vector2(0.1f * -enemy.facingDirection, -1f) * endDodgeForce;
            dodgeEnded = true;
            Debug.Log("Dodge ended with velocity: " + enemy.rb.velocity);
        }
    }

    private void SwitchToNextState()
    {
        float distanceToTarget = enemy.GetDistanceToTarget();
        if (distanceToTarget <= enemy.detectionRange)
        {
            if (distanceToTarget <= enemy.attackRange && enemy.CanAttack())
            {
                Debug.Log("Switching to Attack State from Dodge");
                stateMachine.ChangeState(enemy.attackState);
            }
            else
            {
                Debug.Log("Switching to Chase State from Dodge");
                stateMachine.ChangeState(enemy.chaseState);
            }
        }
        else
        {
            Debug.Log("Switching to Idle State from Dodge");
            stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void Update()
    {
        base.Update();

        // 1. Dodge verzögert starten
        if (dodgeRequestTime >= 0 && !dodgeStarted && Time.time >= dodgeRequestTime + 0.05f)
        {
            StartDodge();
        }

        // 2. Dodge beenden nach endDodgeTime
        if (dodgeStarted && !dodgeEnded && Time.time >= dodgeRequestTime + endDodgeTime)
        {
            EndDodge();
        }

        // 3. Zustandswechsel nach Dodge-Ende
        if (dodgeEnded)
        {
            SwitchToNextState();
        }
    }
}