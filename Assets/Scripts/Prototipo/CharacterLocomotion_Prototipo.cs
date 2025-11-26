using UnityEngine;
using UnityEngine.AI;

public class CharacterLocomotion_Prototipo : MonoBehaviour
{
    public NavMeshAgent agent;
    protected CharacterModel_Prototipo agentModel;
    public Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agentModel = GetComponent<CharacterModel_Prototipo>();
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agentModel = GetComponent<CharacterModel_Prototipo>();
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

    public void WalkingAnimation(bool isWorking)
    {
        if (animator == null || agent == null) return;

        bool isWalking = agent.velocity.magnitude > 0.1f;
        animator.SetBool("walking", isWalking && !isWorking);
    }

    public void SittingAnimation(bool sitting)
    {
        if (animator == null) return;
        animator.SetBool("sitting", sitting);
    }

}
