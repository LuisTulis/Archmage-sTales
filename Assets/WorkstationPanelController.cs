using TMPro;
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
    [SerializeField] TMP_Text status;
    [SerializeField] TMP_Text speed;
    [SerializeField] TMP_Text karma;
    [SerializeField] TMP_Text worker;
    [SerializeField] LayerMask interactableMask;
    private WorkstationManager workstationManager;
    private WorkStationBehaviour selectedWorkstation;
    private bool nose = false;
    private string nombresito = "";


    [Header("Workers UI")]
    [SerializeField] private Transform workersContainer;
    [SerializeField] private GameObject workerEntryPrefab;

    private void Awake()
    {
        this.workstationManager = GameObject.Find("WorkstationManager").GetComponent<WorkstationManager>();
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
    }

    public void Show(WorkStationBehaviour workstation)
    {
        selectedWorkstation = workstation;
        var data = workstation.workstationData;
        nombresito = workstation.assignedWorker;
        nose = true;

        title.text = data.displayName;
        desc.text = data.description;
        profit.text = data.profit + "$";
        status.text = workstation.status;
        if(oro.aletargamiento)
        {
            Debug.Log("AA");
            speed.text = (data.Speed * 2).ToString() + "s";
            speed.color = new Color(1, 0.02830189f, 0.02830189f);
        }
        else
        {
            speed.text = data.Speed.ToString() + "s" ;
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
        foreach (var workerGO in GlobalCharactersManager.Instance.Workers)
        {
            var model = workerGO.GetComponent<CharacterModel>();
            if (model != null && model.CharacterName == name)
            {
                selectedWorkerGO = workerGO;
                break;
            }
        }

        if (!string.IsNullOrEmpty(selectedWorkstation.assignedWorker))
        {
            foreach (var workerGO in GlobalCharactersManager.Instance.Workers)
            {
                var model = workerGO.GetComponent<CharacterModel>();
                if (model != null && model.CharacterName == selectedWorkstation.assignedWorker)
                {
                    model.AsignatedStation = null;
                    workerGO.GetComponent<CharacterLocomotion>().IdleRandomWalk();
                    break;
                }
            }
        }

        WorkStationBehaviour[] workstations = GameObject.FindObjectsOfType<WorkStationBehaviour>();
        foreach (WorkStationBehaviour workstation in workstations)
        {
            if (workstation != selectedWorkstation && workstation.assignedWorker == name)
            {
                workstation.assignedWorker = "";
            }
        }

        if (selectedWorkerGO != null)
        {
            var model = selectedWorkerGO.GetComponent<CharacterModel>();
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

    private void PopulateWorkersList() {
        foreach (Transform child in workersContainer) {
            Destroy(child.gameObject);
        }

        foreach (var workerGO in GlobalCharactersManager.Instance.Workers) {
            var model = workerGO.GetComponent<CharacterModel>();
            if (model == null) continue;

            GameObject entry = Instantiate(workerEntryPrefab, workersContainer);
            TMP_Text nameText = entry.transform.Find("NameText").GetComponent<TMP_Text>();
            Button selectButton = entry.transform.Find("SelectButton").GetComponent<Button>();

            nameText.text = model.CharacterName;

            string workerName = model.CharacterName;
            selectButton.onClick.AddListener(() => selectWorker(workerName));
        }
    }
}
