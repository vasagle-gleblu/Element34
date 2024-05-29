using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using System;

namespace Element34
{
    /// <summary>
    /// Represents different types of browsers that can be controlled using WebDriver.
    /// </summary>
    public enum BrowserType
    {
        /// <summary>
        /// Google Chrome browser.
        /// </summary>
        Chrome,

        /// <summary>
        /// Chromium is an open-source browser project that forms the basis for the Chrome web browser.
        /// </summary>
        Chromium,

        /// <summary>
        /// Mozilla Firefox browser.
        /// </summary>
        Firefox,

        /// <summary>
        /// Internet Explorer browser. Note: Internet Explorer is largely deprecated and support may be limited.
        /// </summary>
        IE,

        /// <summary>
        /// Microsoft Edge browser.
        /// </summary>
        Edge
    }

/// <summary>
/// Factory for creating WebDriver instances based on specified browser types.
/// </summary>
public class WebDriverFactory
    {
        /// <summary>
        /// Creates a WebDriver instance for the specified browser type with optional settings and command-line arguments.
        /// </summary>
        /// <param name="browser">The type of browser for which the WebDriver should be created.</param>
        /// <param name="options">Optional pre-configured driver options. If null, defaults will be created based on the browser type.</param>
        /// <param name="browserArguments">Optional array of command-line arguments for the browser. These arguments are added to the provided or default options, except for Internet Explorer.</param>
        /// <returns>A new instance of IWebDriver configured for the specified browser with the given options and arguments.</returns>
        /// <exception cref="ArgumentException">Thrown when specific options require paired values and are not supplied correctly, particularly for Internet Explorer.</exception>
        /// <exception cref="NotImplementedException">Thrown when the specified browser type is not supported.</exception>
        public static IWebDriver CreateDriver(BrowserType browser, DriverOptions options = null, string[] browserArguments = null)
        {
            if (options == null)
            {
                switch (browser)
                {
                    case BrowserType.Chrome:
                        options = new ChromeOptions();
                        break;
                    case BrowserType.Firefox:
                        options = new FirefoxOptions();
                        break;
                    case BrowserType.Edge:
                        options = new EdgeOptions();
                        break;
                    case BrowserType.IE:
                        options = new InternetExplorerOptions();
                        break;
                    default:
                        throw new NotImplementedException($"Driver {browser} is not supported.");
                }
            }

            // Adding browser-specific arguments or capabilities
            if (browserArguments != null)
            {
                switch (browser)
                {
                    case BrowserType.Chrome:
                        ((ChromeOptions)options).AddArguments(browserArguments);
                        break;
                    case BrowserType.Firefox:
                        ((FirefoxOptions)options).AddArguments(browserArguments);
                        break;
                    case BrowserType.Edge:
                        ((EdgeOptions)options).AddArguments(browserArguments);
                        break;
                    case BrowserType.IE:
                        if (browserArguments.Length % 2 != 0)
                            throw new ArgumentException("Internet Explorer options must be in pairs of command and value.", nameof(browserArguments));
                        var ieOptions = (InternetExplorerOptions)options;
                        for (int i = 0; i < browserArguments.Length; i += 2)
                            ieOptions.AddAdditionalOption(browserArguments[i], browserArguments[i + 1]);
                        break;
                }
            }

            // Use the properly configured options to create the WebDriver.
            return CreateWebDriverWithOptions(browser, options);
        }

        private static IWebDriver CreateWebDriverWithOptions(BrowserType browser, DriverOptions options)
        {
            switch (browser)
            {
                case BrowserType.Chrome:
                    return new ChromeDriver((ChromeOptions)options);
                case BrowserType.Firefox:
                    return new FirefoxDriver((FirefoxOptions)options);
                case BrowserType.Edge:
                    return new EdgeDriver((EdgeOptions)options);
                case BrowserType.IE:
                    return new InternetExplorerDriver((InternetExplorerOptions)options);
                default:
                    throw new NotImplementedException($"Driver {browser} is not supported.");
            }
        }
    }

}
