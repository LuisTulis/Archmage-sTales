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

    public bool showingFeedback = true;

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
            if (showingFeedback)
            {
                if (fx) fx.SetWorking(true);
                actualProgress.transform.position += new Vector3(0, 5, 0);

            }
            else
            {
                actualProgress.transform.position += new Vector3(0, -5000, 0);

            }
            elapsed = 0;
            assignedWorker = workerComponent;
            StartCoroutine(BeingUsed(workerComponent));
        }
    }
    private IEnumerator BeingUsed(BaseWorkerComponent workerComponent)
    {

        float seconds = gameManager.aletargamiento ? workstationData.Speed * 2 : workstationData.Speed;
        float mejoraOro = 1;
        if (ItemController.Instance.activeItemsState[6])
        {
            seconds = seconds * .8f;
        }
        if (ItemController.Instance.activeItemsState[1])
        {
            mejoraOro = 1.2f;
        }

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

        if (assignedCustomer.GetComponent<CustomerModel>().thief && !ItemController.Instance.activeItemsState[10])
        {
            if (ItemController.Instance.activeItemsState[10])
            {
                ItemController.Instance.activeItemsUse[10]--;
                ItemController.Instance.checkActualItems();
            }
            SetBroken(true);
        }
        int realProfit = workstationData.profit;

        realProfit = gameManager.costoso ? (int)(realProfit * .5f) : realProfit;
        realProfit = (int)(realProfit * mejoraOro);
        realProfit = realProfit + (int)(realProfit * (karma * -0.035f));

        gameManager.realKarma += karma / 10;

        if (gameManager.realKarma > 50)
        {
            gameManager.realKarma = 50;
        }
        else if (gameManager.realKarma < -50)
        {
            gameManager.realKarma = -50;
        }

        gameManager.addGold(realProfit);
        if (realProfit < 0)
        {
            GameManager.Instance.perdidas += realProfit;
        }

        GameObject instance = Instantiate(textIndicatorPrefab, this.clientPosition.position, Quaternion.identity, this.transform);
        instance.GetComponent<goldFeedback2>().changeText(realProfit.ToString());
        float randomValue = ItemController.Instance.activeItemsState[2] ? Random.value * 2 : Random.value;
        if (randomValue < breakChance)
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
        if (showingFeedback)
        {
            audioSource.clip = AudioManager.Instance.FindSoundClip(clipName);
            audioSource.loop = true;
            audioSource.Play();

        }
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

    public void modifyActualFeedback()
    {
        if (this.showingFeedback)
        {
            switch (this.type.ToString())
            {
                case "adivinacion":
                    PlaySfx("FairySound");
                    break;
                case "invocacion":
                    PlaySfx("MagicEnchantment");
                    break;
                case "caldero":
                    PlaySfx("BoilingCauldron");
                    break;
                case "encantamiento":
                    PlaySfx("MagicEnchantment");
                    break;
            }
            fx.SetWorking(true);
            actualProgress.transform.position += new Vector3(0, 5000, 0);
        }
        else
        {
            fx.SetWorking(false);
            StopSfx();
            actualProgress.transform.position += new Vector3(0, -5000, 0);
        }
    }

}
