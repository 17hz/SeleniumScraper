using System;
using System.Collections.Generic;
using System.Text;
using Common.Definitions.Classes;
using OpenQA.Selenium;

namespace Common.Definitions.Interfaces
{
    public interface IStrategy
    {
        IEnumerable<ProductSimple> Execute(IWebDriver driver);
    }
}