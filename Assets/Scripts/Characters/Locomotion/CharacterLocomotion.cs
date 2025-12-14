using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class CharacterLocomotion : MonoBehaviour
{
    public NavMeshAgent agent;
    protected CharacterModel agentModel;
    public Animator animator;
    private float oldYPosition = 0;
    private int oldFloor = 0;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agentModel = GetComponent<CharacterModel>();
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agentModel = GetComponent<CharacterModel>();
        if (agent != null)
        {
            int speed = ItemController.Instance.activeItemsState[7] ? agentModel.Speed * 4 : agentModel.Speed;
            SetSpeed(speed);
        }
    }

    public void checkSpeedUpgrade(bool powerUp)
    {
        int speed = powerUp ? agentModel.Speed * 4 : agentModel.Speed;
        SetSpeed(speed);
    }

    private void Update()
    {
        int floorValue = GameManager.Instance.actualFloor;        
        int minValue = floorValue * 3;
        int maxValue = (1 + floorValue) * 3;
        bool isShowing = (this.transform.position.y < .1f || (this.transform.position.y > minValue && this.transform.position.y < maxValue));
        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = isShowing;
        CustomerComponent customer = this.GetComponent<CustomerComponent>();
        if(customer != null)
        {
            customer.customerObjective.show = isShowing;
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
