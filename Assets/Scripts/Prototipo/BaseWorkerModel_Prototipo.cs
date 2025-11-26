using UnityEngine;

public class BaseWorkerModel_Prototipo : CharacterModel_Prototipo
{

    //[SerializeField] private WorkStationBehaviour_Prototipo workStation;
    [SerializeField] private int successRate;
    [SerializeField] private WorkerStats stats;

    //public WorkStationBehaviour_Prototipo AsignatedStation { get => workStation; set => workStation = value; }
    public int SuccessRate { get => successRate; set => successRate = value; }
    public WorkerStats Stats { get => stats; set => stats = value; }
}
