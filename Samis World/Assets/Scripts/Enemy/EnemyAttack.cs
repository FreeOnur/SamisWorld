using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float attackRange;
    [SerializeField] private float attackCooldown;
    private float lastAttackTime;
    [SerializeField] private float damageAmount;

    [SerializeField] private Transform attackTransform;
    [SerializeField] private LayerMask playerLayerMask;
    private Collider2D hit;
    void Start()
    {
        
    }
    public void TryAttack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }
    private void Attack()
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
