using Microsoft.VisualBasic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Chromium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Keys = OpenQA.Selenium.Keys;


namespace Element34.ExtensionClasses
{
    public static class WebDriverExtras
    {
        #region Fields
        private const int _defaultTimeSpan = 15;
        private const int timeDelay = 1500;
        #endregion

        private static readonly HashSet<string> PositiveResponses = new HashSet<string> { "y", "yes", "t", "true", "1", "+", "affirmative", "positive" };
        private static readonly HashSet<string> NegativeResponses = new HashSet<string> { "n", "no", "f", "false", "0", "-", "negative" };

        public static TabManager<TWebDriver> CreateTabManager<TWebDriver>(this TWebDriver driver) where TWebDriver : IWebDriver
        {
            return new TabManager<TWebDriver>(driver);
        }

        #region [Public Functions]
        public static void WaitUntilVisible(this IWebDriver driver, By locator, TimeSpan? timeOut = null)
        {  // Wait until element specified by locator is visible
            timeOut = (timeOut == null) ? TimeSpan.FromSeconds(_defaultTimeSpan) : timeOut.Value;     // default value for TimeSpan parameter
            WebDriverWait wait = new WebDriverWait(driver, (TimeSpan)timeOut);
            wait.Until(drv => drv.FindElement(locator).Displayed);
        }

        public static void WaitUntilInvisible(this IWebDriver driver, By locator, TimeSpan? timeOut = null)
        {  // Wait until element specified by locator is not visible
            timeOut = (timeOut == null) ? TimeSpan.FromSeconds(_defaultTimeSpan) : timeOut.Value;     // default value for TimeSpan parameter
            WebDriverWait wait = new WebDriverWait(driver, (TimeSpan)timeOut);
            wait.Until(drv => !drv.FindElement(locator).Displayed);
        }

        public static void WaitUntilEnabled(this IWebDriver driver, By locator, TimeSpan? timeOut = null)
        {  // Wait until element specified by locator is enabled
            timeOut = (timeOut == null) ? TimeSpan.FromSeconds(_defaultTimeSpan) : timeOut.Value;     // default value for TimeSpan parameter
            WebDriverWait wait = new WebDriverWait(driver, (TimeSpan)timeOut);
            wait.Until(drv => drv.FindElement(locator).Enabled);
        }

        public static void WaitUntilDisabled(this IWebDriver driver, By locator, TimeSpan? timeOut = null)
        {// Wait until element specified by locator is not enabled
            timeOut = (timeOut == null) ? TimeSpan.FromSeconds(_defaultTimeSpan) : timeOut.Value;     // default value for TimeSpan parameter
            var wait = new WebDriverWait(driver, (TimeSpan)timeOut);
            wait.Until(drv => !drv.FindElement(locator).Enabled);
        }

        public static void ExecuteJavaScript(this IWebDriver driver, string script, params object[] args)
        {  // Execute client-side JavaScript
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript(script, args);
        }

        public static T ExecuteJavaScript<T>(this IWebDriver driver, string script, params object[] args)
        {  // Execute client-side JavaScript with return type
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            return (T)js.ExecuteScript(script, args);
        }

        public static void ExecuteJavaScript(this IWebElement element, string script, params object[] args)
        {   // Execute client-side JavaScript on IWebElement
            IWebDriver driver = ((IWrapsDriver)element).WrappedDriver;
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript(script, args);
        }

        public static T ExecuteJavaScript<T>(this IWebElement element, string script, params object[] args)
        {  // Execute client-side JavaScript on IWebElement with return type
            IWebDriver driver = ((IWrapsDriver)element).WrappedDriver;
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            return (T)js.ExecuteScript(script, args);
        }

        public static void ExecuteJavaScript(this SelectElement selectElement, string script, params object[] args)
        {  // Execute client-side JavaScript on Select Element
            IWebDriver driver = ((IWrapsDriver)selectElement).WrappedDriver;
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript(script, args);
        }

        public static T ExecuteJavaScript<T>(this SelectElement selectElement, string script, params object[] args)
        {  // Execute client-side JavaScript on Select Element with return type
            IWebDriver driver = ((IWrapsDriver)selectElement).WrappedDriver;
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            return (T)js.ExecuteScript(script, args);
        }

        /// <summary>
        /// Gets the 'innerText' attribute of an element, which represents the rendered text content of the node.
        /// </summary>
        /// <param name="element">The web element from which to retrieve the inner text.</param>
        /// <returns>The inner text of the element.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the element is null.</exception>
        public static string InnerText(this IWebElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element), "Element cannot be null.");

            return element.GetAttribute("innerText");
        }

        /// <summary>
        /// Gets the 'value' attribute of an element, typically used for input fields.
        /// </summary>
        /// <param name="element">The web element from which to retrieve the value attribute.</param>
        /// <returns>The value attribute of the element.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the element is null.</exception>
        public static string Value(this IWebElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element), "Element cannot be null.");

            return element.GetAttribute("value");
        }

        public static Dictionary<string, object> GetAttributes(this IWebElement element)
        {
            const string script = "var items = {};" +
                                  "for (index = 0; index < arguments[0].attributes.length; ++index) " +
                                  "{" +
                                  "     items[arguments[0].attributes[index].name] = arguments[0].attributes[index].value" +
                                  "}" +
                                  "return items;";

            return element.ExecuteJavaScript<Dictionary<string, object>>(script, element);
        }

        public static void ScrollToPageBottom(this IWebDriver driver)
        {  // Scroll to top of page (JavaScript call)
            ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight)");
        }

        public static void ScrollToPageTop(this IWebDriver driver)
        {  // Scroll to bottom of page (JavaScript call)
            ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, 0)");
        }

        public static void ScrollToElement(this IWebDriver driver, By locator)
        {  // Scroll to element specified by locator (JavaScript call)
            if (locator == null)
                throw new ArgumentNullException(nameof(locator));
            else
            {
                IWebElement element = driver.FindElement(locator);
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView();", element);
            }
        }

        public static void ScrollToElement(this IWebDriver driver, IWebElement element)
        {  // Scroll to element (JavaScript call)
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            else
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView();", element);
        }

        public static void SetFocus(this IWebDriver driver, IWebElement element)
        {  // Set focus on element (JavaScript call)
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            else
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].focus();", element);
        }

        public static void SetFocus(this IWebElement element)
        {  // Set focus on element (JavaScript call)
            IWebDriver driver = ((IWrapsDriver)element).WrappedDriver;

            if (element == null)
                throw new ArgumentNullException(nameof(element));
            else
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].focus();", element);
        }

        /// <summary>
        /// Finds an element and types the specified text into it.
        /// </summary>
        /// <param name="driver">The WebDriver instance.</param>
        /// <param name="locator">The locator of the element to type into.</param>
        /// <param name="input">The text to be typed.</param>
        public static void Type(this IWebDriver driver, By locator, string input)
        {
            IWebElement webElement = driver.FindElement(locator) ?? throw new InvalidOperationException("Element not found with the specified locator.");
            webElement.Type(input);
        }

        /// <summary>
        /// Types the specified text into the provided web element, ensuring the element is suitable for text input.
        /// </summary>
        /// <param name="webElement">The web element.</param>
        /// <param name="input">Text to be typed.</param>
        /// <exception cref="ArgumentNullException">Thrown if the web element is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the element is not a valid input or textarea, or if the input type is unsupported.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the element is not visible and enabled.</exception>
        public static void Type(this IWebElement webElement, string input)
        {
            if (webElement == null)
                throw new ArgumentNullException(nameof(webElement), "Web element cannot be null.");

            if (!webElement.Enabled || !webElement.Displayed)
                throw new InvalidOperationException("Element must be visible and enabled to type.");

            string tagName = webElement.TagName.ToLower();
            if (!new[] { "input", "textarea" }.Contains(tagName))
                throw new ArgumentException("Element must be an input or textarea.", nameof(tagName));

            if (tagName == "input")
            {
                string type = webElement.GetAttribute("type")?.ToLower();
                if (type != null && !new[] { "text", "password", "email", "date", "datetime-local", "month", "number", "search", "tel", "time", "url", "week" }.Contains(type))
                    throw new ArgumentException($"Unsupported input type: {type}.", nameof(type));
            }

            webElement.Click(); // Ensure the element is focused
            webElement.Clear(); // Clear any pre-existing text
            webElement.SendKeys(input); // Input the new text
        }

        /// <summary>
        /// Tests whether an input element enforces a maximum length.
        /// </summary>
        /// <param name="webElement">The input web element to test.</param>
        /// <param name="limit">The expected maximum length of the input field.</param>
        /// <returns>True if the input value's length after insertion is equal to the specified limit, false otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the provided web element is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the web element is not a valid input or if its type is not supported.</exception>
        public static bool testMaxLength(IWebElement webElement, int limit)
        {
            if (webElement == null)
                throw new ArgumentNullException(nameof(webElement));

            if (webElement.TagName.ToLower() != "input")
                throw new ArgumentException("The provided element is not an input element.", nameof(webElement));

            string type = webElement.GetAttribute("type").ToLower();
            var allowedTypes = new[] { "text", "password", "email", "number", "search", "url" };
            if (!allowedTypes.Contains(type))
                throw new ArgumentException($"Unsupported input type '{type}'. Supported types are: {string.Join(", ", allowedTypes)}", nameof(type));

            // Clear the input field before testing
            webElement.Click();
            webElement.Clear();

            // Type characters into the field
            string longInput = new string('a', limit + 20);
            webElement.SendKeys(longInput);

            // Retrieve the value entered in the field
            string typedValue = webElement.GetAttribute("value");
            int typedLength = typedValue.Length;

            // Check if the length of the input is capped at the limit
            return typedLength == limit;
        }

        /// <summary>
        /// Gets a By locator for the CSS selector of the given element.
        /// </summary>
        /// <param name="element">The web element.</param>
        /// <returns>A By locator constructed from the CSS selector of the element.</returns>
        public static By GetCssSelectLocator(this IWebElement element)
        {
            return By.CssSelector(GetCssSelectText(element));
        }

        /// <summary>
        /// Generates a full CSS selector path for a given element using JavaScript.
        /// </summary>
        /// <param name="element">The web element to generate a selector for.</param>
        /// <returns>The CSS selector string for the specified element.</returns>
        public static string GetCssSelectText(this IWebElement element)
        {
            IWebDriver driver = ((IWrapsDriver)element).WrappedDriver;
            IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;

            string script = @"
        var el = arguments[0];
        if (!(el instanceof Element)) return '';
        var path = [];
        while (el.nodeType === Node.ELEMENT_NODE) {
            var selector = el.nodeName.toLowerCase();
            if (el.id) {
                selector += '#' + el.id;
                path.unshift(selector);
                break;
            } else {
                var sibling = el;
                var nth = 1;
                while (sibling = sibling.previousElementSibling) {
                    if (sibling.nodeName.toLowerCase() == selector) nth++;
                }
                if (nth != 1) {
                    selector += ':nth-of-type(' + nth + ')';
                }
            }
            path.unshift(selector);
            el = el.parentNode;
        }
        return path.join(' > ');";

            try
            {
                string cssSelector = (string)executor.ExecuteScript(script, element);
                return cssSelector;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to retrieve CSS selector for the element.", ex);
            }
        }

        /// <summary>
        /// Retrieves an XPath locator for the specified web element.
        /// </summary>
        /// <param name="element">The web element to generate XPath for.</param>
        /// <returns>An XPath By locator.</returns>
        public static By GetXPathLocator(this IWebElement element)
        {
            return By.XPath(element.GetXPathText());
        }

        /// <summary>
        /// Generates a full XPath string for a given element using JavaScript.
        /// </summary>
        /// <param name="element">The web element to generate XPath for.</param>
        /// <returns>The XPath string for the specified element.</returns>
        public static string GetXPathText(this IWebElement element)
        {
            IWebDriver driver = ((IWrapsDriver)element).WrappedDriver;
            IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;

            string script = @"
        function getElementIdx(ele) {
            var count = 1;
            for (var sib = ele.previousSibling; sib; sib = sib.previousSibling) {
                if (sib.nodeType == 1 && sib.tagName === ele.tagName) count++;
            }
            return count;
        }
        var path = '';
        for (var ele = arguments[0]; ele && ele.nodeType == 1; ele = ele.parentNode) {
            var idx = getElementIdx(ele);
            var xname = ele.tagName;
            if (idx > 1) xname += '[' + idx + ']';
            path = '/' + xname + path;
        }
        return path;";

            try
            {
                string fullXPath = (string)executor.ExecuteScript(script, element);
                return fullXPath;  // Removed ToLower to preserve case sensitivity
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to generate XPath for the element.", ex);
            }
        }

        ///<summary>
        /// Sets the state of a checkbox or radio button based on a given input.
        ///</summary>
        ///<param name="radioBox">IWebElement representing a checkbox or radio radioBox in the DOM.</param>
        ///<param name="sInput">Input string indicating Yes/No response; 'yes' for checked and 'no' for unchecked.</param>
        public static void Check(IWebElement radioBox, string sInput)
        {
            if (radioBox == null)
                throw new ArgumentNullException(nameof(radioBox), "The radioBox element cannot be null.");

            if (radioBox.TagName.ToLower() != "input")
                throw new ArgumentException("The element must be an input element.", nameof(radioBox));

            string type = radioBox.GetAttribute("type").ToLower();
            if (!new[] { "radio", "checkbox" }.Contains(type))
                throw new ArgumentException("The element must be of type 'radio' or 'checkbox'.", nameof(type));

            bool? shouldCheck = determineResponse(sInput) ?? throw new ArgumentException("Invalid input. Expected 'yes' or 'no'.", nameof(sInput));

            // Ensure the current selection state matches the desired state.
            if (radioBox.Selected != shouldCheck)
            {
                radioBox.Click();
            }
        }

        /// <summary>
        /// Determines whether the specified element is hidden on the page.
        /// </summary>
        /// <param name="driver">The WebDriver instance used to execute JavaScript.</param>
        /// <param name="element">The web element to check for visibility.</param>
        /// <returns>True if the element is either not displayed or hidden via CSS; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the 'element' is null.</exception>
        public static bool IsElementHidden(this IWebDriver driver, IWebElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element), "Element cannot be null.");

            try
            {
                string script = @"
                const styles = window.getComputedStyle(arguments[0]);
                return styles.display === 'none' || styles.visibility === 'hidden';";

                return driver.ExecuteJavaScript<bool>(script, element);
            }
            catch (WebDriverException ex)
            {
                throw new InvalidOperationException("Failed to execute script on the element.", ex);
            }
        }

        /// <summary>
        /// Checks if an alert is present on the page within the specified delay.
        /// </summary>
        /// <param name="driver">The WebDriver instance this method extends.</param>
        /// <param name="delay">Time delay in milliseconds to wait for the alert.</param>
        /// <returns>True if an alert is found within the given delay; otherwise, false.</returns>
        public static bool IsAlertPresent(this IWebDriver driver, int delay = timeDelay)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(delay));
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.AlertIsPresent());
                return true;
            }
            catch (NoAlertPresentException)
            {
                return false;
            }
        }

        /// <summary>
        /// Closes the currently displayed alert and returns its text.  Initially generated by Selenium IDE.
        /// </summary>
        /// <param name="driver">The WebDriver instance.</param>
        /// <param name="acceptAlert">If true, the alert will be accepted; if false, the alert will be dismissed.</param>
        /// <returns>The text of the alert.</returns>
        /// <exception cref="NoAlertPresentException">Thrown if no alert is present when the method is called.</exception>
        public static string CloseAlertAndGetItsText(this IWebDriver driver, bool acceptAlert = true)
        {
            IAlert alert = driver.SwitchTo().Alert();
            string alertText = alert.Text; // Get the text before closing the alert

            if (acceptAlert)
            {
                alert.Accept();
            }
            else
            {
                alert.Dismiss();
            }

            return alertText;
        }

        /// <summary>
        /// Opens a web browser, navigates to a specified URL, and maximizes the window.
        /// </summary>
        /// <param name="driver">The WebDriver instance.</param>
        /// <param name="url">The URL to navigate to.</param>
        /// <param name="timeOut">Optional. The timeout for the page load. If not specified, a default timeout is used.</param>
        /// <exception cref="UriFormatException">Thrown if the URL is not a valid URI.</exception>
        public static void OpenBrowser(this IWebDriver driver, string url, TimeSpan? timeOut = null)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("URL cannot be null or empty.", nameof(url));

            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                throw new UriFormatException("The provided URL is not in a valid format.");

            timeOut = timeOut ?? TimeSpan.FromSeconds(_defaultTimeSpan);

            driver.Manage().Timeouts().PageLoad = timeOut.Value;
            try
            {
                driver.Navigate().GoToUrl(url);
                driver.Manage().Window.Maximize();
            }
            catch (WebDriverTimeoutException ex)
            {
                throw new WebDriverTimeoutException($"The page did not load within the allotted time of {timeOut.Value.TotalSeconds} seconds.", ex);
            }
        }

        /// <summary>
        /// Navigates to a URL using basic authentication by embedding the credentials.
        /// </summary>
        /// <param name="driver">The WebDriver instance used to navigate.</param>
        /// <param name="URL">The URL to navigate to.</param>
        /// <param name="username">The username for basic authentication.</param>
        /// <param name="password">The password for basic authentication.</param>
        /// <exception cref="ArgumentException">Thrown if the provided URL is not in a valid format.</exception>
        public static void OpenWithBasicAuthURL(this IWebDriver driver, string URL, string username, string password)
        {
            try
            {
                var uriBuilder = new UriBuilder(URL)
                {
                    UserName = Uri.EscapeDataString(username),
                    Password = Uri.EscapeDataString(password)
                };

                driver.Navigate().GoToUrl(uriBuilder.Uri.ToString());
            }
            catch (UriFormatException ex)
            {
                throw new ArgumentException("Provided URL is not in a valid format.", nameof(URL), ex);
            }
        }

    /// <summary>
    /// Opens a new tab with the specified URL, including basic authentication credentials.
    /// </summary>
    /// <param name="tabs">The TabManager instance for managing browser tabs.</param>
    /// <param name="URL">The URL to open.</param>
    /// <param name="username">The username for basic authentication.</param>
    /// <param name="password">The password for basic authentication.</param>
    public static void OpenNewTabWithBasicAuthURL(this TabManager<IWebDriver> tabs, string URL, string username, string password)
        {
            try
            {
                var uriBuilder = new UriBuilder(URL)
                {
                    UserName = Uri.EscapeDataString(username),
                    Password = Uri.EscapeDataString(password)
                };

                tabs.OpenNewTab(uriBuilder.Uri.ToString());
            }
            catch (UriFormatException ex)
            {
                throw new ArgumentException("Provided URL is not in a valid format.", nameof(URL), ex);
            }
        }

        /// <summary>
        /// Clears the browser cache for the current session based on the WebDriver instance provided.
        /// This method supports specific browser types and employs custom routines tailored to each supported browser.
        /// </summary>
        /// <param name="driver">The WebDriver instance used to perform actions on the browser.</param>
        /// <exception cref="NotImplementedException">Thrown when an unsupported browser driver type is encountered or specific functionality is not implemented for a supported browser.</exception>
        /// <remarks>
        /// This method determines the browser type using the provided WebDriver instance and executes
        /// browser-specific commands to navigate to the settings page and clear the browser data.
        /// Supported browsers include Chrome, Firefox, and Edge. Internet Explorer is recognized but not implemented.
        /// 
        /// The method uses explicit waits to ensure that UI elements are interactable before performing actions on them.
        /// Proper error handling is recommended for callers to manage potential timeouts or element not found exceptions
        /// that may arise due to changes in browser settings UI or slow response times.
        /// 
        /// Usage of this method should be carefully managed to avoid disrupting user sessions unintentionally.
        /// </remarks>
        public static void clearCache(this IWebDriver driver)
        {
            ClearSystemCache();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));  // Assuming a default wait time

            switch (driver.GetType().Name)
            {
                case nameof(ChromiumDriver):
                case nameof(ChromeDriver):
                    ClearCacheForChrome(driver, wait);
                    break;
                case nameof(FirefoxDriver):
                    ClearCacheForFirefox(driver, wait);
                    break;
                case nameof(EdgeDriver):
                    ClearCacheForEdge(driver);
                    break;
                case nameof(InternetExplorerDriver):
                    throw new NotImplementedException("Internet Explorer cache clearing not implemented.");
                default:
                    throw new NotImplementedException($"Driver {driver.GetType().Name} is not supported for cache clearing.");
            }
        }

        private static void ClearSystemCache()
        {
            // Selenium accumulates many temp files if it crashes.
            // This commands will prevent too many files from accumulating.
            const string cmd = @"@ECHO OFF & CD %temp% & FOR /d %D IN (*) DO RD /s /q ""%D"" & DEL /F /Q *";
            ExecuteShellCommand(cmd);
        }

        private static void ClearCacheForChrome(IWebDriver driver, WebDriverWait wait)
        {
            // This section navigates to Chrome's privacy and security section.
            // It was updated with ShadowRoot elements to reach the Clear Data button.
            driver.Navigate().GoToUrl("chrome://settings/clearBrowserData");
            driver.wait_A_Moment(timeDelay);
            ShadowRoot shadowRoot1 = driver.getShadowRootElement(By.TagName("settings-ui"));
            ShadowRoot shadowRoot2 = driver.getShadowRootElement(shadowRoot1.FindElement(By.CssSelector("settings-main")));
            ShadowRoot shadowRoot3 = driver.getShadowRootElement(shadowRoot2.FindElement(By.CssSelector("settings-basic-page")));
            ShadowRoot shadowRoot4 = driver.getShadowRootElement(shadowRoot3.FindElement(By.CssSelector("settings-section > settings-privacy-page")));
            driver.wait_A_Moment(timeDelay);  // This is necessary!
            ShadowRoot shadowRoot5 = driver.getShadowRootElement(wait.Until(x => shadowRoot4.FindElement(By.CssSelector("settings-clear-browsing-data-dialog"))));
            IWebElement root6 = shadowRoot5.FindElement(By.CssSelector("#clearBrowsingDataDialog"));

            // This sections reaches the Time Range drop-down list
            // to select "All time".
            IWebElement root2 = root6.FindElement(By.CssSelector("iron-pages")).FindElement(By.CssSelector("settings-dropdown-menu#clearFromBasic"));
            ShadowRoot shadowRoot7 = driver.getShadowRootElement(root2);
            IWebElement ddlTimeRange = shadowRoot7.FindElement(By.CssSelector("select#dropdownMenu"));
            new SelectElement(ddlTimeRange).SelectByValue("4");

            IWebElement clearDataButton = root6.FindElement(By.CssSelector("#clearBrowsingDataConfirm"));
            clearDataButton.Click(); // click that hard to reach button!
        }

        private static void ClearCacheForFirefox(IWebDriver driver, WebDriverWait wait)
        {
            // converted from Python
            driver.Navigate().GoToUrl("about:preferences#privacy");
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(_defaultTimeSpan));

            // Click the "Clear Data..." button under "Cookies and Site Data".
            IWebElement clearSiteDataButton = wait.Until(drv => drv.FindElement(By.CssSelector("#clearSiteDataButton")));
            clearSiteDataButton.Click();

            // Accept the "Clear Data" dialog by clicking on the "Clear" button.
            IWebElement clearDataDialog = wait.Until(drv => drv.FindElement(By.CssSelector("#dialogOverlay-0 > groupbox:nth-child(1) > browser:nth-child(2)")));
            driver.ExecuteJavaScript("const browser = document.querySelector('arguments[0]');" +
                                "browser.contentDocument.documentElement.querySelector('arguments[1]').click();",
                                @"{dialog_selector}", "#clearButton");

            // Accept the confirmation alert.
            IAlert alert = wait.Until(drv => drv.SwitchTo().Alert());
            alert.Accept();
        }

        private static void ClearCacheForEdge(IWebDriver driver)
        {
            driver.Navigate().GoToUrl("edge://settings/clearBrowserData");
            driver.FindElement(By.Id("clear-now")).SendKeys(Keys.Enter);
        }

        public static void wait_A_Moment(this IWebDriver driver, int waitInterval)
        { // Dramatic Pause in milliseconds...
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(waitInterval));
            Thread.Sleep(waitInterval);
            wait.IgnoreExceptionTypes();
        }

        public static void wait_A_Moment(this IWebDriver driver, TimeSpan timeSpan)
        { // Dramatic Pause...
            WebDriverWait wait = new WebDriverWait(driver, timeSpan);
            Thread.Sleep(timeSpan);
            wait.IgnoreExceptionTypes();
        }

        /// <summary>
        /// Checks if an element exists on the current web page.
        /// </summary>
        /// <param name="driver">The WebDriver instance.</param>
        /// <param name="locator">The locator used to find the element.</param>
        /// <returns>True if the element is found, otherwise false.</returns>
        public static bool Exists(this IWebDriver driver, By locator)
        {
            if (locator == null)
                throw new ArgumentNullException(nameof(locator));

            // Save the current implicit wait timeout.
            TimeSpan originalTimeout = driver.Manage().Timeouts().ImplicitWait;

            try
            {
                // Set implicit wait to zero to return immediately when an element is not found.
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;
                return driver.FindElements(locator).Count > 0;
            }
            finally
            {
                // Restore the original implicit wait timeout.
                driver.Manage().Timeouts().ImplicitWait = originalTimeout;
            }
        }

        /// <summary>
        /// Checks if an element exists within the context of a specified parent element.
        /// </summary>
        /// <param name="element">The parent element to search within.</param>
        /// <param name="locator">The locator used to find the child element.</param>
        /// <returns>True if at least one element is found matching the locator, otherwise false.</returns>
        public static bool Exists(this IWebElement element, By locator)
        {
            if (locator == null)
                throw new ArgumentNullException(nameof(locator));

            IWebDriver driver = ((IWrapsDriver)element).WrappedDriver;

            // Save the current implicit wait timeout.
            TimeSpan originalTimeout = driver.Manage().Timeouts().ImplicitWait;

            try
            {
                // Set implicit wait to zero to return immediately when an element is not found.
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;
                return element.FindElements(locator).Count > 0;
            }
            finally
            {
                // Restore the original implicit wait timeout.
                driver.Manage().Timeouts().ImplicitWait = originalTimeout;
            }
        }

        /// <summary>
        /// Determines whether the specified element has any child elements.
        /// </summary>
        /// <param name="driver">The WebDriver instance used to manage settings.</param>
        /// <param name="element">The parent element to check for children.</param>
        /// <returns>True if the element has one or more child elements; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the provided element is null.</exception>
        public static bool HasChildren(this IWebDriver driver, IWebElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            // Save the current implicit wait timeout.
            TimeSpan originalTimeout = driver.Manage().Timeouts().ImplicitWait;

            try
            {
                // Set implicit wait to zero to return immediately when no children are found.
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;
                // Check if there are any direct children
                return element.FindElements(By.XPath("./*")).Count > 0;
            }
            catch
            {
                // Return false if no elements are found
                return false;
            }
            finally
            {
                // Restore the original implicit wait timeout.
                driver.Manage().Timeouts().ImplicitWait = originalTimeout;
            }
        }


        /// <summary>
        /// Determines whether the specified element has any children.
        /// </summary>
        /// <param name="element">The parent element to check for children.</param>
        /// <returns>True if the element has one or more child elements; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the provided element is null.</exception>
        public static bool HasChildren(this IWebElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            IWebDriver driver = ((IWrapsDriver)element).WrappedDriver;

            // Save the current implicit wait timeout.
            TimeSpan originalTimeout = driver.Manage().Timeouts().ImplicitWait;

            try
            {
                // Set implicit wait to zero to return immediately when no children are found.
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;
                // Check if there are any direct children
                return element.FindElements(By.XPath("./*")).Count > 0;
            }
            catch
            {
                // Return false if no elements are found
                return false;
            }
            finally
            {
                // Restore the original implicit wait timeout.
                driver.Manage().Timeouts().ImplicitWait = originalTimeout;
            }
        }

        /// <summary>
        /// Retrieves all row elements from the specified table element.
        /// </summary>
        /// <param name="table">The table element from which to retrieve row elements.</param>
        /// <returns>An IEnumerable of IWebElement representing the rows in the table.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the input 'table' element is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the input element is not a 'table'.</exception>
        public static IEnumerable<IWebElement> getTableRows(IWebElement table)
        {
            if (table == null)
                throw new ArgumentNullException(nameof(table), "Provided table element is null.");

            if (table.TagName.ToLower() != "table")
                throw new ArgumentException("Provided element is not a table.", nameof(table));

            return table.FindElements(By.TagName("tr"));
        }


        /// <summary>
        /// Retrieves the text from each cell of a specified row within a table element.
        /// </summary>
        /// <param name="table">The table element from which to retrieve the data.</param>
        /// <param name="rowNumber">The zero-based index of the row from which to retrieve the data.</param>
        /// <returns>A ReadOnlyCollection containing the text of each cell in the specified row.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the 'table' argument is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the 'table' argument is not an HTML table.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the 'rowNumber' is out of the range of existing rows.</exception>
        public static ReadOnlyCollection<string> getRow(IWebElement table, int rowNumber)
        {
            if (table == null)
                throw new ArgumentNullException(nameof(table), "Provided table element is null.");
            if (table.TagName.ToLower() != "table")
                throw new ArgumentException("Provided element is not a table.", nameof(table));
            if (rowNumber < 0)
                throw new ArgumentOutOfRangeException(nameof(rowNumber), "Row number cannot be negative.");

            List<IWebElement> rows = getTableRows(table).ToList();
            if (rowNumber >= rows.Count)
                throw new ArgumentOutOfRangeException(nameof(rowNumber), "Row number is out of range.");

            List<string> cellTexts = rows[rowNumber].FindElements(By.TagName("td")).Select(cell => cell.Text).ToList();

            return new ReadOnlyCollection<string>(cellTexts);
        }


        /// <summary>
        /// Retrieves the text from each cell of a specified column within a table element.
        /// </summary>
        /// <param name="table">The table element from which to retrieve the data.</param>
        /// <param name="colNumber">The zero-based index of the column from which to retrieve the data.</param>
        /// <returns>A ReadOnlyCollection containing the text of each cell in the specified column.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the 'table' argument is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the 'table' argument is not an HTML table or if the 'colNumber' is invalid.</exception>
        public static ReadOnlyCollection<string> getColumn(IWebElement table, int colNumber)
        {
            if (table == null)
                throw new ArgumentNullException(nameof(table), "Provided table element is null.");
            if (table.TagName.ToLower() != "table")
                throw new ArgumentException("Provided element is not a table.", nameof(table));
            if (colNumber < 0)
                throw new ArgumentOutOfRangeException(nameof(colNumber), "Column number cannot be negative.");

            List<string> result = new List<string>();
            var rows = getTableRows(table).ToList();  // Ensures that all rows are fetched once.

            foreach (var row in rows)
            {
                var cells = row.FindElements(By.TagName("td"));
                if (colNumber >= cells.Count)  // Check if the column number is valid for the current row
                    throw new ArgumentOutOfRangeException(nameof(colNumber), "Column number is out of range for the row with index " + rows.IndexOf(row) + ".");

                result.Add(cells[colNumber].Text);
            }

            return new ReadOnlyCollection<string>(result);
        }

        /// <summary>
        /// Takes a screenshot of the current browser window and saves it to a specified path.
        /// </summary>
        /// <param name="driver">The WebDriver instance used to take the screenshot.</param>
        /// <param name="sPath">The file path where the screenshot will be saved.</param>
        /// <param name="format">The image format for the screenshot. Default is JPEG if not specified.</param>
        /// <exception cref="ArgumentNullException">Thrown if the provided path is null or empty.</exception>
        /// <exception cref="ArgumentException">Thrown if the specified path is invalid or the directory does not exist.</exception>
        /// <exception cref="IOException">Thrown if an I/O error occurs during saving the file.</exception>
        public static void TakeScreenshot(this IWebDriver driver, string sPath, ImageFormat format = null)
        {
            if (string.IsNullOrWhiteSpace(sPath))
                throw new ArgumentNullException(nameof(sPath), "Screenshot path cannot be null or empty.");

            if (!Directory.Exists(Path.GetDirectoryName(sPath)))
                throw new ArgumentException("Directory does not exist.", nameof(sPath));

            Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();

            // Set default format if not specified
            format = format ?? ImageFormat.Jpeg;

            try
            {
                // Capture browser viewport
                using (MemoryStream imageStream = new MemoryStream(ss.AsByteArray))
                using (FileStream fileStream = new FileStream(sPath, FileMode.Create))
                using (Image screenshotImage = Image.FromStream(imageStream))
                {
                    screenshotImage.Save(fileStream, format);
                }
            }
            catch (Exception ex)
            {
                throw new IOException("Failed to save the screenshot.", ex);
            }
        }

        /// <summary>
        /// Takes a screenshot of the entire screen and saves it to a specified path.
        /// </summary>
        /// <param name="sPath">The file path where the screenshot will be saved.</param>
        /// <param name="format">The image format for the screenshot. Default is JPEG if not specified.</param>
        /// <exception cref="ArgumentNullException">Thrown if the provided path is null or empty.</exception>
        /// <exception cref="ArgumentException">Thrown if the specified path is invalid or the directory does not exist.</exception>
        /// <exception cref="IOException">Thrown if an I/O error occurs during saving the file.</exception>
        public static void TakeFullScreenshot(string sPath, ImageFormat format = null)
        {
            if (string.IsNullOrWhiteSpace(sPath))
                throw new ArgumentNullException(nameof(sPath), "Screenshot path cannot be null or empty.");

            if (!Directory.Exists(Path.GetDirectoryName(sPath)))
                throw new ArgumentException("Directory does not exist.", nameof(sPath));

            format = format ?? ImageFormat.Jpeg;

            try
            {
                // Capture the entire screen
                Rectangle bounds = Screen.GetBounds(Point.Empty);
                using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                    }

                    // Save the screenshot
                    bitmap.Save(sPath, format);
                }
            }
            catch (Exception ex)
            {
                throw new IOException("Failed to save the screenshot.", ex);
            }
        }

        /// <summary>
        /// Determines whether the input string corresponds to a known response and what type.
        /// </summary>
        /// <param name="sInput">The input response to evaluate.</param>
        /// <returns>True if positive, False if negative, null if unknown.</returns>
        public static bool? determineResponse(string sInput)
        {
            sInput = sInput.ToLower();
            if (PositiveResponses.Contains(sInput))
                return true;
            if (NegativeResponses.Contains(sInput))
                return false;
            return null;
        }

        /// <summary>
        /// Checks if the given string is a positive response.
        /// </summary>
        /// <param name="sInput">The input string to check.</param>
        /// <returns>True if the input is a positive response, otherwise false.</returns>
        public static bool IsPositiveResponse(string sInput)
        {
            return PositiveResponses.Contains(sInput.ToLower());
        }

        /// <summary>
        /// Checks if the given string is a negative response.
        /// </summary>
        /// <param name="sInput">The input string to check.</param>
        /// <returns>True if the input is a negative response, otherwise false.</returns>
        public static bool IsNegativeResponse(string sInput)
        {
            return NegativeResponses.Contains(sInput.ToLower());
        }

        /// <summary>
        /// Waits for a busy indicator to appear and disappear within the specified timeout.
        /// </summary>
        /// <param name="driver">The WebDriver instance.</param>
        /// <param name="locator">The locator for the busy indicator element.</param>
        /// <param name="timeOut">The maximum time to wait for the busy indicator. If not specified, a default timeout is used.</param>
        public static void PauseOnBusyIndicator(this IWebDriver driver, By locator, TimeSpan? timeOut = null)
        {
            TimeSpan effectiveTimeOut = timeOut ?? TimeSpan.FromSeconds(_defaultTimeSpan);
            WebDriverWait wait = new WebDriverWait(driver, effectiveTimeOut);

            try
            {
                // Wait for the loader to appear and then disappear
                wait.Until(ExpectedConditions.ElementIsVisible(locator));
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(locator));
            }
            catch (WebDriverTimeoutException ex)
            {
                throw new WebDriverTimeoutException($"Timeout waiting for the busy indicator to complete within {effectiveTimeOut.TotalSeconds} seconds.", ex);
            }
        }

        /// <summary>
        /// Forcefully terminates all processes associated with a specified browser type.
        /// </summary>
        /// <param name="browser">The browser type whose processes are to be terminated.</param>
        public static void CloseProcesses(BrowserType browser)
        {
            string[] processNames = GetProcessByName(browser) ?? throw new NotImplementedException($"Driver {browser} is not supported");
            foreach (string processName in processNames)
            {
                foreach (Process process in Process.GetProcessesByName(processName))
                {
                    try
                    {
                        process.Kill();
                    }
                    catch (Exception ex)
                    {
                        // Consider logging the exception with details of the process attempted to kill
                        Debug.WriteLine($"Failed to kill process {processName}: {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Returns an array of process names associated with each browser type.
        /// </summary>
        /// <param name="browser">The browser type.</param>
        /// <returns>An array of process names to terminate, or null if the browser type is not supported.</returns>
        private static string[] GetProcessByName(BrowserType browser)
        {
            switch (browser)
            {
                case BrowserType.Chrome:
                case BrowserType.Chromium:
                    return new[] { "chromedriver", "chrome" };
                case BrowserType.Firefox:
                    return new[] { "geckodriver", "firefox" };
                case BrowserType.Edge:
                    return new[] { "msedgedriver", "msedge" };
                case BrowserType.IE:
                    return new[] { "IEDriverServer", "iexplore" };
                default:
                    return null;
            }
        }

        public static byte[] FileDownloaderXHR(this IWebDriver driver, string url)
        {
            // Experimental: Converted from Java
            string script = "var url = arguments[0];" +
            "var callback = arguments[arguments.length - 1];" +
            "var xhr = new XMLHttpRequest();" +
            "xhr.open('GET', url, true);" +
            "xhr.responseType = 'arraybuffer';" + //force the HTTP response, response-type header to be array buffer
            "xhr.onload = function() {" +
            "  var arrayBuffer = xhr.response;" +
            "  var byteArray = new Uint16Array(arrayBuffer);" +  // originally Uint8Array()
            "  callback(byteArray);" +
            "};" +
            "xhr.send();";

            object response = ((IJavaScriptExecutor)driver).ExecuteAsyncScript(script, url);

            // Selenium returns an Array of UInt16, we need byte[]
            List<ushort> byteList = (List<ushort>)response;
            byte[] bytes = new byte[byteList.Count()];
            for (int i = 0; i < byteList.Count(); i++)
            {
                bytes[i] = (byte)byteList[i];
            }
            return bytes;
        }

        public static void waitUntilDownloadCompleted(this IWebDriver driver, string newFileName)
        {
            string scriptFileName, scriptPercentage, downloadsURL, fileName;

            switch (driver.GetType().ToString())
            {
                case nameof(ChromeDriver):
                    // sourceURL: use "document.querySelector('downloads-manager').shadowRoot.querySelector('#downloadsList downloads-item').shadowRoot.querySelector('div#content #file-link').href"
                    // downloadLocation: use "document.querySelector('downloads-manager').shadowRoot.querySelector('#downloadsList downloads-item').shadowRoot.querySelector('div.is-active.focus-row-active #file-icon-wrapper img').src"
                    scriptFileName = "return document.querySelector('downloads-manager').shadowRoot.querySelector('#downloadsList downloads-item').shadowRoot.querySelector('div#content #file-link').text";
                    scriptPercentage = "return document.querySelector('downloads-manager').shadowRoot.querySelector('#downloadsList downloads-item').shadowRoot.querySelector('#progress').value";
                    downloadsURL = "chrome://downloads";
                    break;

                case nameof(FirefoxDriver):
                    scriptFileName = "return document.querySelector('#contentAreaDownloadsView description:nth-of-type(1)').value";
                    scriptPercentage = "return document.querySelector('#contentAreaDownloadsView richlistitem.download:nth-child(1) > hbox:nth-child(1) > vbox:nth-child(2) > progress:nth-child(2)').value";
                    downloadsURL = "about:downloads";
                    break;

                default:
                    throw new NotImplementedException(string.Format("Driver {0} is not supported", driver.GetType().ToString()));
            }

            // Store the current window handle
            string mainWindow = driver.CurrentWindowHandle;

            // open new tab and switch focus
            IJavaScriptExecutor js = driver as IJavaScriptExecutor;
            driver.SwitchTo().Window(mainWindow);
            js.ExecuteScript("window.open();");
            driver.SwitchTo().Window(driver.WindowHandles.Last());

            // navigate to downloads
            driver.Navigate().GoToUrl(downloadsURL);

            // wait until download is complete
            IJavaScriptExecutor js1 = driver as IJavaScriptExecutor;
            long percentage = 0;
            while (percentage != 100)
            {
                try
                {
                    percentage = (long)js1.ExecuteScript(scriptPercentage);
                }
                catch (Exception)
                {
                    // Nothing to do just wait
                }
                Thread.Sleep(1000);
            }

            // get the latest downloaded file name
            fileName = Path.Combine(Environment.ExpandEnvironmentVariables("%USERPROFILE%"), "Downloads", (string)js1.ExecuteScript(scriptFileName));

            // close the downloads tab
            driver.Close();

            // switch back to main window
            driver.SwitchTo().Window(mainWindow);
            driver.wait_A_Moment(timeDelay);

            // delete if new file exists
            if (File.Exists(newFileName))
                File.Delete(newFileName);

            // rename downloaded file
            File.Move(fileName, newFileName);
            File.Delete(fileName);
        }

        public static bool PerformActionWithRetry(this IWebDriver driver, TimeSpan timeout, int attempts, Func<IWebDriver, bool> action)
        {
            DateTime endTime = DateTime.Now.Add(timeout);
            int iterations = 0;

            while (DateTime.Now < endTime && iterations < attempts)
            {
                try
                {
                    if (action(driver))
                    {
                        return true; // Action succeeded, exit the loop
                    }
                }
                catch (StaleElementReferenceException ex)
                {
                    // Log the exception message as a warning
                    Debug.WriteLine($"Warning: {ex.Message}");
                }

                iterations++;

                // Sleep for a short duration before retrying
                System.Threading.Thread.Sleep(timeDelay);
            }

            // Throw an exception or handle the failure scenario after all attempts
            Debug.WriteLine($"Action failed after {attempts} attempts within the specified timeout.");
            return false;
        }
        #endregion

        #region [Private Functions]
        private static void ExecuteShellCommand(string Command)
        {  // Windows Shell Command
            ProcessStartInfo ProcessInfo;

            ProcessInfo = new ProcessStartInfo("cmd.exe", "/C " + Command)
            {
                CreateNoWindow = true,
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            Process.Start(ProcessInfo);
        }

        private static ShadowRoot getShadowRootElement(this IWebDriver driver, By locator)
        {  // Return shadowroot by locator strategy
            IWebElement element = driver.FindElement(locator);
            return driver.ExecuteJavaScript<ShadowRoot>("return arguments[0].shadowRoot", element);
        }

        private static ShadowRoot getShadowRootElement(this IWebDriver driver, IWebElement element)
        {  // Return shadowroot of given element
            return driver.ExecuteJavaScript<ShadowRoot>("return arguments[0].shadowRoot", element);
        }
        #endregion
    }
}
