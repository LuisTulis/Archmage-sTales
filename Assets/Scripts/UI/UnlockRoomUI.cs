using TMPro;
using UnityEngine;

public class UnlockRoomUI : MonoBehaviour
{
    [Header("Referencias de UI")]
    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private TextMeshProUGUI roomPriceText;

    private AlchemyRoom prePurchaseRoom;

    void Start()
    {
        gameObject.SetActive(false);
    }

    public void Show(AlchemyRoom room)
    {
        prePurchaseRoom = room;

        // Setear los precios de las salas acá

        gameObject.SetActive(true);
        GameManager.Instance.UIOpen = true;
    }

    public void UnlockRoom(string roomName)
    {
        PrePurchaseWorkstation workstation = prePurchaseRoom.workstations.Find(w => w.RoomName.Equals(roomName));
        if (prePurchaseRoom != null)
        {
            Debug.Log("Comprando nueva sala: " + workstation.RoomName);
            prePurchaseRoom.ConfirmUnlock(workstation);
        }
        else
        {
            Debug.Log("ERROR: prePurchaseRoom is missing.");
        }

        OnClose();
    }

    public void OnClose()
    {
        gameObject.SetActive(false);
        GameManager.Instance.UIOpen = false;
    }
}
