using UnityEngine;

public class ProjectileAttackState : EnemyAttackState
{
    private ProjectileEnemyController projectileEnemy;

    public ProjectileAttackState(EnemyStateMachine stateMachine, BaseEnemyController enemy) : base(stateMachine, enemy)
    {
        projectileEnemy = enemy as ProjectileEnemyController;
    }

    protected override void PerformAttack()
    {
        if (projectileEnemy?.projectilePrefab == null || projectileEnemy?.firePoint == null || enemy.target == null)
            return;

        Vector2 direction = (enemy.target.position - projectileEnemy.firePoint.position).normalized;
        GameObject projectile = Object.Instantiate(projectileEnemy.projectilePrefab, projectileEnemy.firePoint.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * projectileEnemy.projectileSpeed;
        }
    }
}
