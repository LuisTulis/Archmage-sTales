using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WorkstationPanelController_Prototipo : MonoBehaviour
{
    public GameManager_Prototipo oro;
    public Button upgradeButton;
    public GameObject panel;
    public GameObject workerPanel;
    [SerializeField] Camera cam;
    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text desc;
    [SerializeField] TMP_Text profit;
    [SerializeField] TMP_Text status;
    [SerializeField] TMP_Text speed;
    [SerializeField] TMP_Text karma;
    [SerializeField] TMP_Text worker;
    [SerializeField] LayerMask interactableMask;
    private WorkstationManager_Prototipo workstationManager;
    private WorkStationBehaviour_Prototipo selectedWorkstation;
    private string nombresito = "";


    [Header("Workers UI")]
    [SerializeField] private Transform workersContainer;
    [SerializeField] private GameObject workerEntryPrefab;

    private void Awake()
    {
        this.workstationManager = GameObject.Find("WorkstationManager").GetComponent<WorkstationManager_Prototipo>();
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
        this.selectedWorkstation.workstationData = workstationManager.upgrade(selectedWorkstation.workstationData.name);
        this.Show(selectedWorkstation);

        selectedWorkstation.UpgradeFX(selectedWorkstation.workstationData.level);
        //selectedWorkstation.fx.SetOn(true);
    }

    public void Show(WorkStationBehaviour_Prototipo workstation)
    {
        selectedWorkstation = workstation;
        var data = workstation.workstationData;
        nombresito = workstation.assignedWorker;

        title.text = data.displayName;
        desc.text = data.description;
        if (oro.costoso)
        {
            Debug.Log("AA");
            profit.text = data.profit * .5f + "$";
            profit.color = new Color(1, 1, 0.02830189f);
        }
        else
        {
            profit.text = data.profit + "$";
            profit.color = new Color(0.02830189f, 0.02830189f, 0.02830189f);
        }
        status.text = workstation.status;
        if (oro.aletargamiento)
        {
            Debug.Log("AA");
            speed.text = (data.Speed * 2).ToString() + "s";
            speed.color = new Color(1, 0.02830189f, 0.02830189f);
        }
        else
        {
            speed.text = data.Speed.ToString() + "s";
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

    }

    public void selectWorker(string name)
    {
        GameObject selectedWorkerGO = null;

        // Primero revisamos la lista de Workers
        foreach (var workerGO in GlobalCharactersManager_Prototipo.Instance.Workers)
        {
            var model = workerGO.GetComponent<CharacterModel_Prototipo>();
            if (model != null && model.CharacterName == name)
            {
                selectedWorkerGO = workerGO;
                break;
            }
        }

        // Si no se encontró, revisamos StaffAdor
        if (selectedWorkerGO == null && GlobalCharactersManager_Prototipo.Instance.StaffAdor != null)
        {
            var staffModel = GlobalCharactersManager_Prototipo.Instance.StaffAdor.GetComponent<CharacterModel_Prototipo>();
            if (staffModel != null && staffModel.CharacterName == name)
            {
                selectedWorkerGO = GlobalCharactersManager_Prototipo.Instance.StaffAdor;
            }
        }

        // Liberar trabajador previamente asignado a la estación
        if (!string.IsNullOrEmpty(selectedWorkstation.assignedWorker))
        {
            foreach (var workerGO in GlobalCharactersManager_Prototipo.Instance.Workers)
            {
                var model = workerGO.GetComponent<CharacterModel_Prototipo>();
                if (model != null && model.CharacterName == selectedWorkstation.assignedWorker)
                {
                    model.AsignatedStation = null;
                    workerGO.GetComponent<RandomWalkLocomotion>().IdleRandomWalk();
                    break;
                }
            }

            // Revisar StaffAdor también
            if (GlobalCharactersManager_Prototipo.Instance.StaffAdor != null)
            {
                var staffModel = GlobalCharactersManager_Prototipo.Instance.StaffAdor.GetComponent<CharacterModel_Prototipo>();
                if (staffModel != null && staffModel.CharacterName == selectedWorkstation.assignedWorker)
                {
                    staffModel.AsignatedStation = null;
                    GlobalCharactersManager_Prototipo.Instance.StaffAdor.GetComponent<RandomWalkLocomotion>().IdleRandomWalk();
                }
            }
        }

        // Limpiar otras estaciones que tengan asignado este trabajador
        WorkStationBehaviour_Prototipo[] workstations = GameObject.FindObjectsOfType<WorkStationBehaviour_Prototipo>();
        foreach (WorkStationBehaviour_Prototipo workstation in workstations)
        {
            if (workstation != selectedWorkstation && workstation.assignedWorker == name)
            {
                workstation.assignedWorker = "";
            }
        }

        // Asignar trabajador seleccionado a la estación
        if (selectedWorkerGO != null)
        {
            var model = selectedWorkerGO.GetComponent<CharacterModel_Prototipo>();
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

        allCharacters.AddRange(GlobalCharactersManager_Prototipo.Instance.Workers);

        if (GlobalCharactersManager_Prototipo.Instance.StaffAdor != null)
        {
            allCharacters.Add(GlobalCharactersManager_Prototipo.Instance.StaffAdor);
        }

        foreach (var characterGO in allCharacters)
        {
            var model = characterGO.GetComponent<CharacterModel_Prototipo>();
            if (model == null) continue;

            GameObject entry = Instantiate(workerEntryPrefab, workersContainer);
            TMP_Text nameText = entry.GetComponent<TMP_Text>();
            Button selectButton = entry.GetComponent<Button>();
            //if (model.AsignatedStation != null)
            //{
            //    entry.GetComponent<Image>().color = new Color(1, 0, .75f, .4f);
            //}

            nameText.text = model.CharacterName;

            string characterName = model.CharacterName; // Capturar variable local para closure
            selectButton.onClick.AddListener(() => selectWorker(characterName));
        }
    }


}
