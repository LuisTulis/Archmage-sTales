using UnityEngine;

public class BaseWorkerModel : CharacterModel
{

    [SerializeField] private WorkStationBehaviour workStation;
    [SerializeField] private int successRate;
    [SerializeField] private WorkerStats stats;

    public WorkStationBehaviour AsignatedStation { get => workStation; set => workStation = value; }
    public int SuccessRate { get => successRate; set => successRate = value; }
    public WorkerStats Stats { get => stats; set => stats = value; }
}
