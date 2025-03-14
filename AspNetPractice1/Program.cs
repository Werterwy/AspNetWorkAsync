using AspNetPractice1;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

public class Practoce1
{

    private static readonly object lockObject = new object();
    private static bool isRunning = true;
    private static int orderCount = 0; 

    public static void Main()
    {
        Console.WriteLine("Добро пожаловать в ресторан!\n");

        Ovisiant ovisiant = new Ovisiant();
        Cook cook = new Cook();

        Thread waiterThread = new Thread(() => AddQueueOrder(ovisiant));
        waiterThread.Start();

        Thread cookThread = new Thread(() => PreparesDishes(cook, ovisiant));
        cookThread.Start();

        waiterThread.Join();
        cookThread.Join();
    }

    public static void AddQueueOrder(Ovisiant ovisiant)
    {
        while (isRunning)
        {
            Console.WriteLine("Какое блюдо хотели бы заказать?");
            Console.Write("Введите название блюда (0 для выхода): ");
            string nameOrder = Console.ReadLine();

            if (nameOrder == "0")
            {
                Console.WriteLine("Приходите еще.");
                isRunning = false;
                return;
            }

            lock (lockObject)
            {
                if (ovisiant.queueOrder.Count >= 5)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Очередь заказов переполнена, подождите, пока повар приготовит.");
                    Console.ResetColor();
                }
                else
                {
                    orderCount++;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[Заказ #{orderCount}] Официант: добавлен заказ на {nameOrder}.".PadRight(50) +
                                      $"| Текущее количество заказов: {ovisiant.queueOrder.Count + 1}");
                    Console.ResetColor();

                    ovisiant.queueOrder.Enqueue(nameOrder);
                }
            }

            Thread.Sleep(2000);
        }
    }

    public static void PreparesDishes(Cook cook, Ovisiant ovisiant)
    {
        while (isRunning || ovisiant.queueOrder.Count > 0)
        {
            string nameDishes = null;

            lock (lockObject)
            {
                if (ovisiant.queueOrder.Count > 0)
                {
                    nameDishes = ovisiant.queueOrder.Dequeue();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{"".PadRight(50)}| Повар: приступаю к приготовлению {nameDishes}. Осталось заказов: {ovisiant.queueOrder.Count}");
                    Console.ResetColor();
                }
            }

            if (nameDishes != null)
            {
                Thread.Sleep(3000);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"{"".PadRight(50)}| Повар: {nameDishes} приготовлено.");
                Console.ResetColor();
            }
            else
            {
                Thread.Sleep(500); 
            }
        }
    }


}

