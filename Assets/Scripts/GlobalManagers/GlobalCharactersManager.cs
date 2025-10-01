using Assets.Scripts.Helpers;
using System.Collections.Generic;
using UnityEngine;

public class GlobalCharactersManager : MonoBehaviour {
    public static GlobalCharactersManager Instance { get; private set; }

    [Header("Prefabs & Data")]
    [SerializeField] private CharacterData staffAdorData;
    [SerializeField] private CharacterData workerData;
    [SerializeField] GameObject StaffAdorPrefab;
    [SerializeField] GameObject WorkerPrefab;

    [Header("State")]
    [SerializeField] private int workerIdCounter = 1;
    public List<GameObject> Workers = new List<GameObject>();
    public GameObject StaffAdor;
    public List<WorkerModel> Candidates = new List<WorkerModel>();

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

        StaffAdorModel model = instance.GetComponent<StaffAdorModel>();

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
        tempWorker.salary = Random.Range(50, 150);
        return tempWorker;
    }

    public void GenerateCandidates() {
        Candidates.Clear();
        for (int i = 0; i < 3; i++) {
            Candidates.Add(CreateWorkerData());
        }
    }

    public void HireWorker(WorkerModel candidate) {
        GameObject instance = Instantiate(WorkerPrefab, Vector3.zero, Quaternion.identity);

        WorkerModel model = instance.GetComponent<WorkerModel>();
        if (model != null) {
            model.Id = candidate.Id;
            model.Speed = candidate.Speed;
            model.CharacterName = candidate.CharacterName;
            model.salary = candidate.salary;

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
}
