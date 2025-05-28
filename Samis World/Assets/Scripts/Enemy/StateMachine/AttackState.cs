using UnityEngine;

public abstract class EnemyAttackState : EnemyState
{
    protected float attackTimer;

    public EnemyAttackState(EnemyStateMachine stateMachine, BaseEnemyController enemy) : base(stateMachine, enemy) { }

    public override void Enter()
    {
        enemy.StopMovement();
        attackTimer = 0f;
    }

    public override void Update()
    {
        attackTimer += Time.deltaTime;

        // Perform attack
        if (attackTimer >= enemy.attackCooldown)
        {
            PerformAttack();
            enemy.lastAttackTime = Time.time;
            attackTimer = 0f;
        }

        // Check if we should continue attacking or change state
        float distanceToTarget = enemy.GetDistanceToTarget();

        if (distanceToTarget > enemy.attackRange)
        {
            if (distanceToTarget <= enemy.detectionRange)
            {
                stateMachine.ChangeState(enemy.chaseState);
            }
            else
            {
                stateMachine.ChangeState(enemy.idleState);
            }
            if (enemy.CheckIfDodge())
            {
                stateMachine.ChangeState(enemy.dodgeState);
            }
        }
    }

    protected abstract void PerformAttack();
}