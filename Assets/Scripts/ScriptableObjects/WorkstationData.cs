using UnityEngine;

[CreateAssetMenu(fileName = "NewWorkstationData", menuName = "Workstations/Workstation Data")]
public class WorkstationData : ScriptableObject
{
    public int Id;
    public string displayName;
    public int level;
    public int Speed;
    public int profit;
    public int karma;
    public string description;
    public float upgradeCost;
}
