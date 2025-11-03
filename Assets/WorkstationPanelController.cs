using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WorkstationPanelController : MonoBehaviour
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
    [SerializeField] TMP_Text status;
    [SerializeField] TMP_Text speed;
    [SerializeField] TMP_Text karma;
    [SerializeField] TMP_Text worker;
    [SerializeField] LayerMask interactableMask;
    [SerializeField] Slider karmaBar;
    private GlobalWorkstationManager workstationManager;
    private WorkStationBehaviour selectedWorkstation;
    private string nombresito = "";


    [Header("Workers UI")]
    [SerializeField] private Transform workersContainer;
    [SerializeField] private GameObject workerEntryPrefab;
    [SerializeField] private Sprite[] stationTypeImage;

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
            if (selectedWorkstation.workstationData.karma > oro.horo)
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
    public void updateStation()
    {
        Debug.Log(selectedWorkstation.ToString());
        Debug.Log(selectedWorkstation.workstationData.karma.ToString());
        this.oro.addGold(-selectedWorkstation.workstationData.karma);
        GameManager.Instance.gastosMesas += selectedWorkstation.workstationData.karma;
        this.selectedWorkstation.workstationData = workstationManager.upgrade(selectedWorkstation.workstationData.name);
        this.Show(selectedWorkstation);

        selectedWorkstation.UpgradeFX(selectedWorkstation.workstationData.level);
    }

    public void Show(WorkStationBehaviour workstation)
    {
        GameManager.Instance.UIOpen = true;
        selectedWorkstation = workstation;
        var data = workstation.workstationData;
        nombresito = workstation.assignedWorker;

        WorkstationData nextLevel = workstationManager.upgrade(selectedWorkstation.workstationData.name);
        this.karmaBar.value = workstation.karma;
        title.text = data.displayName;
        desc.text = data.description;
        if (oro.costoso)
        {
            profit.text = data.profit * .5f + "$";
            profit_actual.text = profit.text;
            if (nextLevel != null)
            {
                profit.text += " -> " + nextLevel.profit * .5f + "$";
            }
            profit.color = new Color(1, 1, 0.02830189f);
            profit_actual.color = new Color(1, 1, 0.02830189f);
        }
        else
        {
            profit.text = data.profit + "$";
            profit_actual.text = profit.text;

            if (nextLevel != null)
            {
                profit.text += " -> " + nextLevel.profit + "$";
            }
            profit.color = new Color(0.02830189f, 0.02830189f, 0.02830189f);
            profit.color = new Color(0.02830189f, 0.02830189f, 0.02830189f);
        }
        status.text = workstation.status;
        if (oro.aletargamiento)
        {
            speed.text = (data.Speed * 2).ToString() + "s";

            if (nextLevel != null)
            {
                speed.text += " -> " + (nextLevel.Speed * 2).ToString() + "s";
            }
            speed.color = new Color(1, 0.02830189f, 0.02830189f);
        }
        else
        {
            speed.text = data.Speed.ToString() + "s";
            if (nextLevel != null)
            {
                speed.text += " -> " + nextLevel.Speed.ToString() + "s";
            }
            speed.color = new Color(0.02830189f, 0.02830189f, 0.02830189f);
        }
        karma.text = data.karma < 10000 ? "Upgrade: " + data.karma.ToString() + "$" : "Max";
        worker.text = string.IsNullOrEmpty(nombresito) ? "Select Worker" : nombresito;
        panel.SetActive(true);
    }

    public void Hide()
    {
        this.panel.SetActive(false);
        this.workerPanel.SetActive(false);
        GameManager.Instance.UIOpen = false;

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
        if (!string.IsNullOrEmpty(selectedWorkstation.assignedWorker))
        {
            foreach (var workerGO in GlobalCharactersManager.Instance.Workers)
            {
                var model = workerGO.GetComponent<WorkerModel>();
                if (model != null && model.CharacterName == selectedWorkstation.assignedWorker)
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
                if (staffModel != null && staffModel.CharacterName == selectedWorkstation.assignedWorker)
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
            if (workstation != selectedWorkstation && workstation.assignedWorker == name)
            {
                workstation.assignedWorker = "";
                workstation.StopAllCoroutines();
                workstation.fx.SetWorking(false);
                workstation.status = "Idle";
            }
        }

        // Asignar trabajador seleccionado a la estación
        if (selectedWorkerGO != null)
        {
            selectedWorkerGO.GetComponent<BaseWorkerComponent>().LeaveWorkSation();
            var model = selectedWorkerGO.GetComponent<BaseWorkerModel>();
            model.AsignatedStation = selectedWorkstation;
            selectedWorkstation.assignedWorker = name;
        }

        worker.text = selectedWorkstation.assignedWorker;
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
            Button selectButton = entry.transform.Find("SelectButton").GetComponent<Button>();
            if (model.AsignatedStation != null)
            {
                entry.GetComponent<Image>().color = new Color(1, 0, .75f, .4f);
                entry.GetComponentsInChildren<Image>()[2].color = new Color(1, 1, 1, 1);
                if (model.AsignatedStation.type.ToString() == "caldero")
                {
                    entry.GetComponentsInChildren<Image>()[2].sprite = stationTypeImage[0];
                }
                else if(model.AsignatedStation.type.ToString() == "adivinacion")
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

            string characterName = model.CharacterName; // Capturar variable local para closure
            selectButton.onClick.AddListener(() => selectWorker(characterName));
        }
    }

    public void ChangeKarma()
    {
        selectedWorkstation.karma = karmaBar.value;
    }
    
}
