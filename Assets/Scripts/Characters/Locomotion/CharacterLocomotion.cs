using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterLocomotion : MonoBehaviour
{
    [SerializeField] private string patrolParentName = "PatrolPoints";
    [SerializeField] private float reachThreshold = 0.5f;
    [SerializeField] private float waitTimeAtPoint = 1f;

    private List<Transform> patrolPoints = new List<Transform>();
    private int currentIndex = 0;
    private NavMeshAgent agent;
    private CharacterModel agentModel;
    private float waitTimer = 0f;

    private void Awake() {
        //agent = GetComponent<NavMeshAgent>();
        //agentModel = GetComponent<CharacterModel>();
        //if(agent != null) {
        //    SetSpeed(agentModel.Speed);
        //    Debug.Log("Speed" + agent.speed.ToString());
        //}
    }

    private void Start() {
        agent = GetComponent<NavMeshAgent>();
        agentModel = GetComponent<CharacterModel>();
        if (agent != null) {
            SetSpeed(agentModel.Speed);
        }

    }

    public void MoveTo(Vector3 targetPosition) {
        if (agent != null) {
            agent.SetDestination(targetPosition);
        }
    }

    public void StopMovement() {
        if (agent != null) {
            agent.ResetPath();
        }
    }

    public void SetSpeed(float speed) {
        if (agent != null) {
            agent.speed = speed;
        }
    }

    public void InitializePatrolPoints() {
        patrolPoints.Clear();
        Transform patrolParent = GameObject.Find(patrolParentName)?.transform;
        if (patrolParent != null) {
            foreach (Transform point in patrolParent) {
                patrolPoints.Add(point);
            }
            if (patrolPoints.Count > 0) {
                currentIndex = 0;
                agent.SetDestination(patrolPoints[currentIndex].position);
            }
        } else {
            Debug.LogWarning($"Patrol parent '{patrolParentName}' not found in the scene.");
        }
    }

    public void IdleRandomWalk() {
        if (patrolPoints.Count == 0) return;

        if (!agent.pathPending && agent.remainingDistance <= reachThreshold) {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtPoint) {
                waitTimer = 0f;
                currentIndex = (currentIndex + 1) % patrolPoints.Count;
                agent.SetDestination(patrolPoints[currentIndex].position);
            }
        }
    }
}
