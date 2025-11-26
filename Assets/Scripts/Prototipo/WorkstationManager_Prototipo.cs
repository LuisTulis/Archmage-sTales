using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class WorkstationManager_Prototipo : MonoBehaviour
{
    public static WorkstationManager_Prototipo Instance { get; private set; }

    public List<WorkStationBehaviour_Prototipo> actualStations;
    public List<WorkStationBehaviour_Prototipo> activeStations;
    public List<StationType> stationTypes;

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
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        actualStations = new List<WorkStationBehaviour_Prototipo>();
        stationTypes = new List<StationType>();
        WorkStationBehaviour_Prototipo[] stations = FindObjectsOfType<WorkStationBehaviour_Prototipo>();

        foreach (WorkStationBehaviour_Prototipo station in stations)
        {
            bool addType = true;
            actualStations.Add(station);

            if(!station.isBroken)
            {
                activeStations.Add(station);
            }
            foreach(StationType type in stationTypes) 
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
                //Debug.Log(station.type);
            }
        }
    }

    public void AddStation(WorkStationBehaviour_Prototipo station) {
        if (station == null) return;

        if (!actualStations.Contains(station)) {
            actualStations.Add(station);

            bool addType = true;
            foreach (StationType type in stationTypes) {
                if (station.type == type) {
                    addType = false;
                    break;
                }
            }

            if (addType) {
                stationTypes.Add(station.type);
                Debug.Log("Nuevo tipo de estación desbloqueado: " + station.type);
            }
        }
    }


}


