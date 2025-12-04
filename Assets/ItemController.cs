using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ItemController : MonoBehaviour
{
    public Item[] baseItems;
    public List<List<int>> playerItems;
    public List<List<int>> activeItems;
    public static ItemController Instance;
    public int maxItemAmount = 1;

    public GameObject itemPrefab;

    public GameObject backpack;
    public GameObject itemContainer;
    public GameObject itemSelection;

    public TMP_Text itemName;
    public TMP_Text itemDescription;
    public TMP_Text itemDuration;
    public TMP_Text itemUses;
    public TMP_Text itemStock;

    private int selectedItemIndex;
    private List<GameObject> itemEntryList;
    void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
        playerItems = new List<List<int>>();
        activeItems = new List<List<int>>();
        itemEntryList = new List<GameObject>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            openBackpack();
        }
    }

    public void openBackpack()
    {
        int addedItems = 0;
        backpack.SetActive(true);
        
        while(itemEntryList.Count > 0)
        {
            Destroy(itemEntryList[0]);
            itemEntryList.RemoveAt(0);
        }
             

        while(addedItems < playerItems.Count)
        {
            GameObject newItem = Instantiate(itemPrefab, itemContainer.transform);
            int actualIndex = playerItems[addedItems][0];
            newItem.GetComponent<Image>().sprite = baseItems[actualIndex].itemImage;
            int testingInt = addedItems;
            Debug.Log(testingInt);
            newItem.GetComponent<Button>().onClick.AddListener(() => selectItem(testingInt));
            itemEntryList.Add(newItem);
            addedItems++;

        }
    }
    public void selectItem(int index)
    {
        int selectedIndex = playerItems[index][0];
        int amount = playerItems[index][1];
        itemName.text = baseItems[selectedIndex].name;
        itemDescription.text = baseItems[selectedIndex].useDescription;
        itemDuration.text = baseItems[selectedIndex].dayDuration + " días";
        itemUses.text = baseItems[selectedIndex].uses == -1 ? "" : baseItems[selectedIndex].uses == 1 ? "Único Uso" : baseItems[selectedIndex].uses + " usos";
        itemStock.text = "Usar - " + amount;
        selectedItemIndex = index;
    }

    public void useItem()
    {
        int actualIndex = 0;
        bool canUse = true;
        while(actualIndex < activeItems.Count)
        {
            if (activeItems[actualIndex][0] == selectedItemIndex)
            {
                canUse = false;
            }
            actualIndex++;
        }

        if(canUse)
        {
            List<int> newActiveItem = new List<int>();
            newActiveItem.Add(selectedItemIndex);
            newActiveItem.Add(GameManager.Instance.dayCount);
            activeItems.Add(newActiveItem);
            playerItems[selectedItemIndex][1]--;
            if (playerItems[selectedItemIndex][1] == 0)
            {
                playerItems.RemoveAt(selectedItemIndex);
                selectedItemIndex = -1;
            }
            if (selectedItemIndex != -1)
            {
                selectItem(selectedItemIndex);
            }
            openBackpack();

        }
                
        
    }
}

[System.Serializable]
public class Item
{
    public Sprite itemImage;
    public int price;
    public string name;
    public string description;

    public string useDescription;
    public int dayDuration;
    public int uses;
    private int initialDay;
}
