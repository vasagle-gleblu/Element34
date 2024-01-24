using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using System;

namespace Element34
{
    public enum BrowserType
    {
        Chrome,
        Chromium,
        Firefox,
        IE,
        Edge
    }

    public class WebDriverFactory
    {
        public static IWebDriver CreateWebDriver(BrowserType browserType, DriverOptions options)
        {
            switch (browserType)
            {
                case BrowserType.Chrome:
                    return CreateChromeDriver(options as ChromeOptions);
                case BrowserType.Firefox:
                    return CreateFirefoxDriver(options as FirefoxOptions);
                case BrowserType.IE:
                    return CreateInternetExplorerDriver(options as InternetExplorerOptions);
                case BrowserType.Edge:
                    return CreateEdgeDriver(options as EdgeOptions);
                default:
                    throw new ArgumentException($"Unsupported browser type: {browserType}", nameof(browserType));
            }
        }

        public static IWebDriver CreateChromeDriver(ChromeOptions options = null)
        {
            if (options == null)
                options = new ChromeOptions();

            return new ChromeDriver(options);
        }

        public static IWebDriver CreateFirefoxDriver(FirefoxOptions options = null)
        {
            if (options == null)
                options = new FirefoxOptions();

            return new FirefoxDriver(options);
        }

        public static IWebDriver CreateInternetExplorerDriver(InternetExplorerOptions options = null)
        {
            if (options == null)
                options = new InternetExplorerOptions();

            return new InternetExplorerDriver(options);
        }

        public static IWebDriver CreateEdgeDriver(EdgeOptions options = null)
        {
            if (options == null)
                options = new EdgeOptions();

            return new EdgeDriver(options);
        }
    }
}
