using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(EnemyStateMachine stateMachine, BaseEnemyController enemy) : base(stateMachine, enemy) { }

    public override void Enter()
    {
        Debug.Log("Entering Idle State");
        enemy.StopMovement();
    }

    public override void Update()
    {
        float distanceToTarget = enemy.GetDistanceToTarget();
        Debug.Log($"Idle State - Distance to target: {distanceToTarget}, Detection Range: {enemy.detectionRange}, Attack Range: {enemy.attackRange}");

        // Check if player is within detection range
        if (distanceToTarget <= enemy.detectionRange)
        {
            if (distanceToTarget <= enemy.attackRange && enemy.CanAttack())
            {
                Debug.Log("Switching to Attack State from Idle");
                stateMachine.ChangeState(enemy.attackState);
            }
            else
            {
                Debug.Log("Switching to Chase State from Idle");
                stateMachine.ChangeState(enemy.chaseState);
            }

            if (enemy.CheckIfDodge())
            {
                stateMachine.ChangeState(enemy.dodgeState);
            }
        }
    }
}