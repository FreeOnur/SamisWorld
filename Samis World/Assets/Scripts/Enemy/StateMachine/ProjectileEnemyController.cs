using UnityEngine;

public class ProjectileEnemyController : BaseEnemyController
{
    [Header("Projectile Attack")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 5f;

    protected override EnemyAttackState CreateAttackState()
    {
        return new ProjectileAttackState(stateMachine, this);
    }
}
