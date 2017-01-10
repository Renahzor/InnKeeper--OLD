public class Order {

    public string orderItemName, orderDescription;
    public int quantity, paymentValue, itemIndex;

    public Order()
    {
        orderItemName = "Rat Tails";
        orderDescription = "We need some rat tails for soup!";
        quantity = 10;
        paymentValue = 15;
        itemIndex = 0; //item required for this order
    }

    public Order(string name, string description, int quantityRequired, int payment, int itemIndexOfObjective)
    {
        orderItemName = name;
        orderDescription = description;
        quantity =quantityRequired;
        paymentValue = payment;
        itemIndex = itemIndexOfObjective; //item required for this order
    }
}
