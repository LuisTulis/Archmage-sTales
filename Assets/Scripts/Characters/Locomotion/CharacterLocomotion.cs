using UnityEngine;
using UnityEngine.AI;

public class CharacterLocomotion : MonoBehaviour
{    
    protected NavMeshAgent agent;
    protected CharacterModel agentModel;

    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
        agentModel = GetComponent<CharacterModel>();
    }

    private void Start() {
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

}
