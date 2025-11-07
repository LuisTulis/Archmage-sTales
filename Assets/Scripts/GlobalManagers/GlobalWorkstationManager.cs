using System.Collections.Generic;
using UnityEngine;

public class GlobalWorkstationManager : MonoBehaviour
{
    public static GlobalWorkstationManager Instance { get; private set; }

    public List<WorkStationBehaviour> actualStations;
    public List<WorkStationBehaviour> activeStations;
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
        return null;
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        actualStations = new List<WorkStationBehaviour>();
        activeStations = new List<WorkStationBehaviour>();
        stationTypes = new List<StationType>();
        WorkStationBehaviour[] stations = FindObjectsOfType<WorkStationBehaviour>();

        foreach (WorkStationBehaviour station in stations)
        {
            bool addType = true;
            actualStations.Add(station);

            if (!station.isBroken)
            {
                activeStations.Add(station);
            }
            foreach (StationType type in stationTypes)
            {
                if (station.type == type)
                {
                    addType = false;
                    break;
                }

            }
            if (addType)
            {
                stationTypes.Add(station.type);
                //Debug.Log(station.type);
            }
        }
    }

    public void AddStation(WorkStationBehaviour station)
    {
        if (station == null) return;

        if (!actualStations.Contains(station))
        {
            actualStations.Add(station);

            bool addType = true;
            foreach (StationType type in stationTypes)
            {
                if (station.type == type)
                {
                    addType = false;
                    break;
                }
            }

            if (addType)
            {
                stationTypes.Add(station.type);
                Debug.Log("Nuevo tipo de estación desbloqueado: " + station.type);
            }
        }
    }


}

public enum StationType
{
    adivinacion,
    caldero,
    invocacion,
    encantamiento
}
