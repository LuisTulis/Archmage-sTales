using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Customer : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private WorkStationBehaviour objectiveStation;
    [SerializeField] private float NearestPointSearchRange = 10f;

    private void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start() {
        MoveToObjectiveStation();
    }

    private void Update() {
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) {
            LeaveTheShop();
        }
    }

    private void MoveToObjectiveStation() {
        LeaveWithoutBuy();
    }

    private void LeaveWithoutBuy() {
        Debug.Log("Leave without buy");
        MoveTo(GlobalCustomerManager.Instance.despawnPoint.position);
    }

    public void MoveTo(Vector3 destination) {
        if (navMeshAgent != null) {
            NavMeshHit hitResult;
            if (NavMesh.SamplePosition(destination, out hitResult, NearestPointSearchRange, NavMesh.AllAreas)) {
                navMeshAgent.SetDestination(hitResult.position);
            }
        }
    }

    private void LeaveTheShop() {
        GlobalCustomerManager.Instance.CustomerLeft(this);
    }



}
