using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class purchaseStairs : MonoBehaviour
{
    [Header("Referencias de la Sala")]
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject prePurchaseArea;
    [SerializeField] private GameObject blockFrameDoor;
    [SerializeField] private GameObject openFrameDoor;
    [SerializeField] private GameObject emptyRoom;

    [Header("UI de Confirmación")]
    [SerializeField] private UnlockStairsUI unlockStairUI;


    [SerializeField] private ParticleSystem purchaseParticle;

    private bool isUnlocked = false;
    public int price;
    void Start()
    {
        if (prePurchaseArea != null && prePurchaseArea.GetComponent<Collider>() == null)
        {
            prePurchaseArea.AddComponent<BoxCollider>();
        }
        blockFrameDoor.SetActive(true);
        door.SetActive(true);

        
    }

    public void OnMouseDown()
    {
        if (!isUnlocked && !GameManager.Instance.UIOpen && unlockStairUI != null)
        {
            unlockStairUI.Show(this);
        }
    }

    public void ConfirmUnlock()
    {
        if (isUnlocked) return;
        if (GameManager.Instance.horo >= price)
        {
            GameManager.Instance.addGold(-price);
            UnlockStair();
            AudioManager.Instance.PlaySound("ComprarSala");
        }

    }

    private void UnlockStair()
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

        GameManager.Instance.maxFloor += 1;
        GameManager.Instance.setFloor(GameManager.Instance.maxFloor);

        
        GameManager.Instance.UIOpen = false;

        purchaseParticle.Play(true);
    }

    
}
