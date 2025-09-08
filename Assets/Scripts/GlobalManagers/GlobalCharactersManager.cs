using Assets.Scripts.Helpers;
using UnityEngine;

public class GlobalCharactersManager : MonoBehaviour
{
    public static GlobalCharactersManager Instance { get; private set; }

    [SerializeField] private CharacterData staffAdorData;
    [SerializeField] private CharacterData workerData;
    [SerializeField] GameObject StaffAdorPrefab;
    [SerializeField] GameObject WorkerPrefab;
    [SerializeField] private int workerIdCounter = 1;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        InitializePlayer();
        InitializeWorker();
        InitializeWorker();

    }

    void InitializePlayer() {
        GameObject instance = Instantiate(StaffAdorPrefab, Vector3.zero, Quaternion.identity);

        StaffAdorModel model = instance.GetComponent<StaffAdorModel>();

        if (model != null) {
            model.Id = staffAdorData.Id;
            model.Name = staffAdorData.Name;
            model.Speed = staffAdorData.Speed;
        } else {
            Debug.LogWarning("El prefab no tiene StaffAdorModel asignado.");
        }
    }

    void InitializeWorker() {
        GameObject instance = Instantiate(WorkerPrefab, Vector3.zero, Quaternion.identity);

        WorkerModel model = instance.GetComponent<WorkerModel>();

        if (model != null) {
            model.Id = GetNewWorkerId();
            model.Speed = workerData.Speed;
            if (model.Id == 3)
            {
                model.AsignatedStation = GameObject.Find("Mesa1").GetComponentInChildren<WorkStationBehaviour>();
                model.Name = "Alejandro Elisei";
            }
            else if(model.Id == 2)
            {
                model.Name = "El Dogthor 😎";
                model.AsignatedStation = GameObject.Find("Mesa2").GetComponentInChildren<WorkStationBehaviour>();
            }
        } else {
            Debug.LogWarning("El prefab no tiene workerData asignado.");
        }
    }

    public int GetNewWorkerId() {
        workerIdCounter++;
        return workerIdCounter;      
    }
}


