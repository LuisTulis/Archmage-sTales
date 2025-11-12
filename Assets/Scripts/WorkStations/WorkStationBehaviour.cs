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
    public string assignedWorkerName;
    public BaseWorkerComponent assignedWorker;
    public CustomerComponent assignedCustomer;
    public StationType type;

    public GameObject textIndicatorPrefab;
    public bool isBroken = false;
    public float karma = 0;

    [Header("Table FX")]
    public WorkstationFX fx;

    [Header("Aux for animations")]
    public Transform workDirection;
    public bool sittingWorkstation;

    public GameObject ProgressBarPrefab;
    public GameObject actualProgress;

    public AlchemyRoom prepurchaseRoom;

    public float elapsed = 0;

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

    public void accessToWork(BaseWorkerComponent workerComponent)
    {
        if (this.status == "Idle")
        {
            if (fx) fx.SetWorking(true);
            this.status = "Being used";
            actualProgress = Instantiate(ProgressBarPrefab, clientPosition.position, Quaternion.identity, transform);
            actualProgress.transform.position += new Vector3(0, 5, 0);
            elapsed = 0;
            assignedWorker = workerComponent;
            StartCoroutine(BeingUsed(workerComponent));
        }
    }
    private IEnumerator BeingUsed(BaseWorkerComponent workerComponent)
    {

        float seconds = gameManager.aletargamiento ? workstationData.Speed * 2 : workstationData.Speed;

        switch (this.type.ToString())
        {
            case "adivinacion":
                Debug.Log(workerComponent.model.Stats.adivinationStat * 5 / 100);
                seconds -= seconds * (workerComponent.model.Stats.adivinationStat * 5f / 100f);
                break;
            case "invocacion":
                seconds -= seconds * (workerComponent.model.Stats.summonStat * 5 / 100);
                break;
            case "caldero":
                seconds -= seconds * (workerComponent.model.Stats.alchemyStat * 5 / 100);
                break;
            case "encantamiento":
                seconds -= seconds * (workerComponent.model.Stats.enchantStat * 5 / 100);
                break;
        }

        while (elapsed < seconds)
        {
            elapsed += Time.deltaTime;
            if (actualProgress != null)
            {
                actualProgress.GetComponent<progressBar>().progress = elapsed * 100 / seconds;
            }
            yield return null;
        }

        //yield return new WaitForSeconds(seconds);

        int realProfit;

        if (assignedCustomer.GetComponent<CustomerModel>().thief)
        {
            realProfit = (int)(workstationData.profit * -0.25f);

        }
        else
        {
            realProfit = workstationData.profit;
        }
        realProfit = gameManager.costoso ? (int)(realProfit * .5f) : realProfit;
        gameManager.addGold(realProfit);
        if (realProfit < 0)
        {
            GameManager.Instance.perdidas += realProfit;
        }

        GameObject instance = Instantiate(textIndicatorPrefab, this.clientPosition.position, Quaternion.identity, this.transform);
        instance.GetComponent<goldFeedback2>().changeText(realProfit.ToString());
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

    public void CloseRoom() {
        prepurchaseRoom.LockRoom(this.gameObject);
    }

}
