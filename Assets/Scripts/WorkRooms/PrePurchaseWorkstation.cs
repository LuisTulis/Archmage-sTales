using UnityEngine;

[System.Serializable]
public class PrePurchaseWorkstation
{
    [Header("Datos de la Sala")]
    [SerializeField] private string roomName;
    [SerializeField] private int roomPrice;
    [SerializeField] private GameObject workstation;

    public string RoomName => roomName;
    public int RoomPrice => roomPrice;
    public GameObject Workstation => workstation;
}
