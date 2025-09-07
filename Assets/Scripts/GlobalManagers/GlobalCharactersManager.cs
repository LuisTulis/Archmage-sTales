using UnityEngine;

public class GlobalCharactersManager : MonoBehaviour
{
    public static GlobalCharactersManager Instance { get; private set; }

    [SerializeField] private CharacterData staffAdorData;
    [SerializeField] private CharacterData workerData;
    [SerializeField] GameObject StaffAdorPrefab;
    [SerializeField] GameObject WorkerPrefab;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        InitializePlayer();
        InitializeWorker();
    }

    void Start() {
        //InitializePlayer();
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
            model.Id = workerData.Id;
            model.Name = workerData.Name;
            model.Speed = workerData.Speed;
        } else {
            Debug.LogWarning("El prefab no tiene workerData asignado.");
        }
    }

    // Metodos de cracion de Workers, StaffAdor lo instanciamos por defecto, el resto se van a generar aleatoriamente
}


