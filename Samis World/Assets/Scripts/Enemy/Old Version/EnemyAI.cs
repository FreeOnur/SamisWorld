using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    public Transform target;

    public float speed = 200f;
    public float nextWaypointDistance = 2f;
    int currentWayPoint = 0;
    Path path;
    bool reachedEndOfPath = false;
    [SerializeField] float stopDistance = 2f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpForce = 10f;
    Seeker seeker;
    Rigidbody2D rb;
    private float facingDirection = 1f; // Track facing direction (1 for right, -1 for left)

    protected virtual void Awake()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        UpdatePath();
        InvokeRepeating("UpdatePath", 0f, 1f);
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    protected virtual void UpdatePath()
    {
        if (target != null && seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    protected virtual void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWayPoint = 0;
        }
    }

    // Flip the entire GameObject based on direction
    private void Flip(float directionX)
    {
        if (directionX > 0 && facingDirection != 1f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f); // Face right
            facingDirection = 1f;
        }
        else if (directionX < 0 && facingDirection != -1f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f); // Face left
            facingDirection = -1f;
        }
    }

    protected virtual void FixedUpdate()
    {
        if (path == null || target == null) return;

        float targetDistance = Vector2.Distance(rb.position, target.position);

        if (targetDistance < stopDistance)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (currentWayPoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWayPoint] - rb.position).normalized;
        Vector2 horizontalDirection = new Vector2(direction.x, 0).normalized;

        // Flip the sprite based on horizontal direction
        Flip(horizontalDirection.x);

        rb.velocity = new Vector2(horizontalDirection.x * speed * Time.deltaTime, rb.velocity.y);

        if (direction.y > 0.5f && IsGrounded())
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWayPoint]);
        if (distance < nextWaypointDistance)
        {
            currentWayPoint++;
        }
    }
}