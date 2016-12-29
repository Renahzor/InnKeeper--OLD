using UnityEngine;
using System.Collections;

public class Order {

    public string orderItemName, orderDescription;
    public int orderLevel, quantity, paymentValue, itemIndex;

    public Order()
    {
        orderItemName = "Rat Tails";
        orderDescription = "We need some rat tails for soup!";
        orderLevel = 1;
        quantity = 10;
        paymentValue = 25;
        itemIndex = 0; //item required for this order
    }

}
