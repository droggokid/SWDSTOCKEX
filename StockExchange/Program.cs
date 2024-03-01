using System;
using System.Collections.Generic;
using System.Linq;

namespace StockExchange
{
    public interface IObserver
    {
        void Update();
    }

    public class Stocks : IObserver
    {
        public string Name { get; }
        public int Amount { get; }
        private float price;
        private readonly List<IObserver> observers = new();

        public Stocks(string name, int amount, float price)
        {
            Name = name;
            Amount = amount;
            this.price = price;
        }

        public float Price
        {
            get { return price; }
            set
            {
                if (price != value)
                {
                    price = value;
                    Notify();
                }
            }
        }

        public void Attach(IObserver observer)
        {
            observers.Add(observer);
        }

        public void Notify()
        {
            foreach (IObserver observer in observers)
            {
                observer.Update();
            }
        }

        public void Update()
        {
            Console.WriteLine($"Stock {Name} updated. New price: {price}");
        }
    }

    public class Portfolio : IObserver
    {
        public List<Stocks> stocks { get; } = new();
        private readonly List<IObserver> observers = new();

        public void AddStock(Stocks stock)
        {
            stocks.Add(stock);
            stock.Attach(this);
        }

        public void Attach(IObserver observer)
        {
            observers.Add(observer);
        }

        public void Update()
        {
            Console.WriteLine("Portfolio updated");
        }
    }

    public class PortfolioDisplay : IObserver
    {
        private readonly Portfolio portfolio;

        public PortfolioDisplay(Portfolio portfolio)
        {
            this.portfolio = portfolio;
            portfolio.Attach(this);  
        }

        public void Update()
        {
            Console.WriteLine("Portfolio display updated");
            foreach (Stocks stock in portfolio.stocks)
            {
                Console.WriteLine($"Stock: {stock.Name}, Price: {stock.Price}");
            }
        }
    }

    class Program
    {
        static void Main()
        {
            Portfolio portfolio = new();
            PortfolioDisplay display = new PortfolioDisplay(portfolio); 

            Stocks xqcStock = new Stocks("XQC", 0, 0);
            portfolio.AddStock(xqcStock);

            Stocks abcStock = new Stocks("ABC", 0, 0);
            portfolio.AddStock(abcStock);

            while (true)
            {
                Console.WriteLine("Enter a command:");
                string? command = Console.ReadLine();
                string[] parts = command?.Split(' ') ?? Array.Empty<string>();

                if (parts.Length == 2 && float.TryParse(parts[1], out float price))
                {
                    var stock = portfolio.stocks.FirstOrDefault(s => s.Name == parts[0]);
                    if (stock != null)
                    {
                        stock.Price = price;
                        display.Update();
                    }
                }
            }
        }
    }
}