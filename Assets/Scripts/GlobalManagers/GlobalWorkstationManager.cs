using System.Collections.Generic;
using UnityEngine;

public class GlobalWorkstationManager : MonoBehaviour
{
    public static GlobalWorkstationManager Instance { get; private set; }

    public List<WorkStationBehaviour> actualStations;
    public List<AlchemyRoom> allStations;
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
        allStations = new List<AlchemyRoom>();
        stationTypes = new List<StationType>();
        WorkStationBehaviour[] stations = FindObjectsOfType<WorkStationBehaviour>();
        AlchemyRoom[] rooms = FindObjectsOfType<AlchemyRoom>();
        foreach(AlchemyRoom room in rooms)
        {
            //Destroy(room);
            allStations.Add(room);
        }
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

    public void RemoveStation(WorkStationBehaviour station) {
        if (station == null) return;

        if (actualStations.Contains(station)) {
            actualStations.Remove(station);
            Debug.Log("Estación removida: " + station.name);

            bool stillHasType = false;
            foreach (var s in actualStations) {
                if (s.type == station.type) {
                    stillHasType = true;
                    break;
                }
            }

            if (!stillHasType && stationTypes.Contains(station.type)) {
                stationTypes.Remove(station.type);
                Debug.Log("Tipo de estación removido: " + station.type);
            }
        }
    }

    public void showFloorRooms(int floorIndex)
    {

        foreach (AlchemyRoom room in allStations)
        {
            //room.gameObject.transform.localScale = new Vector3(500, 500, 500);
            bool show = room.floorIndex <= floorIndex;
            if(room.purchasedWorkstation != null)
            {
                room.purchasedWorkstation.showingFeedback = room.floorIndex == floorIndex;
                if(room.purchasedWorkstation.status == "Being used")
                {
                    room.purchasedWorkstation.modifyActualFeedback();
                }

            }
            room.GetComponent<BoxCollider>().enabled = show;
            MeshRenderer[] meshes = room.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer mesh in meshes)
            {
                mesh.enabled = show;
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
