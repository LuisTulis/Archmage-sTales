using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class WorkstationManager : MonoBehaviour
{
    public List<WorkStationBehaviour> actualStations;
    public List<stationType> stationTypes;

    [SerializeField]
    private WorkstationData[] niveles;

    public WorkstationData upgrade(string name)
    {
        string a = "" + name[^1];
        name = name.Split(name[^1])[0];
        string b = (int.Parse(a) + 1).ToString();
        string newLevel = name + b;
        foreach (WorkstationData nivel in niveles)
        {
            if (nivel.name == newLevel)
            {
                return nivel;
            }
        }
        return niveles[0];
    }
    private void Awake()
    {
        actualStations = new List<WorkStationBehaviour>();
        stationTypes = new List<stationType>();
        WorkStationBehaviour[] stations = FindObjectsOfType<WorkStationBehaviour>();

        foreach (WorkStationBehaviour station in stations)
        {
            bool addType = true;
            actualStations.Add(station);
            foreach(stationType type in stationTypes) 
            {
                if(station.type == type)
                {
                    addType = false;
                    break;
                }

            }
            if(addType)
            {
                stationTypes.Add(station.type);
                Debug.Log(station.type);
            }
        }
    }
    
}

public enum stationType
{
    adivinacion,
    caldero,
    invocacion
}
