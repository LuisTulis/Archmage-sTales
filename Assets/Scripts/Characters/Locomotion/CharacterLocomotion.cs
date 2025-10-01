using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterLocomotion : MonoBehaviour
{
    [SerializeField] private string patrolParentName = "PatrolPoints";
    [SerializeField] private float reachThreshold = 0.5f;
    private float waitTimeAtPoint = 5f;

    private float stopDistance = 2.5f;
    private bool initialized = false;

    private List<Transform> patrolPoints = new List<Transform>();
    private int currentIndex = 0;
    private NavMeshAgent agent;
    private CharacterModel agentModel;
    private float waitTimer = 0f;

    public Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agentModel = GetComponent<CharacterModel>();
        if (agent != null)
        {
            SetSpeed(agentModel.Speed);
        }

    }

    public void MoveTo(Vector3 targetPosition)
    {
        if (agent != null)
        {
            agent.SetDestination(targetPosition);
        }
    }

    public void StopMovement()
    {
        if (agent != null)
        {
            agent.ResetPath();
        }
    }

    public void SetSpeed(float speed)
    {
        if (agent != null)
        {
            agent.speed = speed;
        }
    }

    public void InitializePatrolPoints()
    {
        patrolPoints.Clear();
        Transform patrolParent = GameObject.Find(patrolParentName)?.transform;
        if (patrolParent != null)
        {
            foreach (Transform point in patrolParent)
            {
                patrolPoints.Add(point);
            }
            if (patrolPoints.Count > 0)
            {
                currentIndex = 0;
                agent.SetDestination(patrolPoints[currentIndex].position);
            }
        }
        else
        {
            Debug.LogWarning($"Patrol parent '{patrolParentName}' not found in the scene.");
        }
    }


    public void IdleRandomWalk()
    {
        if (patrolPoints.Count == 0) return;

        if (!initialized)
        {
            currentIndex = Random.Range(0, patrolPoints.Count);

            Vector3 randomOffset = Random.insideUnitSphere * stopDistance;
            randomOffset.y = 0;
            agent.SetDestination(patrolPoints[currentIndex].position + randomOffset);

            initialized = true;
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= reachThreshold)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtPoint)
            {
                waitTimer = 0f;

                int newIndex;
                do
                {
                    newIndex = Random.Range(0, patrolPoints.Count);
                } while (newIndex == currentIndex && patrolPoints.Count > 1);

                currentIndex = newIndex;

                Vector3 randomOffset = Random.insideUnitSphere * stopDistance;
                randomOffset.y = 0;
                Vector3 targetPos = patrolPoints[currentIndex].position + randomOffset;

                agent.SetDestination(targetPos);
            }
        }
    }

    public void WalkingAnimation()
    {
        if (animator == null || agent == null) return;

        bool isWalking = agent.velocity.magnitude > 0.1f;
        animator.SetBool("walking", isWalking);
    }

}
