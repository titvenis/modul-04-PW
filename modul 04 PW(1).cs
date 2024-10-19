using System;
using System.Collections.Generic;

public interface IPayment
{
    void ProcessPayment(double amount);
}

public class CreditCardPayment : IPayment
{
    public void ProcessPayment(double amount)
    {
        Console.WriteLine($"Оплата кредитной картой: {amount} рублей.");
    }
}

public class PayPalPayment : IPayment
{
    public void ProcessPayment(double amount)
    {
        Console.WriteLine($"Оплата через PayPal: {amount} рублей.");
    }
}

public class BankTransferPayment : IPayment
{
    public void ProcessPayment(double amount)
    {
        Console.WriteLine($"Оплата банковским переводом: {amount} рублей.");
    }
}

public interface IDelivery
{
    void DeliverOrder(Order order);
}

public class CourierDelivery : IDelivery
{
    public void DeliverOrder(Order order)
    {
        Console.WriteLine("Доставка курьером.");
    }
}

public class PostDelivery : IDelivery
{
    public void DeliverOrder(Order order)
    {
        Console.WriteLine("Доставка почтой.");
    }
}

public class PickUpPointDelivery : IDelivery
{
    public void DeliverOrder(Order order)
    {
        Console.WriteLine("Самовывоз из пункта выдачи.");
    }
}

public interface INotification
{
    void SendNotification(string message);
}

public class EmailNotification : INotification
{
    public void SendNotification(string message)
    {
        Console.WriteLine($"Отправка email: {message}");
    }
}

public class SmsNotification : INotification
{
    public void SendNotification(string message)
    {
        Console.WriteLine($"Отправка SMS: {message}");
    }
}

public interface IDiscount
{
    double CalculateDiscount(Order order);
}

public class SumDiscount : IDiscount
{
    private readonly double _discountThreshold;
    private readonly double _discountPercentage;

    public SumDiscount(double discountThreshold, double discountPercentage)
    {
        _discountThreshold = discountThreshold;
        _discountPercentage = discountPercentage;
    }

    public double CalculateDiscount(Order order)
    {
        double totalAmount = order.GetTotalAmount();
        if (totalAmount > _discountThreshold)
        {
            return totalAmount * _discountPercentage / 100;
        }
        return 0;
    }
}

public class DiscountCalculator
{
    private readonly List<IDiscount> _discounts = new List<IDiscount>();

    public void AddDiscount(IDiscount discount)
    {
        _discounts.Add(discount);
    }

    public double CalculateTotal(Order order)
    {
        double totalDiscount = 0;
        foreach (var discount in _discounts)
        {
            totalDiscount += discount.CalculateDiscount(order);
        }
        return order.GetTotalAmount() - totalDiscount;
    }
}

public class Order
{
    public List<string> Items { get; private set; } = new List<string>();
    public double Price { get; private set; }
    public IPayment PaymentMethod { get; set; }
    public IDelivery DeliveryMethod { get; set; }
    public INotification NotificationMethod { get; set; }

    public void AddItem(string item, double price)
    {
        Items.Add(item);
        Price += price;
    }

    public double GetTotalAmount()
    {
        return Price;
    }

    public void ProcessOrder()
    {
        Console.WriteLine("Обработка заказа...");
        PaymentMethod.ProcessPayment(Price);
        DeliveryMethod.DeliverOrder(this);
        NotificationMethod.SendNotification("Ваш заказ был успешно оформлен!");
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Создаем заказ
        Order order = new Order();
        order.AddItem("Товар 1", 500);
        order.AddItem("Товар 2", 1500);

        // Устанавливаем способ оплаты, доставки и уведомлений
        order.PaymentMethod = new CreditCardPayment();
        order.DeliveryMethod = new CourierDelivery();
        order.NotificationMethod = new EmailNotification();

        // Создаем и добавляем скидки
        DiscountCalculator discountCalculator = new DiscountCalculator();
        discountCalculator.AddDiscount(new SumDiscount(1000, 10)); // Скидка 10% на заказы выше 1000

        double finalPrice = discountCalculator.CalculateTotal(order);
        Console.WriteLine($"Итоговая сумма с учетом скидки: {finalPrice} рублей.");

        // Обрабатываем заказ
        order.ProcessOrder();
    }
}
