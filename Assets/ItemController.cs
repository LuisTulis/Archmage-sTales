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
    public bool[] activeItemsState;
    public int[] activeItemsDuration;
    public int[] activeItemsUse;
    public static ItemController Instance;
    public int maxItemAmount = 1;

    public GameObject itemPrefab;

    public GameObject backpack;
    public GameObject itemContainer;
    public GameObject itemSelection;

    [SerializeField] public Button useButton;
    public TMP_Text itemName;
    public TMP_Text itemDescription;
    public TMP_Text itemDuration;
    public TMP_Text itemUses;
    public TMP_Text itemStock;

    public Transform gravityTrapPoint;

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
        activeItemsState = new bool[12];
        activeItemsDuration = new int[12];
        activeItemsUse = new int[12];
        itemEntryList = new List<GameObject>();
    }

    

    public void checkActualItems()
    {
        int actualIndex = 0;

        while(actualIndex < 12)
        {
            if (activeItemsState[actualIndex] == true)
            {
                int days = GameManager.Instance.dayCount - activeItemsDuration[actualIndex];
                if (days >= baseItems[actualIndex].dayDuration || activeItemsUse[actualIndex] == 0)
                {
                    activeItemsDuration[actualIndex] = -1;
                    activeItemsUse[actualIndex] = -1;
                    activeItemsState[actualIndex] = false;
                }

            }
            actualIndex++;
        }
    }
    public void closeBackpack()
    {
        itemName.text = "";
        itemDescription.text = "";
        itemDuration.text = "";
        itemUses.text = "";
        itemStock.text = "";
        useButton.interactable = false;
        backpack.SetActive(false);
    }
    public void openBackpack()
    {
        Debug.Log("A");

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
        useButton.interactable = !activeItemsState[selectedIndex];
        itemStock.text = "Usar - " + amount;
        selectedItemIndex = index;
    }

    public void useItem()
    {
        int baseIndex = playerItems[selectedItemIndex][0];


        if (activeItemsState[baseIndex] == false)
        {

            activeItemsUse[baseIndex] = baseItems[baseIndex].uses;
            activeItemsDuration[baseIndex] = GameManager.Instance.dayCount;
            activeItemsState[baseIndex] = true;

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

    public void setGravityTrap(bool setTrap)
    {

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
