using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MummyAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float wanderRadius = 10f;
    public float minWanderWaitTime = 2f;
    public float maxWanderWaitTime = 5f;
    
    private NavMeshAgent agent;
    private float nextWanderTime;
    private Vector3 startPosition;
    private bool isInitialized = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void Initialize()
    {
        if (isInitialized) return;
        
        startPosition = transform.position;
        SetNextWanderTime();
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized) return;

        if (Time.time >= nextWanderTime)
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += startPosition;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                SetNextWanderTime();
            }
        }
    }

    private void SetNextWanderTime()
    {
        nextWanderTime = Time.time + Random.Range(minWanderWaitTime, maxWanderWaitTime);
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the wander radius in the editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(isInitialized ? startPosition : transform.position, wanderRadius);
    }
} 