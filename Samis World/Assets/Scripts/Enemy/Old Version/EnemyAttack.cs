using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] protected float attackRange = 1f;
    [SerializeField] protected float attackCooldown = 2f;
    private float lastAttackTime;
    [SerializeField] protected float damageAmount = 10f;

    [SerializeField] protected Transform attackTransform; // z.B. leerer GameObject vor dem Gegner
    [SerializeField] protected LayerMask playerLayerMask;

    private Collider2D hit;

    void Update()
    {
        TryAttack();
    }

    public virtual void TryAttack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    protected virtual void Attack()
    {
        // Jetzt wird vom Attack Point geprüft
        hit = Physics2D.OverlapCircle(attackTransform.position, attackRange, playerLayerMask);

        if (hit != null)
        {
            IDamagable iDamagable = hit.GetComponent<IDamagable>();

            if (iDamagable != null)
            {
                iDamagable.Damage(damageAmount);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Damit du im Editor sehen kannst, wo der Attack Point wirkt
        if (attackTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackTransform.position, attackRange);
        }
    }
}
