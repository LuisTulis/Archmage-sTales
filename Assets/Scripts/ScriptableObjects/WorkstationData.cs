using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWorkstationData", menuName = "Workstations/Workstation Data")]
public class WorkstationData : ScriptableObject
{
    public int Id;
    public string displayName;
    //public enum Type???
    public int Speed;
    public int profit;
    public string assignedWorker;
    public string status;
    public float karma;
    public string description;
}
