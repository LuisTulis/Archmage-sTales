using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;

public class CustomerComponent : CharacterComponent
{
    protected RandomWalkLocomotion locomotion;
    public CustomerModel model;

    public bool leave = false;
    public WorkStationBehaviour objectiveStation;

    private GlobalWorkstationManager stationManager;
    public List<StationType> objectives;
    public CustomerObjective customerObjective;

    private Animator animator;

    private int searchAttempts = 0;
    private float attemptCooldown = 10f;
    private float attemptTimer = 0f;
    private int maxSearchAttempts = 5;

    private int attendAttempts = 0;
    private float attendTimer = 0f;
    private float attendCooldown = 8f;
    private int maxAttendAttempts = 5;

    public int minMentalDecayRate = 1;
    public int maxMentalDecayRate = 7;
    private bool isBlinking = false;
    private Coroutine blinkingCoroutine;

    

    protected override void Awake()
    {
        base.Awake();

        locomotion = GetComponent<RandomWalkLocomotion>();
        model = GetComponent<CustomerModel>();
        animator = GetComponentInChildren<Animator>();

        objectives = new List<StationType>();
        stationManager = GameObject.Find("GlobalWorkstationManager").GetComponent<GlobalWorkstationManager>();

        objectives.Add(stationManager.stationTypes[Random.Range(0, stationManager.stationTypes.Count)]);
        customerObjective = this.gameObject.GetComponentInChildren<CustomerObjective>();
        customerObjective.objective = objectives[0].ToString();

        this.minMentalDecayRate = GlobalCustomerManager.Instance.minMentalDecayRate;
        this.maxMentalDecayRate = GlobalCustomerManager.Instance.maxMentalDecayRate;

        selectStation();
    }

    private void Start()
    {
        if(GameManager.Instance.dayCount < 2)
        {
            model.thief = false;
        }
        if (model.thief)
        {
            
            customerObjective.image.color = new Color(1, 0, 0);
        }
    }

    private void Update()
    {
        UpdateWalkingAnimation();

        if (Vector3.Distance(this.transform.position, GlobalCustomerManager.Instance.despawnPoint.position) < 3)
        {
            LeaveTheShop();
        }

        if (leave) return;

        if (objectiveStation != null)
        {
            HandleStationLogic();
        }
        else
        {
            HandleWaitingBehaviour();
        }


        // FIXME: Deberia ser una posibilidad de volverse ladron, cuanto mas bajo el mental.
        if (model.mental < 15 && !model.skeleton)
        {
            model.thief = true;
            customerObjective.image.color = new Color(1, 0, 0);
        }
    }

    private void HandleWaitingBehaviour()
    {
        locomotion.IdleRandomWalk();

        model.waitingTime += Time.deltaTime;
        attemptTimer += Time.deltaTime;

        if (attemptTimer >= attemptCooldown)
        {
            attemptTimer = 0f;
            searchAttempts++;
            int mentalReduce = Random.Range(minMentalDecayRate, maxMentalDecayRate);
            mentalReduce = ItemController.Instance.activeItemsState[11] ? (int)(mentalReduce * .5f) : mentalReduce;
            model.mental -= mentalReduce; 
            Debug.Log($"{name} intenta buscar estación (Intento #{searchAttempts}) | Mental: {model.mental}");

            selectStation();

            if (objectiveStation == null && model.mental < 40 && searchAttempts >= 3 && !isBlinking && !model.skeleton)
            {
                blinkingCoroutine = StartCoroutine(BlinkObjectiveIcon());
            }
            var randomValue = Random.Range(0, (model.mental * 2));
            Debug.Log("Intento por irse del local: " + randomValue + "     maximo: " + (model.mental * 2));

            if (searchAttempts >= 3 && randomValue <= 0 && !model.skeleton)
            {
                Debug.Log($"{name} se va por frustración buscando estación.");

                LeaveWithoutBuy();
            }
        }
       
    }

    private void HandleStationLogic()
    {
        if (objectiveStation.clientUsing == 0)
        {
            objectives.RemoveAt(0);

            if (objectives.Count > 0)
            {
                customerObjective.objective = objectives[0].ToString();
            }
            else
            {
                customerObjective.objective = "";
            }

            if (objectives.Count == 0)
            {
                leave = true;
                objectiveStation = null;
                LeaveWithoutBuy();
            }
            else
            {
                selectStation();
            }
        }
        else if (Vector3.Distance(transform.position, objectiveStation.transform.position) < 3)
        {
            objectiveStation.clientUsing = 2;
            objectiveStation.assignedCustomer = this;
            GetIntoBuyingPosition();

            if (string.IsNullOrEmpty(objectiveStation.assignedWorkerName) && objectiveStation.status == "Idle")
            {
                model.waitingTime += Time.deltaTime;
                attendTimer += Time.deltaTime;

                if (attendTimer >= attendCooldown)
                {
                    attendTimer = 0f;
                    attendAttempts++;
                    model.mental -= Random.Range(minMentalDecayRate, maxMentalDecayRate);
                    Debug.Log($"{name} está esperando atención en {objectiveStation.name} (Intento #{attendAttempts}) | Mental: {model.mental}");

                    if (attendAttempts >= 3 && model.mental < 40 && !isBlinking)
                    {
                        blinkingCoroutine = StartCoroutine(BlinkObjectiveIcon());
                    }
                    var randomValue = Random.Range(0, (model.mental * 2));
                    Debug.Log("Intento por irse del local: " + randomValue + "     maximo: " + (model.mental * 2));
                    if (attendAttempts >= 3 && randomValue == 0)
                    {
                        Debug.Log($"{name} se va por falta de atención en {objectiveStation.name}.");
                        LeaveWithoutBuy();
                        customerObjective.objective = "";
                    }
                }
                
            }
        }
        else
        {
            MoveToObjectiveStation();
        }
    }

    private IEnumerator BlinkObjectiveIcon()
    {
        isBlinking = true;
        var img = customerObjective.image;
        Color originalColor = img.color;
        Color blinkColor = Color.black;

        float t = 0f;
        bool toBlack = true;

        while (!leave &&
               (objectiveStation == null && searchAttempts >= 3) ||
               objectiveStation != null &&
               string.IsNullOrEmpty(objectiveStation.assignedWorkerName) &&
               objectiveStation.status == "Idle")
        {
            t += Time.deltaTime * 2f;
            img.color = Color.Lerp(toBlack ? originalColor : blinkColor, toBlack ? blinkColor : originalColor, t);

            if (t >= 1f)
            {
                t = 0f;
                toBlack = !toBlack;
            }

            yield return null;
        }

        img.color = originalColor;
        isBlinking = false;
    }


    private void MoveToObjectiveStation()
    {
        if (objectiveStation != null)
        {
            locomotion.MoveTo(this.objectiveStation.clientPosition.transform.position);
        }
    }

    public void LeaveWithoutBuy()
    {
        this.leave = true;

        if (this.objectiveStation != null)
        {
            this.objectiveStation.StopAllCoroutines();
            this.objectiveStation.fx.SetWorking(false);
            this.objectiveStation.clientUsing = 0;
            this.objectiveStation.status = "Idle";
            Destroy(this.objectiveStation.actualProgress);
            this.objectiveStation.assignedCustomer = null;
            this.objectiveStation = null;
        }

        this.objectives.Clear();
        animator.SetBool("sitting", false);
        locomotion.MoveTo(GlobalCustomerManager.Instance.despawnPoint.position);
    }

    private void LeaveTheShop()
    {
        GameManager.Instance.reputacion += (this.model.mental - 25)/10;
        Debug.Log("mental: " + this.model.mental);
        GlobalCustomerManager.Instance.CustomerLeft(this);
    }

    private void UpdateWalkingAnimation()
    {
        if (animator == null) return;

        bool isWalking = locomotion.agent.velocity.magnitude > 0.1f;
        animator.SetBool("walking", isWalking);
    }

    private void GetIntoBuyingPosition()
    {
        Vector3 targetPos = objectiveStation.clientPosition.position;
        targetPos.y = transform.position.y;
        transform.position = targetPos;

        Vector3 direction = objectiveStation.workDirection.position - transform.position;
        direction.y = 0f;
        transform.rotation = Quaternion.LookRotation(direction);

        animator.SetBool("sitting", objectiveStation.sittingWorkstation);
    }

    private void selectStation()
    {

        if (objectiveStation != null && objectiveStation.clientUsing == 1)
        {
            objectiveStation.clientUsing = 0;
            objectiveStation.assignedCustomer = null;
            objectiveStation.status = "Idle";
            objectiveStation = null;
        }

        List<WorkStationBehaviour> emptyStations = new List<WorkStationBehaviour>();

        foreach (WorkStationBehaviour station in stationManager.activeStations)
        {
            if (station.clientUsing == 0 && this.objectives[0] == station.type)
            {
                emptyStations.Add(station);
            }
        }
        if (emptyStations.Count != 0)
        {
            if (isBlinking && blinkingCoroutine != null)
            {
                StopCoroutine(blinkingCoroutine);
                customerObjective.image.color = Color.white;
                isBlinking = false;
            }

            searchAttempts = 0;
            model.waitingTime = 0f;

            objectiveStation = emptyStations[Random.Range(0, emptyStations.Count)];
            objectiveStation.clientUsing = 1;
            objectiveStation.assignedCustomer = this;
            MoveToObjectiveStation();
        }
    }

    public override Dictionary<string, object> GetStats()
    {
        var stats = new Dictionary<string, object>
        {
            { "Name", model.CharacterName },
            { "Speed", model.Speed.ToString("F1") },
            { "Icon", model.Icon }
        };
        return stats;
    }



}
