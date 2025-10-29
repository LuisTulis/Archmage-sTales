using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

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
    void Start()
    {
        if (prePurchaseArea != null && prePurchaseArea.GetComponent<Collider>() == null)
        {
            prePurchaseArea.AddComponent<BoxCollider>();
        }
        gameManager = GameManager.Instance;
    }

    public void OnMouseDown()
    {
        if (!isUnlocked && !gameManager.UIOpen && unlockRoomUI != null)
            unlockRoomUI.Show(this);
    }

    public void ConfirmUnlock(PrePurchaseWorkstation prePurchaseWorkstation)
    {
        if (isUnlocked) return;

        GameManager gm = FindObjectOfType<GameManager>();

        if (gm != null)
        {
            if (gm.horo >= prePurchaseWorkstation.RoomPrice)
            {
                gm.horo -= prePurchaseWorkstation.RoomPrice;
                gm.horo_mostrar.text = gm.horo.ToString() + "$";

                UnlockRoom(prePurchaseWorkstation.Workstation);
            }
            else
            {
                if (gm.feedbackPrefab != null)
                {
                    GameObject instance = Instantiate(
                        gm.feedbackPrefab,
                        gm.feedbackPlacement.transform.position,
                        Quaternion.identity,
                        gm.canvas.transform
                    );
                    instance.GetComponent<goldFeedback>().amount = 0;
                }
            }
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
            prePurchaseArea.SetActive(false);

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
            }
        }

        if (navMeshSurface != null)
            navMeshSurface.BuildNavMesh();
        else
        {
            NavMeshSurface nm = GameObject.Find("Terrain")?.GetComponent<NavMeshSurface>();
            if (nm != null)
                nm.BuildNavMesh();
        }
        gameManager.UIOpen = false;
    }

}
