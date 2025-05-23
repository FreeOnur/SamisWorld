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
    protected virtual void Start()
    {
        seeker = GetComponent<Seeker>();  
        rb = GetComponent<Rigidbody2D>();
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

    // Update is called once per frame
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
