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

    [Header("SFX")]
    public AudioSource audioSource;

    public int floor;

    [Header("Break settings")]
    [Range(0f, 1f)]
    [Tooltip("Probability the workstation breaks after a use (0..1)")]
    public float breakChance = 0.05f;

    private void Awake()
    {
        this.status = "Idle";
        infoPanel = GameObject.Find("WorkStationUI").GetComponent<WorkstationPanelController>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        //gameManager.addGold(5);
        Debug.Log(this.transform.position);

        fx = GetComponent<WorkstationFX>();

        if (isBroken)
        {
            ApplyBrokenState();
        }
    }

    public void OnPointerClick(PointerEventData e)
    {
        infoPanel.Show(this);
    }

    public void accessToWork(BaseWorkerComponent workerComponent)
    {
        if (this.status == "Idle")
        {
            if (isBroken) return;

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
                PlaySfx("FairySound");
                break;
            case "invocacion":
                seconds -= seconds * (workerComponent.model.Stats.summonStat * 5 / 100);
                PlaySfx("MagicEnchantment");
                break;
            case "caldero":
                seconds -= seconds * (workerComponent.model.Stats.alchemyStat * 5 / 100);
                PlaySfx("BoilingCauldron");
                break;
            case "encantamiento":
                seconds -= seconds * (workerComponent.model.Stats.enchantStat * 5 / 100);
                PlaySfx("MagicEnchantment");
                break;
        }

        while (elapsed < seconds)
        {
            if (GameManager.Instance.isPlaying && !GameManager.Instance.isPaused)
            {
                elapsed += Time.deltaTime;
                if (actualProgress != null)
                {
                    actualProgress.GetComponent<progressBar>().progress = elapsed * 100 / seconds;
                }
            }

            yield return null;
        }

        if (assignedCustomer != null && assignedCustomer.GetComponent<CustomerModel>().thief)
        {
            SetBroken(true);
        }
        var realProfit = workstationData.profit + (int)(workstationData.profit * (karma * -0.035f));
        gameManager.realKarma += karma / 10;
        gameManager.addGold(realProfit);
        if (realProfit < 0)
        {
            GameManager.Instance.perdidas += realProfit;
        }

        GameObject instance = Instantiate(textIndicatorPrefab, this.clientPosition.position, Quaternion.identity, this.transform);
        instance.GetComponent<goldFeedback2>().changeText(realProfit.ToString());
        if (Random.value < breakChance)
        {
            SetBroken(true);

            if (actualProgress != null)
            {
                Destroy(actualProgress);
                actualProgress = null;
            }

            StopSfx();
            yield break;
        }

        this.status = "Idle";
        this.clientUsing = 0;
        if (fx) fx.SetWorking(false);
        StopSfx();

        if (actualProgress != null)
        {
            Destroy(actualProgress);
            actualProgress = null;
        }
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

    public void CloseRoom()
    {
        prepurchaseRoom.LockRoom(this.gameObject);
    }

    private void PlaySfx(string clipName)
    {
        if (audioSource == null) return;
        audioSource.clip = AudioManager.Instance.FindSoundClip(clipName);
        audioSource.loop = true;
        audioSource.Play();
    }

    private void StopSfx()
    {
        if (audioSource == null) return;
        audioSource.Stop();
    }

    public void SetBroken(bool broken)
    {
        if (isBroken == broken) return;
        isBroken = broken;
        if (isBroken)
        {
            ApplyBrokenState();
        }
        else
        {
            RepairState();
        }
    }

    private void ApplyBrokenState()
    {
        StopAllCoroutines();
        if (fx) fx.SetWorking(false);
        if (actualProgress != null)
        {
            Destroy(actualProgress);
            actualProgress = null;
        }

        if (assignedCustomer != null)
        {
            assignedCustomer.LeaveWithoutBuy();
            assignedCustomer = null;
            clientUsing = 0;
        }

        if (assignedWorker != null)
        {
            assignedWorker.isWorking = false;
            assignedWorker.LeaveWorkStation();
            assignedWorker = null;
            assignedWorkerName = null;
        }

        status = "Broken";

        if (GlobalWorkstationManager.Instance.activeStations.Contains(this))
        {
            GlobalWorkstationManager.Instance.activeStations.Remove(this);
        }

        fx.SetBroken(true);

    }

    private void RepairState()
    {
        status = "Idle";
        if (!GlobalWorkstationManager.Instance.activeStations.Contains(this))
        {
            GlobalWorkstationManager.Instance.activeStations.Add(this);
        }

        fx.SetBroken(false);
    }

}
