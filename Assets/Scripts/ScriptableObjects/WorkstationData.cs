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
    public float profit;
    public CharacterComponent assignedWorker;
    public bool status;
}
