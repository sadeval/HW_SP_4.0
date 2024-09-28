using System;
using System.Threading;
using System.Threading.Tasks;

class Bank
{
    private decimal _balance;
    private readonly object _balanceLock = new object();

    public Bank(decimal initialBalance)
    {
        _balance = initialBalance;
    }

    public void Withdraw(decimal amount, string atmName)
    {
        Monitor.Enter(_balanceLock);
        try
        {
            Console.WriteLine($"{atmName} пытается снять {amount}.");

            if (_balance >= amount)
            {
                Console.WriteLine($"{atmName} подтверждено снятие {amount}. Текущий баланс: {_balance}");
                _balance -= amount;
                Console.WriteLine($"{atmName} завершил снятие. Новый баланс: {_balance}");
            }
            else
            {
                Console.WriteLine($"{atmName} недостаточно средств для снятия {amount}. Баланс: {_balance}");
            }
        }
        finally
        {
            Monitor.Exit(_balanceLock);
        }
    }
}

class Program
{
    static void Main()
    {
        Bank bank = new Bank(1000);
        Task[] tasks = new Task[5];

        for (int i = 0; i < tasks.Length; i++)
        {
            string atmName = $"Клиент-{i + 1}";
            tasks[i] = Task.Run(() =>
            {
                Random rnd = new Random();
                for (int j = 0; j < 3; j++)
                {
                    decimal amount = rnd.Next(100, 500);
                    bank.Withdraw(amount, atmName);
                    Thread.Sleep(rnd.Next(1000, 2000));
                }
            });
        }

        Task.WaitAll(tasks);
        Console.WriteLine("Все транзакции завершены.");
    }
}
