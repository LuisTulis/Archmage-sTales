using System.Collections.Generic;
using UnityEngine;

public class RandomWalkLocomotion_Prototipo : CharacterLocomotion_Prototipo
{
    [SerializeField] private float reachThreshold = 0.5f;
    private float waitTimeAtPoint = 5f;

    private float stopDistance = 2.5f;
    private bool initialized = false;
    
    private List<Transform> patrolPoints => GlobalLocomotionManager.Instance?.PatrolPoints;

    private int currentIndex = 0;
    private float waitTimer = 0f;

    public void IdleRandomWalk() {
        if (patrolPoints == null || patrolPoints.Count == 0) return;

        if (!initialized) {
            currentIndex = Random.Range(0, patrolPoints.Count);

            Vector3 randomOffset = Random.insideUnitSphere * stopDistance;
            randomOffset.y = 0;
            agent.SetDestination(patrolPoints[currentIndex].position + randomOffset);

            initialized = true;
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= reachThreshold) {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtPoint) {
                waitTimer = 0f;

                int newIndex;
                do {
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


}
