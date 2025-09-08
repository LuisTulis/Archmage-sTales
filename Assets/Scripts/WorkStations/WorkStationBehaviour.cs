using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class WorkStationBehaviour : MonoBehaviour, IPointerClickHandler
{
    public WorkstationData workstationData;

    private WorkstationPanelController infoPanel;

    private void Awake()
    {
        infoPanel = GameObject.Find("WorkStationUI").GetComponent<WorkstationPanelController>();
        Debug.Log(this.transform.position);
    }

    public void OnPointerClick(PointerEventData e) => infoPanel.Show(workstationData);

}
