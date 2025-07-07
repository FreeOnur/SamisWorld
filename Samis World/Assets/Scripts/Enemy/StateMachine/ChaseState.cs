using UnityEngine;

public class EnemyChaseState : EnemyState
{
    public EnemyChaseState(EnemyStateMachine stateMachine, BaseEnemyController enemy) : base(stateMachine, enemy) { }

    public override void Enter()
    {
        Debug.Log("Entering Chase State");
    }

    public override void Update()
    {
        float distanceToTarget = enemy.GetDistanceToTarget();
        Debug.Log($"Chase State - Distance: {distanceToTarget}, Attack Range: {enemy.attackRange}, Can Attack: {enemy.CanAttack()}");

        // Check if we should attack
        if (distanceToTarget <= enemy.attackRange && enemy.CanAttack())
        {
            Debug.Log("Switching to Attack State from Chase");
            stateMachine.ChangeState(enemy.attackState);
            return;
        }
        else if (enemy.CheckIfDodge())
        {
            stateMachine.ChangeState(enemy.dodgeState);
        }
    }
    public override void FixedUpdate()
    {
        if (enemy.path == null || enemy.target == null) return;

        float targetDistance = Vector2.Distance(enemy.rb.position, enemy.target.position);

        // Stop if we're close enough to attack
        if (targetDistance < enemy.stopDistance)
        {
            enemy.StopMovement();
            return;
        }

        if (enemy.currentWayPoint >= enemy.path.vectorPath.Count)
        {
            enemy.reachedEndOfPath = true;
            return;
        }
        else
        {
            enemy.reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)enemy.path.vectorPath[enemy.currentWayPoint] - enemy.rb.position).normalized;
        Vector2 horizontalDirection = new Vector2(direction.x, 0).normalized;

        // Flip the sprite based on horizontal direction
        enemy.Flip(horizontalDirection.x);

        // Move horizontally
        enemy.rb.velocity = new Vector2(horizontalDirection.x * enemy.speed * Time.deltaTime, enemy.rb.velocity.y);

        // Jump if needed
        if (direction.y > 0.5f && enemy.IsGrounded())
        {
            enemy.rb.AddForce(Vector2.up * enemy.jumpForce, ForceMode2D.Impulse);
        }

        // Check if we reached current waypoint
        float distance = Vector2.Distance(enemy.rb.position, enemy.path.vectorPath[enemy.currentWayPoint]);
        if (distance < enemy.nextWaypointDistance)
        {
            enemy.currentWayPoint++;
        }
    }
}