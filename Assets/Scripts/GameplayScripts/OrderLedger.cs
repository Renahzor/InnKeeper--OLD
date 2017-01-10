using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Collection and sorting of orders for display to player
public class OrderLedger : MonoBehaviour
{

    public GameObject ledgerPanel;
    public GameObject ledgerElement;

    List<Order> orders = new List<Order>();
	
	void Start()
    {
        orders.Add(new Order());
        ledgerPanel.SetActive(false);

        //create the table for displaying open orders
        foreach (Order o in orders)
        {
            var le = Instantiate(ledgerElement);
            le.transform.SetParent(ledgerPanel.transform, false);

            le.GetComponentInChildren<Text>().text = o.orderDescription + "\n Qty: " + o.quantity + " Payment: " + o.paymentValue + " Gold";
            le.GetComponent<Button>().onClick.AddListener(() => ExecuteOrder(o));
        }
    }

    void ExecuteOrder(Order o)
    {
        if (InventoryMaster.Instance.inventoryTracker.ContainsKey(o.itemIndex) &&  InventoryMaster.Instance.inventoryTracker[o.itemIndex] >= o.quantity)
        {
            InventoryMaster.Instance.inventoryTracker[o.itemIndex] -= o.quantity;
            

            Player.Instance.playerGold += o.paymentValue;
            Player.Instance.UpdateGoldDisplay();

            InventoryMaster.Instance.UpdateInventoryQuantity(o.itemIndex);
        }

        else 
            Debug.Log("Not Enough Items");
    }

    public void AddNewOrder(Order o)
    {
        orders.Add(o);
        var le = Instantiate(ledgerElement);
        le.transform.SetParent(ledgerPanel.transform, false);

        le.GetComponentInChildren<Text>().text = o.orderDescription + "\n Qty: " + o.quantity + " Payment: " + o.paymentValue + " Gold";
        le.GetComponent<Button>().onClick.AddListener(() => ExecuteOrder(o));
    }
}
