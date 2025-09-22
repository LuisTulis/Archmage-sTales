using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
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
        catch {}
        

    }
    public void updateStation()
    {
        Debug.Log(selectedWorkstation.ToString());
        this.oro.addGold(-selectedWorkstation.workstationData.karma);
        this.selectedWorkstation.workstationData = workstationManager.upgrade(selectedWorkstation.workstationData.name);
        this.Show(selectedWorkstation.workstationData);
        try
        {
            NavMeshSurface nm = GameObject.Find("Terrain").GetComponent<NavMeshSurface>();
            nm.BuildNavMesh();
        }
        catch{ }
    }
    public void Show(WorkstationData data)
    {
        nombresito = "";
        WorkStationBehaviour[] workstations = GameObject.FindObjectsOfType<WorkStationBehaviour>();
        foreach(WorkStationBehaviour workstation in workstations)
        {
            if(workstation.workstationData.Id == data.Id)
            {
                selectedWorkstation = workstation;
                nombresito = selectedWorkstation.assignedWorker;
                nose = true;
            }
        }
        title.text = data.displayName;
        desc.text = data.description;
        profit.text = data.profit + "$";
        status.text = selectedWorkstation.status;
        speed.text = data.Speed.ToString() + "s";
        if(data.karma < 10000)
        {
            karma.text = "Upgrade: " + data.karma.ToString() + "$";
        }
        else
        {
            karma.text = "Max";
        }
        if(nombresito == "")
        {
            worker.text = "Select Worker";
        }
        else
        {
            worker.text = selectedWorkstation.assignedWorker;
        }
        panel.SetActive(true);
    }

    public void Hide()
    {
        this.panel.SetActive(false);
        this.workerPanel.SetActive(false);

    }

    public void selectWorker(string name)
    {
        string otherName = "";
        WorkStationBehaviour[] workstations = GameObject.FindObjectsOfType<WorkStationBehaviour>();
        foreach(WorkStationBehaviour workstation in workstations)
        {
            
            if(workstation.workstationData.Id == selectedWorkstation.workstationData.Id)
            {
                otherName = workstation.assignedWorker;
                Debug.Log(otherName);
            }
            else
            {
                if (workstation.assignedWorker == name)
                {
                    workstation.assignedWorker = "";
                }
            }
        }

        CharacterComponent[] workers = GameObject.FindObjectsOfType<CharacterComponent>();
        foreach(CharacterComponent character in workers)
        {
            CharacterModel model = character.GetComponent<CharacterModel>();

            if(model.CharacterName == otherName)
            {
                model.AsignatedStation = null;
                character.GetComponent<CharacterLocomotion>().IdleRandomWalk();
            }
            else if(model.CharacterName == name)
            {
                Debug.Log(selectedWorkstation);
                model.AsignatedStation = selectedWorkstation;
                selectedWorkstation.assignedWorker = name;
            }
        }

        worker.text = selectedWorkstation.assignedWorker;
        workerPanel.SetActive(false);

    }
    public void showWorkers()
    {

        this.workerPanel.SetActive(true);
    }
}
