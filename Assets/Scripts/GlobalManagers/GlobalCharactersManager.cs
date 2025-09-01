using UnityEngine;

public class GlobalCharactersManager : MonoBehaviour
{
    public static GlobalCharactersManager Instance { get; private set; }

    [SerializeField] private CharacterData staffAdorData;

    public StaffAdorModel Player { get; private set; }
    [SerializeField] GameObject StaffAdorPrefab;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        InitializePlayer();
        Debug.Log($"Player initialized: {Player.Name} with Speed {Player.Speed}");
    }

    void InitializePlayer() {
        Player = new StaffAdorModel {
            Id = staffAdorData.Id,
            Name = staffAdorData.Name,
            Speed = staffAdorData.Speed,
            //Stats = new Stats {
            //},
        };

        Instantiate(StaffAdorPrefab, new Vector3(0,0,0), Quaternion.identity);
    }

    // Metodos de cracion de Workers, StaffAdor lo instanciamos por defecto, el resto se van a generar aleatoriamente
}
