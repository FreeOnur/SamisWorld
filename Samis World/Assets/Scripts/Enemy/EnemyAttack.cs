using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] protected float attackRange;
    [SerializeField] protected float attackCooldown;
    private float lastAttackTime;
    [SerializeField] protected float damageAmount;

    [SerializeField] protected Transform attackTransform;
    [SerializeField] protected LayerMask playerLayerMask;
    private Collider2D hit;
    void Start()
    {
        
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
        hit = Physics2D.OverlapCircle(transform.position, attackRange, playerLayerMask);

        if(hit != null)
        {
            IDamagable iDamagable = hit.GetComponent<IDamagable>();

            if (iDamagable != null)
            {
                iDamagable.Damage(damageAmount);
            }
        }
        
    }
    // Update is called once per frame
    void Update()
    {
        TryAttack();
    }
}
