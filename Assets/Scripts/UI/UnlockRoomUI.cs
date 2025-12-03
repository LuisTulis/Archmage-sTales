using TMPro;
using UnityEngine;

public class UnlockRoomUI : MonoBehaviour
{
    [Header("Referencias de UI")]
    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private TextMeshProUGUI roomPriceText;

    private AlchemyRoom prePurchaseRoom;
    private float closeCooldown = 0;

    public Dialogue afterBuyDialogue;
    private bool firstBuy = true;

    void Start()
    {
        gameObject.SetActive(false);
    }

    public void Show(AlchemyRoom room)
    {
        closeCooldown = .1f;
        prePurchaseRoom = room;

        // Setear los precios de las salas acá

        gameObject.SetActive(true);
        GameManager.Instance.UIOpen = true;

        AudioManager.Instance.PlaySound("Madera1");
    }
    private void Update()
    {
        if(closeCooldown > 0)
        {
            closeCooldown -= Time.deltaTime;
        }
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

        if(firstBuy)
        {
            firstBuy = false;
            DialogueManager.Instance.showDialoge(afterBuyDialogue);
        }

        OnClose();
    }

    public void OnClose()
    {
        if(!(closeCooldown > 0))
        {
            gameObject.SetActive(false);
            GameManager.Instance.UIOpen = false;
        }
    }
}
