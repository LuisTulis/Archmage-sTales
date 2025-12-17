using Assets.Scripts.Helpers;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder.MeshOperations;

public class GlobalCharactersManager : MonoBehaviour {
    public static GlobalCharactersManager Instance { get; private set; }

    [Header("Prefabs & Data")]
    [SerializeField] private CharacterData staffAdorData;
    [SerializeField] private CharacterData workerData;
    [SerializeField] GameObject StaffAdorPrefab;
    [SerializeField] List<GameObject> WorkerPrefab;

    [Header("State")]
    [SerializeField] private int workerIdCounter = 1;
    public List<GameObject> Workers = new List<GameObject>();
    public GameObject StaffAdor;
    public List<WorkerModel> Candidates = new List<WorkerModel>();

    public CharacterComponent SelectedCharacter;

    public event Action<CharacterComponent> OnCharacterSelected;
    public event Action<CharacterComponent> OnCharacterDeselected;

    private bool lastNormalClayStatus = false;
    private bool lastReinforcedClayStatus = false;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        InitializePlayer();
        GenerateCandidates();
    }

    private void Update() {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) {
            if (SelectedCharacter != null) {
                DeselectCharacter();
            }
        }
        if(lastNormalClayStatus != ItemController.Instance.activeItemsState[8])
        {
            lastNormalClayStatus = ItemController.Instance.activeItemsState[8];
            manageNormalClay(lastNormalClayStatus);
        }
        if (lastReinforcedClayStatus != ItemController.Instance.activeItemsState[9])
        {
            lastReinforcedClayStatus = ItemController.Instance.activeItemsState[9];
            manageReinforcedClay(lastReinforcedClayStatus);
        }
    }

    private void manageNormalClay(bool hire)
    {
        Debug.Log("aca estoy manejando los normalclay -> " + hire);
        if(hire)
        {
            hireClayDoll();
        }
        else
        {
            int workerIndex = 0;
            while(workerIndex < Workers.Count)
            {
                WorkerModel model = Workers[workerIndex].GetComponent<WorkerModel>();
                if (model.clayDoll == 1)
                {
                    Workers[workerIndex].GetComponent<WorkerComponent>().BeKidnapped(GlobalLocomotionManager.Instance.despawnPoint);
                }
                workerIndex++;
            }
        }
    }

    private void manageReinforcedClay(bool hire)
    {
        Debug.Log("aca estoy manejando los normalclay -> " + hire);
        if (hire)
        {
            hireReinforcedClayDoll();
        }
        else
        {
            int workerIndex = 0;
            while (workerIndex < Workers.Count)
            {
                WorkerModel model = Workers[workerIndex].GetComponent<WorkerModel>();
                if (model.clayDoll == 2)
                {
                    Workers[workerIndex].GetComponent<WorkerComponent>().BeKidnapped(GlobalLocomotionManager.Instance.despawnPoint);
                }
                workerIndex++;
            }
        }
    }

    public int getAllSalary()
    {
        int salary = 0;

        foreach (GameObject worker in Workers)
        {
            salary += worker.GetComponent<WorkerModel>().salary;
        }


        return salary;
    }
    void InitializePlayer() {
        GameObject instance = Instantiate(StaffAdorPrefab, Vector3.zero, Quaternion.identity);

        StaffAdorModel model = instance.GetComponent<StaffAdorModel>();
        model.Stats = new WorkerStats(0);
        if (model != null) {
            model.Id = staffAdorData.Id;
            model.CharacterName = staffAdorData.Name;
            model.Speed = staffAdorData.Speed;

            StaffAdor = instance;
        } else {
            Debug.LogWarning("El prefab no tiene StaffAdorModel asignado.");
        }
    }


    private WorkerModel CreateWorkerData() {
        WorkerModel tempWorker = new WorkerModel();
        tempWorker.Id = GetNewWorkerId();
        tempWorker.Speed = workerData.Speed;
        tempWorker.CharacterName = CharacterNameHelper.GetRandomName();
        tempWorker.salary = UnityEngine.Random.Range(50, 150);
        tempWorker.Stats = new WorkerStats(1);
        tempWorker.clayDoll = 0;
        return tempWorker;
    }

    private void hireClayDoll()
    {
        for(int i = 0; i < 5; i ++)
        {
            WorkerModel newClayDoll = new WorkerModel();
            newClayDoll.Id = GetNewWorkerId();
            newClayDoll.Speed = 2;
            newClayDoll.CharacterName = CharacterNameHelper.GetRandomClayName();
            newClayDoll.salary = 0;
            newClayDoll.Stats = new WorkerStats(2);
            newClayDoll.clayDoll = 1;
            HireWorker(newClayDoll);
        }
    }
    private void hireReinforcedClayDoll()
    {
        WorkerModel newClayDoll = new WorkerModel();
        newClayDoll.Id = GetNewWorkerId();
        newClayDoll.Speed = 2;
        newClayDoll.CharacterName = CharacterNameHelper.GetRandomClayName();
        newClayDoll.salary = 0;
        newClayDoll.Stats = new WorkerStats(3);
        newClayDoll.clayDoll = 2;
        HireWorker(newClayDoll);
    }

    public void GenerateCandidates() {
        Candidates.Clear();
        for (int i = 0; i < 3; i++) {
            Candidates.Add(CreateWorkerData());
        }
    }

    public void changeMental(int amount)
    {
        foreach(GameObject worker in Workers)
        {
            WorkerModel mental = worker.GetComponent<WorkerModel>();
            if(mental.clayDoll == 0)
            {
                if (mental.mental >= 0)
                {
                    mental.mental += amount;
                    if (mental.mental < 0)
                    {
                        worker.GetComponentInChildren<WorkerStatus>().setStatus(0);
                    }
                    else
                    {
                        if (amount > 0)
                        {

                            worker.GetComponentInChildren<WorkerStatus>().setStatus(2);
                        }
                        else
                        {
                            worker.GetComponentInChildren<WorkerStatus>().setStatus(1);
                        }
                    }
                }

            }
            
        }
    }

    public void HireWorker(WorkerModel candidate) {
        var randomIndex = UnityEngine.Random.Range(0, WorkerPrefab.Count);
        GameObject instance = Instantiate(WorkerPrefab[randomIndex], Vector3.zero, Quaternion.identity);

        WorkerModel model = instance.GetComponent<WorkerModel>();
        if (model != null) {
            model.Id = candidate.Id;
            model.Speed = candidate.Speed;
            model.CharacterName = candidate.CharacterName;
            model.salary = candidate.salary;
            model.Stats = candidate.Stats;
            model.mental = model.salary > 1 ? 50 : 0;
            model.clayDoll = candidate.clayDoll;

            Workers.Add(instance);
        }

        Candidates.Remove(candidate);
    }

    public void FireWorker(WorkerModel worker) {
        GameObject workerToRemove = Workers.Find(w => w.GetComponent<WorkerModel>().Id == worker.Id);
        if (workerToRemove != null) {
            Workers.Remove(workerToRemove);
        }
    }

    public int GetNewWorkerId() {
        workerIdCounter++;
        return workerIdCounter;
    }

    public void SelectCharacter(CharacterComponent character) {
        if (SelectedCharacter == character)
            return;

        if (SelectedCharacter != null) {
            SelectedCharacter.OnDeselect();
            OnCharacterDeselected?.Invoke(SelectedCharacter);
        }

        SelectedCharacter = character;
        SelectedCharacter.OnSelect();
        OnCharacterSelected?.Invoke(SelectedCharacter);
    }

    public void DeselectCharacter() {
        if (SelectedCharacter == null) return;

        SelectedCharacter.OnDeselect();
        OnCharacterDeselected?.Invoke(SelectedCharacter);
        SelectedCharacter = null;

        CameraControl.Instance.cameraTarget = null;
    }

    
}
