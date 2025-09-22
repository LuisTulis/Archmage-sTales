using UnityEngine;
using TMPro;

public class UnlockRoomUI : MonoBehaviour {
    [Header("Referencias de UI")]
    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private TextMeshProUGUI roomPriceText;

    private AlchemyRoom currentRoom;

    void Start() {
        gameObject.SetActive(false);
    }

    public void Show(AlchemyRoom room) {
        currentRoom = room;

        if (roomNameText != null)
            roomNameText.text = room.RoomName;

        if (roomPriceText != null)
            roomPriceText.text = "$ " + room.RoomPrice.ToString();

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
