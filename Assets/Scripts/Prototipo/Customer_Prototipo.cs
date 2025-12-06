using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Customer_Prototipo : MonoBehaviour
{
    private WorkstationManager_Prototipo stationManager;
    private NavMeshAgent navMeshAgent;
    private WorkStationBehaviour_Prototipo objectiveStation;
    [SerializeField] private float NearestPointSearchRange = 10f;
    [SerializeField] private string patrolParentName = "PatrolPoints";
    private List<Transform> patrolPoints = new List<Transform>();
    private int currentIndex = 0;

    public List<StationType> objectives;
    public float stopDistance = 10f;
    private bool initialized = false;
    [SerializeField] private float reachThreshold = 0.5f;
    private float waitTimeAtPoint = 5f;
    public float waitTimer = 0f;
    public bool isThief;

    public bool leave = false;

    public float attempt = 0;
    public Vector3 targetPosition;

    private CustomerObjective customerObjective;

    private Animator animator;

    private void Awake()
    {
        objectives = new List<StationType>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = 2f;

        stationManager = GameObject.Find("WorkstationManager").GetComponent<WorkstationManager_Prototipo>();
        objectives.Add(stationManager.stationTypes[Random.Range(0, stationManager.stationTypes.Count)]);
        customerObjective = this.gameObject.GetComponentInChildren<CustomerObjective>();
        customerObjective.objective = objectives[0].ToString();
        InitializePatrolPoints();
        selectStation();

        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        var randomNumber = Random.Range(0f, 1f);
        isThief = randomNumber < 0.05f;
        if (isThief)
        {
            customerObjective.image.color = new Color(1, 0, 0);
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
            navMeshAgent.SetDestination(patrolPoints[currentIndex].position + randomOffset);

            initialized = true;
            return;
        }

        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= reachThreshold)
        {
            waitTimer += Time.deltaTime;

            if (waitTimer >= waitTimeAtPoint)
            {
                waitTimer = 0f;

                if (objectiveStation == null)
                {
                    if (attempt > 5)
                    {
                        Debug.Log("Irse sin pagar");
                        LeaveWithoutBuy();
                    }
                    else
                    {
                        Debug.Log("Buscando mesa nueva");
                        attempt += 1;
                        var randomNumber = Random.Range(0f, 1f);
                        isThief = randomNumber < 0.05f;
                        selectStation();

                    }
                }
                int newIndex;
                do
                {
                    newIndex = Random.Range(0, patrolPoints.Count);
                } while (newIndex == currentIndex && patrolPoints.Count > 1);

                currentIndex = newIndex;

                Vector3 randomOffset = Random.insideUnitSphere * stopDistance;
                randomOffset.y = 0;
                Vector3 targetPos = patrolPoints[currentIndex].position + randomOffset;
                if (attempt > 3)
                {
                    Debug.Log("Irse sin pagar");
                    targetPos = GlobalCustomerManager_Prototipo.Instance.despawnPoint.position;
                    LeaveWithoutBuy();
                }

                navMeshAgent.SetDestination(targetPos);
            }
        }
    }

    private void selectStation()
    {
        List<WorkStationBehaviour_Prototipo> emptyStations = new List<WorkStationBehaviour_Prototipo>();
        foreach (WorkStationBehaviour_Prototipo station in stationManager.activeStations)
        {
            if (station.clientUsing == 0 && this.objectives[0] == station.type)
            {
                emptyStations.Add(station);
            }
        }
        if (emptyStations.Count != 0)
        {

            this.objectiveStation = emptyStations[Random.Range(0, emptyStations.Count)];
            objectiveStation.clientUsing = 1;

            MoveToObjectiveStation();
        }
    }
    public void StopMovement()
    {
        if (navMeshAgent != null)
        {
            navMeshAgent.ResetPath();
        }
    }


    private void Update()
    {
        if (Vector3.Distance(this.transform.position, GlobalCustomerManager_Prototipo.Instance.despawnPoint.position) < 3)
        {
            LeaveTheShop();
        }
        if (!leave)
        {
            if (this.objectiveStation != null)
            {
                if (objectiveStation.clientUsing == 0)
                {
                    this.objectives.Remove(this.objectives[0]);
                    if (this.objectives.Count > 0)
                    {
                        this.customerObjective.objective = this.objectives[0].ToString();
                    }
                    else
                    {
                        this.customerObjective.objective = "";
                    }
                    if (this.objectives.Count == 0)
                    {
                        this.leave = true;
                        this.objectiveStation = null;
                        LeaveWithoutBuy();
                    }
                    else
                    {
                        selectStation();
                    }
                }
                else if (Vector3.Distance(this.transform.position, objectiveStation.transform.position) < 3)
                {
                    objectiveStation.clientUsing = 2;
                    objectiveStation.assignedCustomer = this;
                }
                else
                {
                    MoveToObjectiveStation();
                }

            }
            else
            {
                IdleRandomWalk();

            }
        }

        UpdateWalkingAnimation();
    }

    private void MoveToObjectiveStation()
    {
        MoveTo(this.objectiveStation.clientPosition.transform.position);
        //LeaveWithoutBuy();
    }

    public void LeaveWithoutBuy()
    {
        this.leave = true;
        if (this.objectiveStation != null)
        {

            this.objectiveStation.StopAllCoroutines();
            this.objectiveStation.assignedCustomer = null;
            this.objectiveStation.clientUsing = 0;
            this.objectiveStation.status = "Idle";
            this.objectiveStation = null;
        }
        this.objectives.Clear();
        this.customerObjective.objective = "";
        MoveTo(GlobalCustomerManager_Prototipo.Instance.despawnPoint.position);
    }

    public void MoveTo(Vector3 destination)
    {

        if (navMeshAgent != null)
        {
            NavMeshHit hitResult;
            if (NavMesh.SamplePosition(destination, out hitResult, NearestPointSearchRange, NavMesh.AllAreas))
            {
                targetPosition = hitResult.position;
                navMeshAgent.SetDestination(hitResult.position);
            }
        }
    }

    private void LeaveTheShop()
    {
        GlobalCustomerManager_Prototipo.Instance.CustomerLeft(this);
    }

    private void UpdateWalkingAnimation()
    {
        if (animator == null || navMeshAgent == null) return;

        bool isWalking = navMeshAgent.velocity.magnitude > 0.1f;
        animator.SetBool("walking", isWalking);
    }

}
