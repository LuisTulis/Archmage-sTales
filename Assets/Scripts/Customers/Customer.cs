using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Customer : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private WorkStationBehaviour objectiveStation;
    [SerializeField] private float NearestPointSearchRange = 10f;


    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void selectStation()
    {
        WorkStationBehaviour[] stations = GameObject.FindObjectsOfType<WorkStationBehaviour>();
        List<WorkStationBehaviour> emptyStations = new List<WorkStationBehaviour>();
        foreach (WorkStationBehaviour station in stations)
        {
            if (station.clientUsing == 0)
            {
                emptyStations.Add(station);
            }
        }
        if (emptyStations.Count != 0)
        {

            this.objectiveStation = emptyStations[Random.Range(0, emptyStations.Count)];
            objectiveStation.clientUsing = 1;
        }
    }
    private void Start()
    {
        selectStation();
        if (this.objectiveStation != null)
        {
            MoveToObjectiveStation();
        }
        else
        {
            LeaveWithoutBuy();
        }
    }

    private void Update()
    {
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            //LeaveTheShop();
        }
        if (this.objectiveStation != null)
        {
            if (objectiveStation.clientUsing == 0)
            {
                LeaveWithoutBuy();
            }
            else if (Vector3.Distance(this.transform.position, objectiveStation.transform.position) < 3)
            {
                objectiveStation.clientUsing = 2;
            }

        }
    }

    private void MoveToObjectiveStation()
    {
        MoveTo(this.objectiveStation.clientPosition.transform.position);
        //LeaveWithoutBuy();
    }

    private void LeaveWithoutBuy()
    {
        Debug.Log("Leave without buy");
        MoveTo(GlobalCustomerManager.Instance.despawnPoint.position);
    }

    public void MoveTo(Vector3 destination)
    {
        if (navMeshAgent != null)
        {
            NavMeshHit hitResult;
            if (NavMesh.SamplePosition(destination, out hitResult, NearestPointSearchRange, NavMesh.AllAreas))
            {
                navMeshAgent.SetDestination(hitResult.position);
            }
        }
    }

    private void LeaveTheShop()
    {
        GlobalCustomerManager.Instance.CustomerLeft(this);
    }



}
