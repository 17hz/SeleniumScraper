using OpenQA.Selenium;
using System;
using System.Linq;
using System.Text.Json;
using Common.Definitions.Classes;
using OpenQA.Selenium.Chrome;
using Common.Definitions.Interfaces;
using Strategies.Restaurants;
using Core.ScrapingEngine;
using Common.Utilities.ExtensionMethods;

namespace Tests.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ScrapingEngine engine = new ScrapingEngine();

            // Initialize scraper strategy
            IStrategy pureStrategy = new StrategyPureCoUk("https://www.pure.co.uk/menus/breakfast");

            // Execute scraper strategy
            var result = engine.ExecuteStrategy(pureStrategy);

            foreach (ProductSimple product in result)
            {
                Console.WriteLine($"{product.DishName}");
            }

            Console.ReadLine();
        }
    }
}
