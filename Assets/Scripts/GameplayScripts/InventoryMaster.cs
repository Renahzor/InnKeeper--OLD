using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class InventoryMaster : MonoBehaviour {

    public static InventoryMaster Instance { get; private set; }

    public Dictionary<int, int> inventoryTracker = new Dictionary<int, int>();//id, qty
    public List<Button> inventoryButtons = new List<Button>();
    public GameObject inventoryWindow;
    public List<Sprite> inventorySprites;

    public Button inventoryItemPrefab;


	// Use this for initialization
	void Start () {
        Instance = this;
	}

    public void AddItem(int itemID, int quantity)
    {
        if (inventoryTracker.ContainsKey(itemID))
        {
            inventoryTracker[itemID] += quantity;
            inventoryButtons[itemID].GetComponentInChildren<Text>().text = inventoryTracker[itemID].ToString();
        }

        else
        {
            inventoryTracker.Add(itemID, quantity);
            Button b = Instantiate(inventoryItemPrefab) as Button;
            b.transform.SetParent(inventoryWindow.transform, false);
            b.GetComponentInChildren<Text>().text = quantity.ToString();
            b.GetComponentInChildren<Image>().sprite = inventorySprites[itemID];
            inventoryButtons.Add(b);
        }
    }

    public void RemoveItem(int itemID, int quantity)
    {

    }

    public void UpdateInventoryQuantity(int itemIndex)
    {
        inventoryButtons[itemIndex].GetComponentInChildren<Text>().text = inventoryTracker[itemIndex].ToString();
    }
}
