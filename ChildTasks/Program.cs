using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    static Random random = new Random();
    static void Main()
    {
        var orders = GenerateOrders(5);
        ProcessOrders(orders);
    }

    static List<Order> GenerateOrders(int count)
    {
        return Enumerable.Range(1, count).Select(id => new Order
        {
            Id = id,
            Items = Enumerable.Range(1, random.Next(1, 6)).Select(_ => random.Next(100, 1000)).ToList(),
            TotalAmount = 0
        }).ToList();
    }

    static void ProcessOrders(List<Order> orders)
    {
        var tasks = orders.Select(order => Task.Run(async () => await ProcessOrder(order))).ToArray();
        Task.WaitAll(tasks);
    }

    static async Task ProcessOrder(Order order)
    {
        Console.WriteLine($"Обработка заказа {order.Id}");
        var parentTask = Task.Run(() =>
        {
            var stockTask = Task.Run(() => CheckStock(order));
            var paymentTask = stockTask.ContinueWith(t => ProcessPayment(order), TaskContinuationOptions.OnlyOnRanToCompletion);
            var packagingTask = paymentTask.ContinueWith(t => PackageOrder(order), TaskContinuationOptions.OnlyOnRanToCompletion);
            Task.WaitAll(stockTask, paymentTask, packagingTask);
        });
        try { await parentTask; }
        catch (Exception ex) { Console.WriteLine($"Ошибка в заказе {order.Id}: {ex.Message}"); }
    }

    static void CheckStock(Order order)
    {
        if (random.NextDouble() < 0.2) throw new Exception("Товар отсутствует");
        Console.WriteLine($"Заказ {order.Id}: Товар на складе");
    }

    static void ProcessPayment(Order order)
    {
        if (random.NextDouble() < 0.2) throw new Exception("Ошибка платежа");
        Console.WriteLine($"Заказ {order.Id}: Оплата успешна");
    }

    static void PackageOrder(Order order)
    {
        if (random.NextDouble() < 0.1) throw new Exception("Ошибка упаковки");
        Console.WriteLine($"Заказ {order.Id}: Упакован");
    }
}

class Order
{
    public int Id { get; set; }
    public List<int> Items { get; set; }
    public int TotalAmount { get; set; }
}
