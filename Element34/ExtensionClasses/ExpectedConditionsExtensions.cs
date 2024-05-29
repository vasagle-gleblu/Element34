using OpenQA.Selenium;
using System;

namespace Element34.ExtensionClasses
{
    /// <summary>
    /// Provides extension methods for expected conditions in Selenium WebDriver.
    /// </summary>
    public static class ExpectedConditionsExtensions
    {
        /// <summary>
        /// Returns a function that checks if a custom JavaScript event has been fired on the specified element.
        /// </summary>
        /// <param name="element">The web element to check the event on.</param>
        /// <param name="eventType">The type of the event to check.</param>
        /// <returns>A function that returns <c>true</c> if the event has been fired; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> or <paramref name="eventType"/> is null.</exception>
        public static Func<IWebDriver, bool> EventFires(IWebElement element, string eventType)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            if (eventType == null) throw new ArgumentNullException(nameof(eventType));

            return driver =>
            {
                try
                {
                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                    return (bool)js.ExecuteScript($"return (typeof checkCustomEvent === 'function') && checkCustomEvent('{eventType}')", element);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception in EventFires: {ex.Message}");
                    return false;
                }
            };
        }

        /// <summary>
        /// Returns a function that checks if the specified attribute of an element has the expected value.
        /// </summary>
        /// <param name="element">The web element to check the attribute on.</param>
        /// <param name="attributeName">The name of the attribute to check.</param>
        /// <param name="expectedValue">The expected value of the attribute.</param>
        /// <returns>A function that returns <c>true</c> if the attribute has the expected value; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/>, <paramref name="attributeName"/>, or <paramref name="expectedValue"/> is null.</exception>
        public static Func<IWebDriver, bool> AttributeToBe(IWebElement element, string attributeName, string expectedValue)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            if (attributeName == null) throw new ArgumentNullException(nameof(attributeName));
            if (expectedValue == null) throw new ArgumentNullException(nameof(expectedValue));

            return driver =>
            {
                try
                {
                    string actualValue = element.GetAttribute(attributeName);
                    return actualValue != null && actualValue.Equals(expectedValue, StringComparison.OrdinalIgnoreCase);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception in AttributeToBe: {ex.Message}");
                    return false;
                }
            };
        }
    }
}
