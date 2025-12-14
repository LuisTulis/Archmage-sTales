using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WorkstationPanelController : MonoBehaviour, IPointerClickHandler
{
    public GameManager oro;
    public Button upgradeButton;
    public GameObject panel;
    public GameObject workerPanel;
    [SerializeField] Camera cam;
    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text desc;
    [SerializeField] TMP_Text profit;
    [SerializeField] TMP_Text profit_actual;
    private int profit_actual_int;
    [SerializeField] TMP_Text status;
    [SerializeField] TMP_Text speed;
    [SerializeField] TMP_Text karma;
    [SerializeField] TMP_Text worker;
    [SerializeField] LayerMask interactableMask;
    [SerializeField] Slider karmaBar;
    private GlobalWorkstationManager workstationManager;
    private WorkStationBehaviour selectedWorkstation;
    private string asignatedWorkerName = "";

    [Header("Workers UI")]
    [SerializeField] private Transform workersContainer;
    [SerializeField] private GameObject workerEntryPrefab;
    [SerializeField] private Sprite[] stationTypeImage;
    public bool canClose = true;

    private GameObject preSelectedWorker;

    [Header("Worker stats panel")]
    [SerializeField] private GameObject statsPanel;

    [SerializeField] private Image characterImage;
    [SerializeField] private TMP_Text workerStatsName;
    [SerializeField] private TMP_Text workerStatsMental;
    [SerializeField] private Image statusIcon;

    [SerializeField] private TMP_Text speedText;
    [SerializeField] private TMP_Text workingText;
    [SerializeField] private TMP_Text enchantationText;
    [SerializeField] private TMP_Text alchemyText;
    [SerializeField] private TMP_Text invocationText;
    [SerializeField] private TMP_Text adivinationText;

    private void Awake()
    {
        this.workstationManager = GameObject.Find("GlobalWorkstationManager").GetComponent<GlobalWorkstationManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Hide();
        }
        try
        {
            int goldAmount = ItemController.Instance.activeItemsState[3] ? (int)(selectedWorkstation.workstationData.karma * .8f) : selectedWorkstation.workstationData.karma;
            if (goldAmount > oro.horo)
            {
                upgradeButton.interactable = false;
            }
            else
            {
                upgradeButton.interactable = true;
            }
        }
        catch { }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        bool close = true;
        var ray = Camera.main.ScreenPointToRay(eventData.position);
        RaycastHit[] hits = Physics.RaycastAll(ray, 2000f, 3, QueryTriggerInteraction.Ignore);

        foreach (var hit in hits)
        {
            Debug.Log(hit.collider.gameObject);
            var ws = hit.collider.GetComponentInParent<WorkStationBehaviour>();
            if (ws != null)
            {
                Show(ws);
                close = false;
                break;
            }
        }
        if (close)
        {
            if (!workerPanel.activeSelf)
            {
                Hide();
            }
        }

    }
    public void updateStation()
    {
        int goldAmount = ItemController.Instance.activeItemsState[3] ? (int)(selectedWorkstation.workstationData.karma * .8f) : selectedWorkstation.workstationData.karma;
        this.oro.addGold(-goldAmount);
        GameManager.Instance.gastosMesas += goldAmount;
        this.selectedWorkstation.workstationData = workstationManager.upgrade(selectedWorkstation.workstationData.name);
        this.Show(selectedWorkstation);

        selectedWorkstation.UpgradeFX(selectedWorkstation.workstationData.level);
    }

    public void Show(WorkStationBehaviour workstation)
    {

        GameManager.Instance.UIOpen = true;
        AudioManager.Instance.PlaySound("Madera1");

        bool mejoraPua = false;
        bool mejoraOro = false;
        if (ItemController.Instance.activeItemsState[6])
        {
            mejoraPua = true;
        }
        
        if (ItemController.Instance.activeItemsState[1])
        {
            mejoraOro = true;
        }


        selectedWorkstation = workstation;
        var data = workstation.workstationData;
        asignatedWorkerName = workstation.assignedWorkerName;

        WorkstationData nextLevel = workstationManager.upgrade(selectedWorkstation.workstationData.name);
        this.karmaBar.value = workstation.karma;
        var multiplicador_karma = workstation.karma * -.035f;
        profit_actual_int = data.profit + (int)(data.profit * multiplicador_karma);
        profit_actual_int = mejoraOro ? (int)(profit_actual_int * 1.2f) : profit_actual_int;
        title.text = data.displayName;
        desc.text = data.description;
        if (oro.costoso)
        {
            profit_actual_int = (int)(profit_actual_int * .5f);
            profit.text = "¤"+data.profit * .5f;
            profit_actual.text = "¤" + profit_actual_int.ToString();
            if (nextLevel != null)
            {
                profit.text += " -> " + "¤" + nextLevel.profit * .5f;
            }
            profit.color = new Color(1, 1, 0.02830189f);
            profit_actual.color = new Color(1, 1, 0.02830189f);
        }
        else
        {
            profit.text = "¤" + data.profit;
            profit_actual.text = "¤" + profit_actual_int.ToString();

            if (nextLevel != null)
            {
                profit.text += " -> " + "¤" + nextLevel.profit;
            }
            profit.color = new Color(0.02830189f, 0.02830189f, 0.02830189f);
            profit.color = new Color(0.02830189f, 0.02830189f, 0.02830189f);
        }

        status.text = workstation.status;
        int actualSpeed = mejoraPua ? (int)(data.Speed * .8f) : data.Speed;
        if (oro.aletargamiento)
        {
            speed.text = (actualSpeed * 2).ToString() + "s";

            if (nextLevel != null)
            {
                speed.text += " -> " + (nextLevel.Speed * 2).ToString() + "s";
            }
            speed.color = new Color(1, 0.02830189f, 0.02830189f);
        }
        else
        {
            speed.text = actualSpeed.ToString() + "s";
            if (nextLevel != null)
            {
                speed.text += " -> " + nextLevel.Speed.ToString() + "s";
            }
            speed.color = new Color(0.02830189f, 0.02830189f, 0.02830189f);
        }

        int goldAmount = ItemController.Instance.activeItemsState[3] ? (int)(data.karma * .8f) : data.karma;
        karma.text = data.karma < 10000 ? "Upgrade: " + "¤" + goldAmount.ToString(): "Max";
        worker.text = string.IsNullOrEmpty(asignatedWorkerName) ? "Select Worker" : asignatedWorkerName;
        panel.SetActive(true);
    }

    public void Hide()
    {
        this.panel.SetActive(false);
        this.workerPanel.SetActive(false);
        GameManager.Instance.UIOpen = false;
    }

    public void PreSelectWorker(string name)
    {
        GameObject selectedWorkerGO = null;

        // Primero revisamos la lista de Workers
        foreach (var workerGO in GlobalCharactersManager.Instance.Workers)
        {
            var model = workerGO.GetComponent<CharacterModel>();
            if (model != null && model.CharacterName == name)
            {
                selectedWorkerGO = workerGO;
                break;
            }
        }

        // Si no se encontró, revisamos StaffAdor
        if (selectedWorkerGO == null && GlobalCharactersManager.Instance.StaffAdor != null)
        {
            var staffModel = GlobalCharactersManager.Instance.StaffAdor.GetComponent<StaffAdorModel>();
            if (staffModel != null && staffModel.CharacterName == name)
            {
                selectedWorkerGO = GlobalCharactersManager.Instance.StaffAdor;
            }
        }

        // Setear las stats en el stat panel
        BaseWorkerModel workerModel = selectedWorkerGO.GetComponent<BaseWorkerModel>();
        CharacterModel character = selectedWorkerGO.GetComponent<CharacterModel>();

        characterImage.sprite = character.Icon;

        workerStatsName.text = workerModel.CharacterName;
        //workerStatsMental.text = character.Mental.ToString();

        speedText.text = $"Speed: {workerModel.Speed}";
        workingText.text = "Working: " + (character.Idle ? "No" : "Yes");

        enchantationText.text = $"Enchantation: {workerModel.Stats.enchantStat}";
        alchemyText.text = $"Alchemy: {workerModel.Stats.alchemyStat}";
        invocationText.text = $"Invocation: {workerModel.Stats.summonStat}";
        adivinationText.text = $"Adivination: {workerModel.Stats.adivinationStat}";

        var type = workerModel?.AsignatedStation?.type;

        if (type == StationType.caldero)
            statusIcon.sprite = stationTypeImage[0];
        else if (type == StationType.adivinacion)
            statusIcon.sprite = stationTypeImage[1];
        else if (type == StationType.invocacion)
            statusIcon.sprite = stationTypeImage[2];
        else if (type == StationType.encantamiento)
            statusIcon.sprite = stationTypeImage[3];
        else
            statusIcon.sprite = stationTypeImage[4];
    }

    public void selectWorker(string name)
    {
        GameObject selectedWorkerGO = null;

        // Primero revisamos la lista de Workers
        foreach (var workerGO in GlobalCharactersManager.Instance.Workers)
        {
            var model = workerGO.GetComponent<CharacterModel>();
            if (model != null && model.CharacterName == name)
            {
                selectedWorkerGO = workerGO;
                break;
            }
        }

        // Si no se encontró, revisamos StaffAdor
        if (selectedWorkerGO == null && GlobalCharactersManager.Instance.StaffAdor != null)
        {
            var staffModel = GlobalCharactersManager.Instance.StaffAdor.GetComponent<StaffAdorModel>();
            if (staffModel != null && staffModel.CharacterName == name)
            {
                selectedWorkerGO = GlobalCharactersManager.Instance.StaffAdor;
            }
        }

        // Liberar trabajador previamente asignado a la estación
        if (!string.IsNullOrEmpty(selectedWorkstation.assignedWorkerName))
        {
            foreach (var workerGO in GlobalCharactersManager.Instance.Workers)
            {
                var model = workerGO.GetComponent<WorkerModel>();
                if (model != null && model.CharacterName == selectedWorkstation.assignedWorkerName)
                {
                    model.AsignatedStation = null;
                    workerGO.GetComponent<RandomWalkLocomotion>().IdleRandomWalk();
                    break;
                }
            }

            // Revisar StaffAdor también
            if (GlobalCharactersManager.Instance.StaffAdor != null)
            {
                var staffModel = GlobalCharactersManager.Instance.StaffAdor.GetComponent<StaffAdorModel>();
                if (staffModel != null && staffModel.CharacterName == selectedWorkstation.assignedWorkerName)
                {
                    staffModel.AsignatedStation = null;
                    GlobalCharactersManager.Instance.StaffAdor.GetComponent<RandomWalkLocomotion>().IdleRandomWalk();
                }
            }
        }

        // Limpiar otras estaciones que tengan asignado este trabajador
        WorkStationBehaviour[] workstations = GameObject.FindObjectsOfType<WorkStationBehaviour>();
        foreach (WorkStationBehaviour workstation in workstations)
        {
            if (workstation != selectedWorkstation && workstation.assignedWorkerName == name)
            {
                workstation.assignedWorkerName = "";
                workstation.StopAllCoroutines();
                workstation.fx.SetWorking(false);
                workstation.status = "Idle";
            }
        }

        // Asignar trabajador seleccionado a la estación
        if (selectedWorkerGO != null)
        {
            selectedWorkerGO.GetComponent<BaseWorkerComponent>().LeaveWorkStation();
            var model = selectedWorkerGO.GetComponent<BaseWorkerModel>();
            model.AsignatedStation = selectedWorkstation;
            selectedWorkstation.assignedWorkerName = name;
        }

        worker.text = selectedWorkstation.assignedWorkerName;
        workerPanel.SetActive(false);
    }


    public void showWorkers()
    {

        this.workerPanel.SetActive(true);
        PopulateWorkersList();
    }

    private void PopulateWorkersList()
    {
        foreach (Transform child in workersContainer)
        {
            Destroy(child.gameObject);
        }

        var allCharacters = new List<GameObject>();

        allCharacters.AddRange(GlobalCharactersManager.Instance.Workers);

        if (GlobalCharactersManager.Instance.StaffAdor != null)
        {
            allCharacters.Add(GlobalCharactersManager.Instance.StaffAdor);
        }

        foreach (var characterGO in allCharacters)
        {
            var model = characterGO.GetComponent<BaseWorkerModel>();
            if (model == null) continue;

            GameObject entry = Instantiate(workerEntryPrefab, workersContainer);
            TMP_Text nameText = entry.transform.Find("NameText").GetComponent<TMP_Text>();
            Image icon = entry.transform.Find("Image").GetComponent<Image>();
            Button selectButton = entry.transform.Find("SelectButton").GetComponent<Button>();
            if (model.AsignatedStation != null)
            {
                entry.GetComponent<Image>().color = new Color(1, 0, .75f, .4f);
                entry.GetComponentsInChildren<Image>()[2].color = new Color(1, 1, 1, 1);
                if (model.AsignatedStation.type.ToString() == "caldero")
                {
                    entry.GetComponentsInChildren<Image>()[2].sprite = stationTypeImage[0];
                }
                else if (model.AsignatedStation.type.ToString() == "adivinacion")
                {
                    entry.GetComponentsInChildren<Image>()[2].sprite = stationTypeImage[1];
                }
                else if (model.AsignatedStation.type.ToString() == "invocacion")
                {
                    entry.GetComponentsInChildren<Image>()[2].sprite = stationTypeImage[2];
                }
                else
                {
                    entry.GetComponentsInChildren<Image>()[2].sprite = stationTypeImage[3];
                }

            }
            else
            {
                entry.GetComponentsInChildren<Image>()[2].color = new Color(1, 1, 1, 0);
            }

            nameText.text = model.CharacterName;
            icon.sprite = model.Icon;

            string characterName = model.CharacterName; // Capturar variable local para closure
            selectButton.onClick.AddListener(() => selectWorker(characterName));

            entry.GetComponent<Button>().onClick.AddListener(() => PreSelectWorker(characterName));
        }


        PreSelectWorker("Staff Ador");
    }

    public void ChangeKarma()
    {
        selectedWorkstation.karma = karmaBar.value;
        var new_profit = selectedWorkstation.workstationData.profit;
        new_profit = GameManager.Instance.costoso ? (int)(new_profit * .5f) : new_profit;

        profit_actual.text = "¤" + (new_profit + (int)(new_profit * selectedWorkstation.karma * -.035));

    }


    public void tryToClose()
    {
        Hide();
    }

    public void DeleteWorkstation()
    {
        selectedWorkstation.CloseRoom();
    }


}
