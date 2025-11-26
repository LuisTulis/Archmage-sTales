using Assets.Scripts.Helpers;
using System.Collections.Generic;
using UnityEngine;

public class GlobalCharactersManager_Prototipo : MonoBehaviour {
    public static GlobalCharactersManager_Prototipo Instance { get; private set; }

    [Header("Prefabs & Data")]
    [SerializeField] private CharacterData staffAdorData;
    [SerializeField] private CharacterData workerData;
    [SerializeField] GameObject StaffAdorPrefab;
    [SerializeField] GameObject WorkerPrefab;

    [Header("State")]
    [SerializeField] private int workerIdCounter = 1;
    public List<GameObject> Workers = new List<GameObject>();
    public GameObject StaffAdor;
    public List<WorkerModel_Prototipo> Candidates = new List<WorkerModel_Prototipo>();

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


    void InitializePlayer() {
        GameObject instance = Instantiate(StaffAdorPrefab, Vector3.zero, Quaternion.identity);

        StaffAdorModel_Prototipo model = instance.GetComponent<StaffAdorModel_Prototipo>();

        if (model != null) {
            model.Id = staffAdorData.Id;
            model.CharacterName = staffAdorData.Name;
            model.Speed = staffAdorData.Speed;

            StaffAdor = instance;
        } else {
            Debug.LogWarning("El prefab no tiene StaffAdorModel asignado.");
        }
    }


    private WorkerModel_Prototipo CreateWorkerData() {
        WorkerModel_Prototipo tempWorker = new WorkerModel_Prototipo();
        tempWorker.Id = GetNewWorkerId();
        tempWorker.Speed = Random.Range(1, 3);
        tempWorker.CharacterName = CharacterNameHelper.GetRandomName();
        tempWorker.salary = Random.Range(50, 150);
        return tempWorker;
    }

    public void GenerateCandidates() {
        Candidates.Clear();
        for (int i = 0; i < 3; i++) {
            Candidates.Add(CreateWorkerData());
        }
    }

    public void crearparaelprototipo()
    {
        WorkerModel_Prototipo hola = CreateWorkerData();
        HireWorker(hola);
    }


    public void HireWorker(WorkerModel_Prototipo candidate) {
        GameObject instance = Instantiate(WorkerPrefab, Vector3.zero, Quaternion.identity);

        WorkerModel_Prototipo model = instance.GetComponent<WorkerModel_Prototipo>();
        if (model != null) {
            model.Id = candidate.Id;
            model.Speed = candidate.Speed;
            model.CharacterName = candidate.CharacterName;
            model.salary = candidate.salary;

            Workers.Add(instance);
        }

        Candidates.Remove(candidate);
    }

    public void FireWorker(WorkerModel_Prototipo worker) {
        GameObject workerToRemove = Workers.Find(w => w.GetComponent<WorkerModel_Prototipo>().Id == worker.Id);
        if (workerToRemove != null) {
            Workers.Remove(workerToRemove);
        }
    }

    public int GetNewWorkerId() {
        workerIdCounter++;
        return workerIdCounter;
    }
}
