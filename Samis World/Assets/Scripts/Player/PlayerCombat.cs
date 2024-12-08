using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private Transform attackPoint;

    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private int attackDamage = 40;
    void Update()
    {
         
    }

    public void Attack(int attackPower)
    {
        //Play an attack animation
        switch (attackPower)
        {
            case 1:
                animator.SetFloat("attackPower", 1f);
                animator.SetTrigger("Attack");
                break;
        }

        //Detect enemies in range of attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        //Damage them
        foreach(Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if(attackPoint ==null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
