using OpenQA.Selenium;
using System;

namespace Element34.ExtensionClasses
{
    public static class ExpectedConditionsExtensions
    {
        public static Func<IWebDriver, bool> EventFires(IWebElement element, string eventType)
        {
            return driver =>
            {
                try
                {
                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                    return (bool)js.ExecuteScript($"return checkCustomEvent('{eventType}')", element);
                }
                catch (Exception)
                {
                    return false;
                }
            };
        }

        public static Func<IWebDriver, bool> AttributeToBe(IWebElement element, string attributeName, string expectedValue)
        {
            return driver =>
            {
                try
                {
                    string actualValue = element.GetAttribute(attributeName);
                    return actualValue != null && actualValue.Equals(expectedValue, StringComparison.OrdinalIgnoreCase);
                }
                catch (Exception)
                {
                    // Handle Exception if needed
                    return false;
                }
            };
        }
    }
}
