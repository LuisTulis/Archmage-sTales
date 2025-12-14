using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

    private void Awake()
    {
        this.status = "Idle";
        infoPanel = GameObject.Find("WorkStationUI").GetComponent<WorkstationPanelController>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        //gameManager.addGold(5);
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
            if(GameManager.Instance.isPlaying && !GameManager.Instance.isPaused)
            {
                elapsed += Time.deltaTime;
                if (actualProgress != null)
                {
                    actualProgress.GetComponent<progressBar>().progress = elapsed * 100 / seconds;
                }
            }
            
            yield return null;
        }

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
        realProfit = (int)(realProfit * mejoraOro);
        realProfit = realProfit + (int)(realProfit * (karma * -0.035f));
        gameManager.realKarma += karma / 10;

        if(gameManager.realKarma > 50)
        {
            gameManager.realKarma = 50;
        }
        else if(gameManager.realKarma < -50)
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
        this.status = "Idle";
        this.clientUsing = 0;
        if (fx) fx.SetWorking(false);
        StopSfx();
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
        if(showingFeedback)
        {
            audioSource.clip = AudioManager.Instance.FindSoundClip(clipName);
            audioSource.loop = true;
            audioSource.Play();

        }
    }

    private void StopSfx()
    {
        audioSource.Stop();
    }

    public void modifyActualFeedback()
    {
        if(this.showingFeedback)
        {
            switch(this.type.ToString())
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
