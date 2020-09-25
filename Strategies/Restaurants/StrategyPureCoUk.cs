using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Common.Definitions.Classes;
using Common.Definitions.Interfaces;
using Common.Utilities.ExtensionMethods;
using OpenQA.Selenium;

namespace Strategies.Restaurants
{
    public class StrategyPureCoUk : IStrategy
    {
        private readonly string _url;
        private IWebDriver _webDriver;

        public StrategyPureCoUk(string url)
        {
            _url = url;
        }

        public IEnumerable<ProductSimple> Execute(IWebDriver driver)
        {
            _webDriver = driver;

            List<ProductSimple> products = new List<ProductSimple>();

            try
            {
                driver.Navigate().GoToUrl(_url);

                // Get all menu titles. We will iterate each of them
                List<MenuCategory> menuCategories = ScrapeMenuTitles();

                foreach (MenuCategory category in menuCategories)
                {
                    // During the first iteration we are expected to be on the correct page
                    if (_webDriver.Url != category.Url)
                        _webDriver.Navigate().GoToUrl(category.Url);

                    // Get the menu description
                    category.MenuDescription = ScrapeMenuDescription();

                    List<string> menuSectionTitles = ScrapeMenuSectionTitles();
                    var menuSectionContainers = _webDriver.FindElements(By.XPath("//div[@class='collapse in']"));

                    for (int i = 0; i < menuSectionTitles.Count; i++)
                    {
                        // Get all products for this particular section
                        var sectionProducts = menuSectionContainers[i].FindElements(By.XPath(".//div/div/a"));

                        foreach (IWebElement sectionProduct in sectionProducts)
                        {
                            // Start scraping product information.
                            // Note that product description will need to be scraped afterwards,
                            // that's why we store the URL to product page
                            ProductSimple product = new ProductSimple
                            {
                                MenuTitle = category.MenuTitle,
                                MenuDescription = category.MenuDescription,
                                MenuSectionTitle = menuSectionTitles[i],
                                DishName = sectionProduct.GetAttribute("title").HtmlDecode(),
                                Url = sectionProduct.GetAttribute("href")
                            };

                            Debug.WriteLine($"Adding product: {product.DishName}");
                            products.Add(product);
                        }
                    }
                }

                // Get product descriptions for each product
                foreach (ProductSimple product in products)
                {
                    _webDriver.Navigate().GoToUrl(product.Url);

                    IWebElement paragraph = null;
                    // Some pages appear to be constructed differently. Let's handle that.
                    try
                    {
                        paragraph = _webDriver.FindElement(By.XPath("//article[@class='menu-item-details']/div[1]/p[1]"));
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            paragraph = _webDriver.FindElement(By.XPath("//article[@class='menu-item-details']/div[1]/div[1]/p[1]"));
                        }
                        catch (Exception exception)
                        {
                            try
                            {
                                paragraph = _webDriver.FindElement(By.XPath("//div[@class='cardBack']"));
                            }
                            catch (Exception e1)
                            {
                                Debug.WriteLine($"It's a sad, sad situation...");
                                throw;
                            }
                        }
                    }

                    Debug.WriteLine($"Adding product description for product: {product.DishName}");
                    product.DishDescription = paragraph.Text.HtmlDecode();
                }

                // Dispose of the driver
                _webDriver.Quit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return products;
        }

        private List<MenuCategory> ScrapeMenuTitles()
        {
            // Find all menu anchor elements
            var menuAnchors = _webDriver.FindElements(By.XPath("//ul[@class='submenu']/li/a"));

            // Remove unwanted elements
            var applicableAnchors = menuAnchors.Where(x => x.Text.ToLower() != "wellbeing boxes").ToList();

            // Parse anchors
            return applicableAnchors.Select(x => new MenuCategory { MenuTitle = x.Text.HtmlDecode().TrimSpecialChars().ToTitleCase(), Url = x.GetAttribute("href") }).ToList();
        }

        private string ScrapeMenuDescription()
        {
            // Find menu description paragraph element
            var menuParagraph = _webDriver.FindElement(By.XPath("//header[@class='menu-header']/p"));

            return $"{menuParagraph.Text.HtmlDecode().TrimSpecialChars().Split('\r').First()}..";
        }

        private List<string> ScrapeMenuSectionTitles()
        {
            // Find menu section span elements
            var spanElements = _webDriver.FindElements(By.XPath("//h4[@class='menu-title']/a/span"));

            // Parse to a clean list of strings and return
            return spanElements.Select(x => x.Text.HtmlDecode().TrimSpecialChars().ToTitleCase()).ToList();
        }

        private class MenuCategory
        {
            public string MenuTitle;
            public string MenuDescription;
            public string Url;
        }
    }
}