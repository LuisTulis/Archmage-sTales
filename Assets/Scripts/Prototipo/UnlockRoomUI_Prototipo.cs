using UnityEngine;
using TMPro;

public class UnlockRoomUI_Prototipo : MonoBehaviour {
    [Header("Referencias de UI")]
    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private TextMeshProUGUI roomPriceText;

    private AlchemyRoom_Prototipo currentRoom;

    void Start() {
        gameObject.SetActive(false);
    }

    public void Show(AlchemyRoom_Prototipo room) {
        currentRoom = room;

        if (roomNameText != null)
            roomNameText.text = room.RoomName;

        if (roomPriceText != null)
            roomPriceText.text = "¤ " + room.RoomPrice.ToString();

        gameObject.SetActive(true);
    }

    public void OnYes() {
        if (currentRoom != null)
            currentRoom.ConfirmUnlock();

        gameObject.SetActive(false);
    }

    public void OnNo() {
        if (currentRoom != null)
            currentRoom.CancelUnlock();

        gameObject.SetActive(false);
    }
}
