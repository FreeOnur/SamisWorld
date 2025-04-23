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

    Seeker seeker;
    Rigidbody2D rb;
    void Start()
    {
        seeker = GetComponent<Seeker>();  
        rb = GetComponent<Rigidbody2D>();
        UpdatePath();
        InvokeRepeating("UpdatePath", 0f, 1f);

    }

    void UpdatePath()
    {
        if (target != null && seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWayPoint = 0;
        }

    }

    // Update is called once per frame
    void FixedUpdate()
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
        rb.velocity = direction * speed * Time.deltaTime;

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWayPoint]);
        if (distance < nextWaypointDistance)
        {
            currentWayPoint++;
        }
    }

}
