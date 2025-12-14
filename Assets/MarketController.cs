using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MarketController : MonoBehaviour
{
    public GameObject chooseMarketMenu;
    public GameObject actualMarket;
    public GameObject itemEntryPrefab;

    public Image actualMarketBackground;
    public Image actualMarketSeller;

    public List<Market> marketList;
    public GameObject sellerObject;
    public GameObject itemsObject;
    public GameObject backButton;

    public GameObject confirmBuyPanel;
    public TMP_Text itemName;
    public TMP_Text itemDesc;
    public TMP_Text itemPrice;

    private int actualBuying;
    private void Update()
    {
        if(DialogueManager.Instance.currentDialogue != null)
        {
            if(sellerObject.activeSelf)
            {
                sellerObject.SetActive(false);
                itemsObject.SetActive(false);
                backButton.SetActive(false);
            }
        }
        else
        {
            if (!sellerObject.activeSelf)
            {
                sellerObject.SetActive(true);
                itemsObject.SetActive(true);
                backButton.SetActive(true);
            }
        }
    }
    public void confirmBuy()
    {
        int precio = ItemController.Instance.baseItems[actualBuying].price;
        if (GameManager.Instance.horo >= precio)
        {
           
            bool newItem = true;
            for(int i = 0; i < ItemController.Instance.playerItems.Count; i++)
            {
                if (ItemController.Instance.playerItems[i][0] == actualBuying)
                {
                    ItemController.Instance.playerItems[i][1]++;
                    GameManager.Instance.addGold(-precio);
                    newItem = false;
                    break;
                }
                
            }
            if(newItem)
            {
                if (ItemController.Instance.maxItemAmount > ItemController.Instance.playerItems.Count)
                {
                    List<int> newListItem = new List<int>();
                    newListItem.Add(actualBuying);
                    newListItem.Add(1);
                    ItemController.Instance.playerItems.Add(newListItem);
                    GameManager.Instance.addGold(-precio);
                }
            }

        }
        else
        {
            // hay sonido de no poder comprar? 
        }
        
        
    }

    public void cancelBuy()
    {
        confirmBuyPanel.SetActive(false);
    }

    public void goToShop()
    {
        GameManager.Instance.closeMarket();
    }
    public void deselectMarket()
    {
        chooseMarketMenu.SetActive(true);
        actualMarket.SetActive(false);
    }

    public void chooseMarket(int marketIndex)
    {
        Market selectedMarket = marketList[marketIndex];

        actualMarket.SetActive(true);
        chooseMarketMenu.SetActive(false);
        actualMarketBackground.sprite = selectedMarket.background;
        actualMarketSeller.sprite = selectedMarket.seller;
        if(!selectedMarket.firstDialogueFlag)
        {
            marketList[marketIndex].firstDialogueFlag = true;
            DialogueManager.Instance.showDialoge(selectedMarket.firstDialogue);
        }
        else
        {
            DialogueManager.Instance.showDialoge(selectedMarket.secondDialogue);
        }
        int addedItems = 0;
        while(addedItems < selectedMarket.itemList.Length)
        {
            int actualIndex = selectedMarket.itemList[addedItems];
            GameObject newItemEntry = Instantiate(itemEntryPrefab, itemsObject.transform);
            newItemEntry.GetComponent<Image>().sprite = ItemController.Instance.baseItems[actualIndex].itemImage;
            newItemEntry.GetComponent<Button>().onClick.AddListener(() => showEntry(actualIndex));
            addedItems += 1;

        }
    }

    public void showEntry(int itemIndex)
    {
        actualBuying = itemIndex;
        Item selectedItem = ItemController.Instance.baseItems[itemIndex];
        confirmBuyPanel.SetActive(true);
        itemName.text = selectedItem.name;
        itemDesc.text = selectedItem.description;
        itemPrice.text = "¤" + selectedItem.price;
    }

}

[System.Serializable]
public class Market
{
    public Sprite background;
    public Sprite seller;
    public Dialogue firstDialogue;
    public Dialogue secondDialogue;
    public bool firstDialogueFlag;
    public int[] itemList;
}


