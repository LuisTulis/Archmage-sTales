using UnityEngine;

public class GlobalEnemiesManager : MonoBehaviour {
    private static GlobalEnemiesManager Instance;

    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject skeletonPrefab;
    [SerializeField] private GameObject thugPrefab;

    [Header("Enemy Data")]
    [SerializeField] private EnemyData enemyData;


    private int enemyIdCounter;
    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.K)) {
            Vector3 randomPosition = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
            SpawnSkeleton(randomPosition);
        }
        if (Input.GetKeyDown(KeyCode.L)) {
            Vector3 randomPosition = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
            SpawnThug(randomPosition);
        }
    }


    public void SpawnSkeleton(Vector3 position) {
        if (skeletonPrefab == null) {
            Debug.LogError("Skeleton prefab not assigned.");
            return;
        }

        GameObject instance = Instantiate(skeletonPrefab, position, Quaternion.identity);

        SkeletonModel model = instance.GetComponent<SkeletonModel>();
        if (model != null) {
            model.Id = GetNewEnemyId();
            model.Speed = enemyData.Speed;
            model.AttackRange = enemyData.AttackRange;
        } else {
            Debug.LogWarning("El prefab de Skeleton no tiene SkeletonModel asignado.");
        }
    }

    public void SpawnThug(Vector3 position) {
        if (thugPrefab == null) {
            Debug.LogError("Thug prefab not assigned.");
            return;
        }

        GameObject instance = Instantiate(thugPrefab, position, Quaternion.identity);

        ThugModel model = instance.GetComponent<ThugModel>();
        if (model != null) {
            model.Id = GetNewEnemyId();
            model.Speed = enemyData.Speed;
            model.AttackRange = enemyData.AttackRange;
            model.StealAmount = enemyData.stealAmount;
        } else {
            Debug.LogWarning("El prefab de Thug no tiene ThugModel asignado.");
        }
    }

    public int GetNewEnemyId() {
        enemyIdCounter++;
        return enemyIdCounter;
    }
}
