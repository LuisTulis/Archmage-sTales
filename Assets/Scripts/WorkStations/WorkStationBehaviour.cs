using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class WorkStationBehaviour : MonoBehaviour, IPointerClickHandler
{
    public WorkstationData workstationData;

    private WorkstationPanelController infoPanel;
    private GameManager gameManager;
    public string status;

    private void Awake()
    {
        this.status = "Idle";
        infoPanel = GameObject.Find("WorkStationUI").GetComponent<WorkstationPanelController>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.addGold(5);
        Debug.Log(this.transform.position);
    }

    public void OnPointerClick(PointerEventData e)
    {
        infoPanel.Show(workstationData);
    }

    public void accessToWork(string workerName)
    {
        workstationData.assignedWorker = workerName;
        workstationData.status = this.status;
        if (this.status == "Idle")
        {
            this.status = "Being used";
            StartCoroutine(BeingUsed());
        }
        Debug.Log(workstationData.displayName);
    }
    private IEnumerator BeingUsed()
    {
        Debug.Log("Entré");
        yield return new WaitForSeconds(workstationData.Speed);
        gameManager.addGold(workstationData.profit);
        this.status = "Idle";
    }

    

    
}
