using Unity.AI.Navigation;
using UnityEngine;

public class AlchemyRoom : MonoBehaviour {
    [Header("Datos de la Sala")]
    [SerializeField] private string roomName = "Alchemy Room";
    [SerializeField] private int roomPrice = 20;

    [Header("Referencias de la Sala")]
    [SerializeField] private GameObject alchemyStation;
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject prePurchaseArea;

    [Header("UI de Confirmación")]
    [SerializeField] private UnlockRoomUI unlockRoomUI;

    [Header("NavMesh")]
    [SerializeField] private NavMeshSurface navMeshSurface; 

    private bool isUnlocked = false;

    public string RoomName => roomName;
    public int RoomPrice => roomPrice;

    void Start() {
        if (alchemyStation != null)
            alchemyStation.gameObject.SetActive(false);

        if (prePurchaseArea != null && prePurchaseArea.GetComponent<Collider>() == null) {
            prePurchaseArea.AddComponent<BoxCollider>();
        }
    }

    void Update() {
        if (!isUnlocked && Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider.gameObject == prePurchaseArea) {
                    if (unlockRoomUI != null)
                        unlockRoomUI.Show(this);
                }
            }
        }
    }

    public void ConfirmUnlock() {
        if (isUnlocked) return;

        GameManager gm = FindObjectOfType<GameManager>();

        if (gm != null) {
            if (gm.horo >= roomPrice) {
                gm.horo -= roomPrice;
                gm.horo_mostrar.text = gm.horo.ToString() + "$";

                UnlockRoom();
            } else {
                if (gm.feedbackPrefab != null) {
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

    private void UnlockRoom() {
        if (isUnlocked) return;
        isUnlocked = true;

        if (door != null) {
            door.SetActive(false);
        }

        if (prePurchaseArea != null)
            prePurchaseArea.SetActive(false);

        if (alchemyStation != null) {
            alchemyStation.SetActive(true);

            WorkStationBehaviour ws = alchemyStation.GetComponentInChildren<WorkStationBehaviour>();
            if (ws != null) {
                WorkstationManager manager = FindObjectOfType<WorkstationManager>();
                if (manager != null)
                    manager.AddStation(ws);
            }
        }

        if (navMeshSurface != null)
            navMeshSurface.BuildNavMesh();
        else {
            NavMeshSurface nm = GameObject.Find("Terrain")?.GetComponent<NavMeshSurface>();
            if (nm != null)
                nm.BuildNavMesh();
        }
    }


    public void CancelUnlock() {
    }
}
