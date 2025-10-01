using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class WorkStationBehaviour : MonoBehaviour, IPointerClickHandler
{
    public WorkstationData workstationData;

    private WorkstationPanelController infoPanel;
    private GameManager gameManager;
    public string status;
    public Transform workerPosition;
    public Transform clientPosition;
    public int clientUsing = 0;
    public string assignedWorker;
    public stationType type;

    public GameObject textIndicatorPrefab; 
    

    [Header("Table FX")]
    private WorkstationFX fx;

    private void Awake()
    {
        this.status = "Idle";
        infoPanel = GameObject.Find("WorkStationUI").GetComponent<WorkstationPanelController>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.addGold(5);
        Debug.Log(this.transform.position);

        fx = GetComponent<WorkstationFX>();
    }

    public void OnPointerClick(PointerEventData e)
    {
        infoPanel.Show(this);
    }

    public void accessToWork(string workerName)
    {
        if (this.status == "Idle")
        {
            if (fx) fx.SetWorking(true);
            this.status = "Being used";
            StartCoroutine(BeingUsed());
        }
    }
    private IEnumerator BeingUsed()
    {
        Debug.Log("Entré");
        float seconds = gameManager.aletargamiento ? workstationData.Speed : workstationData.Speed * 2;
        yield return new WaitForSeconds(workstationData.Speed);
        gameManager.addGold(workstationData.profit);
        GameObject instance = Instantiate(textIndicatorPrefab, this.transform.position, Quaternion.identity, this.transform);
        instance.GetComponent<goldFeedback2>().changeText(workstationData.profit.ToString());
        this.status = "Idle";
        this.clientUsing = 0;
        if (fx) fx.SetWorking(false);
    }

    public void UpgradeFX(int level)
    {
        if (fx == null)
        {
            Debug.LogWarning("No WorkstationFX attached.");
            return;
        }

        Debug.Log("Playing particle system.");
        fx.ApplyUpgradeLevel(level);
    }

}
