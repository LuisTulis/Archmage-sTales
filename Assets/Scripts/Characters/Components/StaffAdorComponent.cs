using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StaffAdorComponent : CharacterComponent {
    [SerializeField] private string patrolParentName = "PatrolPoints";
    [SerializeField] private float reachThreshold = 0.5f;
    [SerializeField] private float waitTimeAtPoint = 1f;

    private List<Transform> patrolPoints = new List<Transform>();
    private int currentIndex = 0;
    private NavMeshAgent agent;
    private float waitTimer = 0f;

    private void Start() {
        agent = GetComponent<NavMeshAgent>();

        GameObject parent = GameObject.Find(patrolParentName);
        if (parent != null) {
            foreach (Transform child in parent.transform) {
                patrolPoints.Add(child);
            }
        }

        if (patrolPoints.Count > 0) {
            agent.SetDestination(patrolPoints[currentIndex].position);
        } else {
            Debug.LogWarning("No se encontraron patrol points bajo: " + patrolParentName);
        }
    }

    private void Update() {
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
