using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using Common.Definitions.Classes;
using OpenQA.Selenium.Chrome;
using Common.Definitions.Interfaces;

namespace Core.ScrapingEngine
{
    public class ScrapingEngine
    {
        private readonly IWebDriver _driver;

        public ScrapingEngine()
        {
            _driver = new ChromeDriver();
        }

        public IEnumerable<ProductSimple> ExecuteStrategy(IStrategy strategy)
        {
            IEnumerable<ProductSimple> products;

            try
            {
                products = strategy.Execute(_driver);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return products;
        }
    }
}