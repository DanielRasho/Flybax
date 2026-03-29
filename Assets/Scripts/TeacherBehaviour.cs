using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class TeacherBehaviour : MonoBehaviour
{
    private enum EnemyState { Patrol, Chase }

    [Header("Target")]
    [SerializeField] private Transform ball;

    [Header("Patrol")]
    [SerializeField] private List<Transform> waypoints;
    [SerializeField] private float waitTime = 3.0f;

    [Header("Speeds")]
    [SerializeField] private float patrolSpeed = 6f;
    [SerializeField] private float chaseSpeed = 8f;

    [Header("Vision")]
    [SerializeField] private float viewRadius = 10.0f;
    [SerializeField] private float viewAngle = 120.0f;
    [SerializeField] private float eyeHeight = 1.5f;

    [Header("Raycast")]
    [SerializeField] private LayerMask obstacleMask;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private int wpIndex = 0;
    private EnemyState currentState = EnemyState.Patrol;
    private Coroutine patrolCoroutine;
    private NavMeshAgent agent;
    private bool gameOver = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>(true);

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        // agent.updateRotation = true;
    }

    private void Start()
    {
        if (waypoints == null || waypoints.Count == 0) return;

        agent.isStopped = true;
        StartCoroutine(StartDelay());

        wpIndex = Random.Range(0, waypoints.Count);
        agent.SetDestination(waypoints[wpIndex].position);

        UpdateAnimations();
    }

    private void Update()
    {
        if (gameOver) return;

        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;

            case EnemyState.Chase:
                ChaseBall();
                break;
        }

        UpdateAnimations();
    }

    private void Patrol()
    {
        agent.speed = patrolSpeed;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f && !agent.isStopped)
        {
            if (patrolCoroutine == null)
                patrolCoroutine = StartCoroutine(PatrolPoint());
        }

        if (CanSeeBall())
        {
            currentState = EnemyState.Chase;

            if (patrolCoroutine != null)
            {
                StopCoroutine(patrolCoroutine);
                patrolCoroutine = null;
            }

            agent.isStopped = false;
        }
    }

    private void ChaseBall()
    {
        if (ball == null) return;

        agent.speed = chaseSpeed;
        agent.isStopped = false;
        agent.SetDestination(ball.position);

        Vector3 lookPos = new Vector3(ball.position.x, transform.position.y, ball.position.z);
        transform.LookAt(lookPos);
    }

    private bool CanSeeBall()
    {
        if (ball == null) return false;

        Vector3 eyePos = transform.position + Vector3.up * eyeHeight;
        Vector3 ballPos = ball.position;
        Vector3 dir = ballPos - eyePos;
        float dist = dir.magnitude;

        if (dist > viewRadius) return false;

        float angleToBall = Vector3.Angle(transform.forward, dir.normalized);
        if (angleToBall > viewAngle * 0.5f) return false;

        if (Physics.Raycast(eyePos, dir.normalized, dist, obstacleMask))
        {
            return false;
        }

        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            gameOver = true;
            agent.isStopped = true;

            if (animator != null)
            {
                animator.SetBool("IsChasing", false);
                animator.SetFloat("Speed", 0f);
            }

            Debug.Log("Se terminó el juego");
        }
    }

    private IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(1f);
        agent.isStopped = false;

        if (waypoints != null && waypoints.Count > 0)
        {
            wpIndex = Random.Range(0, waypoints.Count);
            agent.SetDestination(waypoints[wpIndex].position);
        }
    }

    private IEnumerator PatrolPoint()
    {
        agent.isStopped = true;

        yield return new WaitForSeconds(waitTime);

        int newIndex;
        do
        {
            newIndex = Random.Range(0, waypoints.Count);
        }
        while (newIndex == wpIndex);

        wpIndex = newIndex;

        agent.isStopped = false;
        agent.SetDestination(waypoints[wpIndex].position);

        patrolCoroutine = null;
    }

    private void UpdateAnimations()
    {
        if (animator == null) return;

        animator.SetBool("IsChasing", currentState == EnemyState.Chase);
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }
}