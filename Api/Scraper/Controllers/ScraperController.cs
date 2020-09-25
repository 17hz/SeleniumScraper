using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Common.Definitions.Classes;
using Common.Definitions.Interfaces;
using Core.ScrapingEngine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Strategies.Restaurants;

namespace Api.Scraper.Controllers
{
    [ApiController]
    [Route("")]
    public class ScraperController : ControllerBase
    {
        private readonly ILogger<ScraperController> _logger;
        private readonly ScrapingEngine _engine;

        public ScraperController(ILogger<ScraperController> logger)
        {
            _logger = logger;
            _engine = new ScrapingEngine();
        }

        /// <summary>
        /// Will scrape the menu of the PURE restaurant
        /// </summary>
        /// <param name="menuUrl">URL of the menu to scrape</param>
        /// <returns>Entire menu for the PURE restaurant in JSON format</returns>
        [HttpPost]
        [Route("scrape")]
        public IActionResult Scrape([FromBody] string menuUrl = "https://www.pure.co.uk/menus/breakfast")
        {
            try
            {
                // Validate input URL
                if (string.IsNullOrWhiteSpace(menuUrl) || !menuUrl.Contains("pure.co.uk/menus"))
                    _logger.LogError($"{nameof(menuUrl)} value is invalid.");

                // Initialize scraper strategy
                IStrategy pureStrategy = new StrategyPureCoUk(menuUrl);

                // Execute scraper strategy
                List<ProductSimple> products = _engine.ExecuteStrategy(pureStrategy).ToList();

                // Serialize to JSON and return
                JsonSerializerOptions jso = new JsonSerializerOptions();
                jso.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
                jso.WriteIndented = true;

                var jsonString = JsonSerializer.Serialize(products, jso);
                return Ok(jsonString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpGet]
        [Route("scrape")]
        public IActionResult ScrapeTest()
        {
            return Scrape();
        }
    }
}
