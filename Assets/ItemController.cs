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
    public int[] activeItems;
    public static ItemController Instance;
    public int maxItemAmount = 3;

    public GameObject itemPrefab;

    public GameObject backpack;
    public GameObject itemContainer;
    public GameObject itemSelection;

    public TMP_Text itemName;
    public TMP_Text itemDescription;
    public TMP_Text itemDuration;
    public TMP_Text itemUses;
    public TMP_Text itemStock;
    void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
        playerItems = new List<List<int>>();
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
        while(addedItems < playerItems.Count)
        {
            GameObject newItem = Instantiate(itemPrefab, itemContainer.transform);
            int actualIndex = playerItems[addedItems][0];
            newItem.GetComponent<Image>().sprite = baseItems[actualIndex].itemImage;
            newItem.GetComponent<Button>().onClick.AddListener(() => selectItem(actualIndex));
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
