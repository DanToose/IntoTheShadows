using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavmeshAgentScript : MonoBehaviour
{

    public Transform target;
    NavMeshAgent agent;
    public GameObject[] wayPoints;

    private Transform currentDestination;
    private int PatrolPoint;
    private int PatrolPointCount;
    private float dist;
    private float seenDist;
    public int AIState;

    public float patrolSpeed;
    public float chaseSpeed;

    public Vector3 guardPosition;
    public Vector3 initialFacingDirection;
    public float sightRange;
    public bool inLoS; // NOT USED? Delete?
    private bool hadChased;
    public Vector3 lastSeenAt;
    public float delay = 3f;
    public float patrolCheckRange;

    // This enemy uses an integer to flag the AI state:

    // 1 = Head to the player and raycast to check LOS again
    // 2 = Head to player's last know location.
    // 3 = Patrol

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("PlayerBody").transform;
        PatrolPoint = 0;
        PatrolPointCount = wayPoints.Length;
        if (patrolCheckRange == 0)
        {
            patrolCheckRange = 0.5f;
        }
        initialFacingDirection = transform.localEulerAngles;
    }

    void DelayedSwitch()
    {
        // This returns the guard to its patrol.
        AIState = 3;
    }

    // Update is called once per frame
    void Update()
    {
        guardPosition = transform.position;

        if (AIState == 1)
        {
            agent.speed = chaseSpeed;
            agent.SetDestination(target.position);
            lastSeenAt = target.transform.position;
        }

        if (AIState == 2) // HEAD TO LAST PLACE PLAYER WAS SEEN 
        {
            agent.speed = chaseSpeed;
            seenDist = Vector3.Distance(lastSeenAt, guardPosition);
            if (seenDist > 2.0)
            {
                agent.SetDestination(lastSeenAt);
                Debug.Log("lastSeenAt = " + lastSeenAt + " seenDist = " + seenDist);
            }
            else if (seenDist <= 2.0)
            {
                Invoke("DelayedSwitch", delay);
                seenDist = 100;
            }
        }

        if (AIState == 3) // ON PATROL -- THIS ALL WORKS AS DESIRED. 
        {
            agent.speed = patrolSpeed;
            if (wayPoints.Length > 0)
            {
                currentDestination = wayPoints[PatrolPoint].transform;
                dist = Vector3.Distance(currentDestination.position, transform.position);
                //Debug.Log("No of points: " + PatrolPointCount + " Current: " + PatrolPoint);

                if (dist > patrolCheckRange)
                {
                    agent.SetDestination(currentDestination.position);
                }
                else if (dist <= patrolCheckRange && PatrolPoint == (PatrolPointCount - 1))
                {
                    PatrolPoint = 0;
                }

                else if (dist <= patrolCheckRange && PatrolPoint < (PatrolPointCount - 1))
                {
                    PatrolPoint++;

                }
            }

            if (wayPoints.Length == 1)
            {
                //Debug.Log(initialFacingDirection);
                currentDestination = wayPoints[PatrolPoint].transform;
                dist = Vector3.Distance(currentDestination.position, transform.position);
                if (dist <= patrolCheckRange)
                {
                    transform.localEulerAngles = initialFacingDirection;
                    //Debug.Log(transform.localEulerAngles + " and " + initialFacingDirection);
                }
            }
        }
    }
}