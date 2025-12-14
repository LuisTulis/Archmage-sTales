using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnlockStairsUI : MonoBehaviour
{
    [Header("Referencias de UI")]
    [SerializeField] private TextMeshProUGUI priceText;

    private purchaseStairs prePurchaseStairs;
    private float closeCooldown = 0;


    void Start()
    {
        gameObject.SetActive(false);
    }

    public void Show(purchaseStairs purchase)
    {
        closeCooldown = .1f;
        prePurchaseStairs = purchase;
        priceText.text = "¤" + purchase.price;
        // Setear los precios de las salas acá

        gameObject.SetActive(true);
        GameManager.Instance.UIOpen = true;

        AudioManager.Instance.PlaySound("Madera1");
    }
    private void Update()
    {
        if (closeCooldown > 0)
        {
            closeCooldown -= Time.deltaTime;
        }
    }
    public void UnlockRoom()
    {
       
         prePurchaseStairs.ConfirmUnlock();


        OnClose();
    }

    public void OnClose()
    {
        if (!(closeCooldown > 0))
        {
            gameObject.SetActive(false);
            GameManager.Instance.UIOpen = false;
        }
    }
}
