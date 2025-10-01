using System.Collections.Generic;
using UnityEngine;

public class GlobalLocomotionManager : MonoBehaviour {
    public static GlobalLocomotionManager Instance { get; private set; }

    [SerializeField] private string patrolParentName = "PatrolPoints";
    public Transform despawnPoint;
    public List<Transform> PatrolPoints { get; private set; } = new List<Transform>();

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start() {
        InitializePatrolPoints();
    }

    private void InitializePatrolPoints() {
        PatrolPoints.Clear();

        Transform patrolParent = GameObject.Find(patrolParentName)?.transform;
        if (patrolParent != null) {
            foreach (Transform point in patrolParent) {
                PatrolPoints.Add(point);
            }
            Debug.Log($"[GlobalRandomWalkManager] {PatrolPoints.Count} patrol points loaded.");
        } else {
            Debug.LogWarning($"Patrol parent '{patrolParentName}' not found in the scene.");
        }
    }
}
