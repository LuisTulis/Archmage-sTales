using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class AlchemyRoom : MonoBehaviour
{

    [SerializeField] public List<PrePurchaseWorkstation> workstations;

    [Header("Referencias de la Sala")]
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject prePurchaseArea;
    [SerializeField] private GameObject blockFrameDoor;
    [SerializeField] private GameObject openFrameDoor;
    [SerializeField] private GameObject emptyRoom;

    [Header("UI de Confirmación")]
    [SerializeField] private UnlockRoomUI unlockRoomUI;

    [Header("NavMesh")]
    [SerializeField] private NavMeshSurface navMeshSurface;

    [SerializeField] private bool isUnlocked = false;
    private GameManager gameManager;

    [SerializeField] private GameObject leaveRoomPoint;

    [SerializeField] private ParticleSystem purchaseParticle;

    void Start()
    {
        if (prePurchaseArea != null && prePurchaseArea.GetComponent<Collider>() == null)
        {
            prePurchaseArea.AddComponent<BoxCollider>();
        }
        blockFrameDoor.SetActive(true);
        door.SetActive(true);

        gameManager = GameManager.Instance;
    }

    public void OnMouseDown()
    {
        if (!isUnlocked && !gameManager.UIOpen && unlockRoomUI != null)
        {
            unlockRoomUI.Show(this);
        }
    }

    public void ConfirmUnlock(PrePurchaseWorkstation prePurchaseWorkstation)
    {
        if (isUnlocked) return;
        if (GameManager.Instance.horo >= prePurchaseWorkstation.RoomPrice)
        {
            GameManager.Instance.addGold(-prePurchaseWorkstation.RoomPrice);
            //gm.horo_mostrar.text = gm.horo.ToString() + "¤";

            UnlockRoom(prePurchaseWorkstation.Workstation);
            AudioManager.Instance.PlaySound("ComprarSala");
        }
        
    }

    private void UnlockRoom(GameObject workstation)
    {
        if (isUnlocked) return;
        isUnlocked = true;

        if (door != null)
        {
            door.SetActive(false);
        }
        if (blockFrameDoor != null)
            blockFrameDoor.SetActive(false);

        if (openFrameDoor != null)
            openFrameDoor.SetActive(true);

        if (emptyRoom != null)
            emptyRoom.SetActive(false);

        if (prePurchaseArea != null)
            prePurchaseArea.GetComponent<BoxCollider>().enabled = false;

        if (workstation != null)
        {
            workstation.SetActive(true);

            WorkStationBehaviour ws = workstation.GetComponentInChildren<WorkStationBehaviour>();
            if (ws != null)
            {
                GlobalWorkstationManager manager = FindObjectOfType<GlobalWorkstationManager>();
                if (manager != null)
                    ws.isBroken = false;
                manager.AddStation(ws);
                manager.activeStations.Add(ws);
                ws.prepurchaseRoom = this;
            }
        }

        //if (navMeshSurface != null)
        //    navMeshSurface.BuildNavMesh();
        //else
        //{
        //    NavMeshSurface nm = GameObject.Find("Terrain")?.GetComponent<NavMeshSurface>();
        //    if (nm != null)
        //        nm.BuildNavMesh();
        //}
        gameManager.UIOpen = false;

        purchaseParticle.Play(true);
    }

    public void LockRoom(GameObject workstation)
    {
        if (GlobalWorkstationManager.Instance.activeStations.Count <= 1)
        {
            Debug.LogWarning("No se puede cerrar la sala: es la última estación disponible.");
            return;
        }

        if (!isUnlocked) return;
        isUnlocked = false;


        if (door != null)
            door.SetActive(true);

        if (blockFrameDoor != null)
            blockFrameDoor.SetActive(true);

        if (openFrameDoor != null)
            openFrameDoor.SetActive(false);

        if (emptyRoom != null)
            emptyRoom.SetActive(true);

        if (prePurchaseArea != null)
            prePurchaseArea.GetComponent<BoxCollider>().enabled = true;


        if (workstation != null)
        {
            workstation.SetActive(false);

            WorkStationBehaviour ws = workstation.GetComponentInChildren<WorkStationBehaviour>();
            if (ws != null)
            {
                GlobalWorkstationManager manager = FindObjectOfType<GlobalWorkstationManager>();
                if (manager != null)
                {
                    manager.RemoveStation(ws);
                    manager.activeStations.Remove(ws);

                    if (ws.assignedCustomer != null && leaveRoomPoint != null)
                    {
                        var customer = ws.assignedCustomer;
                        var agent = customer.GetComponent<NavMeshAgent>();

                        if (agent != null)
                            agent.Warp(leaveRoomPoint.transform.position);
                        else
                            customer.transform.position = leaveRoomPoint.transform.position;

                        ws.assignedCustomer.LeaveWithoutBuy();

                        ws.assignedCustomer = null;
                        ws.prepurchaseRoom = null;
                    }

                    if (ws.assignedWorker != null)
                    {
                        var worker = ws.assignedWorker;
                        var agent = worker.GetComponent<NavMeshAgent>();

                        if (agent != null)
                            agent.Warp(leaveRoomPoint.transform.position);
                        else
                            worker.transform.position = leaveRoomPoint.transform.position;
                        ws.assignedWorker.isWorking = false;
                        ws.assignedWorker.LeaveWorkStation();
                        ws.assignedWorker = null;
                        ws.assignedWorkerName = "";

                    }
                }
            }
        }
        gameManager.UIOpen = false;
        AudioManager.Instance.PlaySound("CerrarSala");
    }

}
