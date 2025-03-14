using System.Text;
using TPLGenerationPayments;

public class Practice2
{

    private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(3);
    private static readonly List<Payment> successfulPayments = new List<Payment>();
    private static readonly List<Payment> failedPayments = new List<Payment>();
    private static readonly string logFilePath = "payments_log.txt";

    public static async Task Main(string[] args)
    {
        Console.WriteLine("Генерация платежей");
        List<Task> tasks = new List<Task>();
        CancellationTokenSource cts = new CancellationTokenSource();
        Random random = new Random();

        for (int i = 1; i <= 15; i++)
        {
            var payment = new Payment
            {
                PaymentID = i,
                SumPayment = random.Next(100, 5000),
                TimeProcessing = random.Next(1, 5)
            };

            tasks.Add(ProcessPaymentAsync(payment, cts.Token));
        }

        Task cancelTask = Task.Run(() =>
        {
            while (true)
            {
                if (Console.ReadLine()?.Trim().ToLower() == "cancel")
                {
                    cts.Cancel();
                    break;
                }
            }
        });

        await Task.WhenAll(tasks);
        cts.Dispose();

        LogResults();
    }
    
    public static async Task ProcessPaymentAsync(Payment payment, CancellationToken token)
    {
        await semaphore.WaitAsync();
        try
        {
            if (token.IsCancellationRequested)
            {
                Console.WriteLine($"Платеж {payment.PaymentID} отменен до начала обработки.");
                return;
            }

            Console.WriteLine($"Обрабатывается платеж {payment.PaymentID}.");
            await Task.Delay(payment.TimeProcessing * 1000, token);

            if (new Random().Next(1, 101) <= 20) // 15% вероятность сбоя
            {
                throw new Exception("Ошибка при обработке платежа.");
            }

            lock (successfulPayments)
            {
                successfulPayments.Add(payment);
            }
            Console.WriteLine($"Платеж {payment.PaymentID} успешно обработан.");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine($"Платеж {payment.PaymentID} отменен.");
        }
        catch (Exception ex)
        {
            lock (failedPayments)
            {
                failedPayments.Add(payment);
            }
            Console.WriteLine($"Ошибка при обработке платежа {payment.PaymentID}: {ex.Message}");
        }
        finally
        {
            semaphore.Release();
        }
    }

    private static void LogResults()
    {
        using (StreamWriter writer = new StreamWriter(logFilePath, false, Encoding.UTF8))
        {
            writer.WriteLine("Успешные платежи:");
            foreach (var payment in successfulPayments)
            {
                writer.WriteLine($"ID: {payment.PaymentID}, Сумма: {payment.SumPayment}, Время обработки: {payment.TimeProcessing} сек.");
            }
            writer.WriteLine();

            writer.WriteLine("Неуспешные платежи:");
            foreach (var payment in failedPayments)
            {
                writer.WriteLine($"ID: {payment.PaymentID}, Сумма: {payment.SumPayment}, Время обработки: {payment.TimeProcessing} сек.");
            }
            writer.WriteLine();
        }
        Console.WriteLine("Результаты записаны в файл payments_log.txt.");
    }
}