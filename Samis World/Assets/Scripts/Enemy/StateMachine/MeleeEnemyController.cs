using UnityEngine;

public class MeleeEnemyController : BaseEnemyController
{
    [Header("Melee Attack")]
    public Transform attackTransform; // Attack point in front of enemy

    protected override EnemyAttackState CreateAttackState()
    {
        return new MeleeAttackState(stateMachine, this);
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        // Melee attack range
        if (attackTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackTransform.position, attackRange);
        }
    }
}
