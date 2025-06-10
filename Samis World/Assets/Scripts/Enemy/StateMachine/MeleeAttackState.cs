using UnityEngine;

public class MeleeAttackState : EnemyAttackState
{
    private MeleeEnemyController meleeEnemy;

    public MeleeAttackState(EnemyStateMachine stateMachine, BaseEnemyController enemy) : base(stateMachine, enemy)
    {
        meleeEnemy = enemy as MeleeEnemyController;
    }

    public override void Enter()
    {
        Debug.Log("Entering MeleeAttackState!");
        base.Enter();
    }

    public override void Update()
    {
        Debug.Log($"MeleeAttackState Update - Timer: {attackTimer}, Cooldown: {enemy.attackCooldown}");
        base.Update();
    }

    protected override void PerformAttack()
    {
        Debug.Log("PerformAttack called!");

        if (meleeEnemy?.attackTransform == null)
        {
            Debug.LogError("Attack Transform is null!");
            return;
        }

        Debug.Log($"Checking attack at position: {meleeEnemy.attackTransform.position}, Range: {enemy.attackRange}");

        Collider2D hit = Physics2D.OverlapCircle(meleeEnemy.attackTransform.position, enemy.attackRange, enemy.playerLayerMask);

        if (hit != null)
        {
            Debug.Log($"Hit detected: {hit.name}");
            IDamagable damageable = hit.GetComponent<IDamagable>();
            if (damageable != null)
            {
                Debug.Log($"Dealing {enemy.damageAmount} damage!");
                damageable.Damage(enemy.damageAmount);
            }
            else
            {
                Debug.LogError("No IDamagable component found on " + hit.name);
            }
        }
        else
        {
            Debug.Log("No hit detected in attack range!");
        }
    }
}