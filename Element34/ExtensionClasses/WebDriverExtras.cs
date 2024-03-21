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
using Keys = OpenQA.Selenium.Keys;


namespace Element34.ExtensionClasses
{
    public static class WebDriverExtras
    {
        #region Fields
        private static bool _acceptNextAlert = true;
        private const int _defaultTimeSpan = 45;
        private const int timeDelay = 1500;
        //private static Logger _logger;
        #endregion

        public static string[] positiveResponse = new string[] { "y", "yes", "t", "true", "1", "+", "affirmative", "positive" };
        public static string[] negativeResponse = new string[] { "n", "no", "f", "false", "0", "-", "negative" };

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

        public static string InnerText(this IWebElement element)
        {
            return element.GetAttribute("innerText");
        }

        public static string Value(this IWebElement element)
        {
            return element.GetAttribute("value");
        }

        public static void actionClick(this IWebDriver driver, IWebElement element)
        {  // Experimental: Method extension of actionClick() for WebDriver
            Actions actionBuilder = new Actions(driver);
            var flag = false;

            while (flag == false)
            {
                IAction act = actionBuilder
                    .MoveToElement(element)
                    .Click()
                    .Build();
                try
                {
                    var tmp = element.TagName;
                    if (tmp == "html")
                        break;

                    act.Perform();
                    flag = true;
                }
                catch (ElementNotInteractableException)
                {
                    var e = element.FindElement(By.XPath(".."));
                    element = e;
                }
            }
        }

        public static void actionClick(this IWebElement element)
        {  // Experimental: If element is not clickable, move up to parent element and try again utilizing an Action Builder
            var driver = ((IWrapsDriver)element).WrappedDriver;
            Actions actionBuilder = new Actions(driver);
            var flag = false;

            while (flag == false)
            {
                IAction act = actionBuilder
                    .MoveToElement(element)
                    .Click()
                    .Build();
                try
                {
                    var tmp = element.TagName;
                    if (tmp == "html")
                        break;

                    act.Perform();
                    flag = true;
                }
                catch (ElementNotInteractableException)
                {
                    var e = element.FindElement(By.XPath(".."));
                    element = e;
                }
            }
        }

        public static void actionType(this IWebDriver driver, IWebElement element, string sInput)
        {  // Experimental: Method extension of actionType() for WebDriver
            Actions actionBuilder = new Actions(driver);
            IAction act = actionBuilder
                .MoveToElement(element)
                .Click()
                .KeyDown(Keys.Control)
                .KeyDown("A")
                .KeyUp("A")
                .KeyUp(Keys.Control)
                .SendKeys(sInput)
                .Build();

            act.Perform();
        }

        public static void actionType(this IWebElement element, string sInput)
        {  // Experimental: Consolidated steps for typing in a string value using an Action Builder
            var driver = ((IWrapsDriver)element).WrappedDriver;
            Actions actionBuilder = new Actions(driver);
            IAction act = actionBuilder
                .MoveToElement(element)
                .Click()
                .KeyDown(Keys.Control)
                .KeyDown("A")
                .KeyUp("A")
                .KeyUp(Keys.Control)
                .SendKeys(sInput)
                .Build();

            act.Perform();
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

        public static void Type(this IWebDriver driver, By locator, string sInput)
        {
            IWebElement webElement = driver.FindElement(locator);
            webElement.Type(sInput);
        }

        public static void Type(this IWebElement webElement, string sInput)
        {
            if (webElement == null)
                throw new ArgumentNullException(nameof(webElement));
            else if (webElement.TagName != "input")
                throw new ArgumentException("tag name");

            string type = webElement.GetAttribute("type");
            if (!compareAnyStr(type.ToLower(), new string[] { "text", "password", "email", "date", "datetime-local", "month", "number", "search", "tel", "time", "url", "week" }))
                throw new ArgumentException("type attribute");

            webElement.Click();
            webElement.Clear();
            webElement.SendKeys(sInput);
        }

        public static bool testMaxLength(IWebElement webElement, int limit)
        {
            if (webElement == null)
                throw new ArgumentNullException(nameof(webElement));
            else if (webElement.TagName != "input")
                throw new ArgumentException("tag name");

            string type = webElement.GetAttribute("type");
            if (!compareAnyStr(type.ToLower(), new string[] { "text", "password", "email", "number", "search", "url" }))
                throw new ArgumentException("type attribute");

            // Clear field
            webElement.Click();
            webElement.Clear();

            // Type more than max limit
            for (int i = 1; i < (limit + 20); i++)
                webElement.SendKeys((i % 10).ToString());

            // Get the typed value
            string typedValue = webElement.GetAttribute("value");

            // Get the length of typed value
            int size = typedValue.Length;

            // Assert size limit equals expected value
            return (size == limit);
        }

        public static By GetCssSelectLocator(this IWebElement element)
        {// Experimental
            string cssSelector = GetCssSelectText(element);
            return By.CssSelector(cssSelector);
        }

        public static string GetCssSelectText(this IWebElement element)
        {
            // Use JavaScript to obtain the full CSS Selector
            IWebDriver driver = ((IWrapsDriver)element).WrappedDriver;
            IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;

            string script =
            "    var el = arguments[0];" +
            "    if (!(el instanceof Element)) return;" +
            "    var path = [];" +
            "    while (el.nodeType === Node.ELEMENT_NODE) {" +
            "        var selector = el.nodeName.toLowerCase();" +
            "        if (el.id) {" +
            "            selector += '#' + el.id;" +
            "            path.unshift(selector);" +
            "            break;" +
            "        } else {" +
            "            var sibling = el;" +
            "            var nth = 1;" +
            "            while (sibling = sibling.previousElementSibling) {" +
            "                if (sibling.nodeName.toLowerCase() == selector) {" +
            "                    nth++;" +
            "                }" +
            "            }" +
            "            if (nth != 1) {" +
            "                selector += ':nth-of-type(' + nth + ')';" +
            "            }" +
            "        }" +
            "        path.unshift(selector);" +
            "        el = el.parentNode;" +
            "    }" +
            "    return path.join(' > ');";

            string cssSelector = (string)executor.ExecuteScript(script, element);

            if (cssSelector != string.Empty)
            {
                cssSelector = "." + cssSelector;
            }
            return cssSelector;
        }

        public static By GetXPathLocator(this IWebElement element)
        {
            return By.XPath(element.GetXPathText());
        }

        public static string GetXPathText(this IWebElement element)
        {
            // Use JavaScript to obtain the full XPath
            IWebDriver driver = ((IWrapsDriver)element).WrappedDriver;
            IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;

            string script =
            "function getElementIdx(ele) {" +
            "    var count = 1;" +
            "    for (var sib = ele.previousSibling; sib ; sib = sib.previousSibling) {" +
            "        if (sib.nodeType == 1 && sib.tagName == ele.tagName) count++;" +
            "    }" +
            "    return count;" +
            "}" +
            "    var path = '';" +
            "    var ele = arguments[0];" +
            "    for (; ele && ele.nodeType == 1; ele = ele.parentNode) {" +
            "        var idx = getElementIdx(ele);" +
            "        var xname = ele.tagName;" +
            "        if (idx > 1) xname += '[' + idx + ']';" +
            "        path = '/' + xname + path;" +
            "    }" +
            "    return path;";

            string fullXPath = (string)executor.ExecuteScript(script, element);
            return fullXPath.ToLower();
        }

        ///<summary>
        ///Check:
        /// 1) Absolute selection state of control.
        /// 2) Ensure checkbox or radio button is the specified value of sInput is regardless of initial state.
        ///</summary>
        ///<param name="radioBox">IWebElement object representing checkbox or radio button in DOM.</param>
        ///<param name="sInput">Input string indicating Yes/No response.</param>
        public static void Check(IWebElement radioBox, string sInput)
        {
            if (radioBox == null)
                throw new ArgumentNullException(nameof(radioBox));
            else if (radioBox.TagName != "input")
                throw new ArgumentException("tag name");

            string type = radioBox.GetAttribute("type");
            if (!compareAnyStr(type.ToLower(), new string[] { "radio", "checkbox" }))
                throw new ArgumentException("type attribute");

            IWebDriver driver = ((IWrapsDriver)radioBox).WrappedDriver;
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("arguments[0].focus();", radioBox);

            bool blnSelected = radioBox.Selected;
            bool? blnInput = determineResponse(sInput);

            if (blnInput != null)
            {
                if ((bool)blnInput)
                {
                    if (!blnSelected)
                    {
                        radioBox.Click();
                    }

                    else
                    {
                        if (blnSelected)
                        {
                            radioBox.Click();
                        }
                    }
                }
            }
        }

        public static bool IsElementHidden(this IWebDriver driver, By locator)
        {
            return driver.IsElementHidden(driver.FindElement(locator));
        }

        public static bool IsElementHidden(this IWebDriver driver, IWebElement element)
        {
            return driver.ExecuteJavaScript<bool>("const styles = window.getComputedStyle(arguments[0]); return styles.display === 'none' || styles.visibility === 'hidden';", element);
        }

        /// <summary>
        /// Extension method to check if an alert box is present.  Initially generated by Katalon Recorder.
        /// </summary>
        /// <param name="driver">The type (e.g. IWebDriver) that the method operates on.</param>
        /// <param name="delay">Time delay in ms.</param>
        /// <returns>True if found.</returns>
        public static bool IsAlertPresent(this IWebDriver driver, int delay = timeDelay)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(delay));
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.AlertIsPresent());
                driver.SwitchTo().Alert();
                return true;
            }
            catch (StaleElementReferenceException)
            {
                return false;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public static string CloseAlertAndGetItsText(this IWebDriver driver)
        { // Generated by Selenium IDE
            try
            {
                IAlert alert = driver.SwitchTo().Alert();
                string alertText = alert.Text;
                if (_acceptNextAlert)
                {
                    alert.Accept();
                }
                else
                {
                    alert.Dismiss();
                }
                return alertText;
            }
            finally
            {
                _acceptNextAlert = true;
            }
        }

        public static void OpenBrowser(this IWebDriver driver, string URL, TimeSpan? timeOut = null)
        {  // GoToUrl() and Maximize() window in given time span
            timeOut = (timeOut == null) ? TimeSpan.FromSeconds(_defaultTimeSpan) : timeOut.Value;     // default value for TimeSpan parameter

            driver.Manage().Timeouts().PageLoad = (TimeSpan)timeOut;
            driver.Navigate().GoToUrl(URL);
            driver.Manage().Window.Maximize();
        }

        public static void OpenWithBasicAuthURL(this IWebDriver driver, string URL, string username, string password)
        {   // Workaround for Selenium WebDriver's lack of support for Basic HTTP Authentication.
            string sDoubleSlash = @"//";
            int iDblSlashOffset = URL.IndexOf(sDoubleSlash) + sDoubleSlash.Length;
            URL = URL.Substring(0, iDblSlashOffset) + Uri.EscapeDataString(username) + ":" + Uri.EscapeDataString(password) + "@" + URL.Substring(iDblSlashOffset, URL.Length - iDblSlashOffset);
            driver.OpenBrowser(URL);
        }

        public static void OpenNewTabWithBasicAuthURL(this TabManager<IWebDriver> tabs, string URL, string username, string password)
        {   // Workaround for Selenium WebDriver's lack of support for Basic HTTP Authentication.
            string sDoubleSlash = @"//";
            int iDblSlashOffset = URL.IndexOf(sDoubleSlash) + sDoubleSlash.Length;
            URL = URL.Substring(0, iDblSlashOffset) + Uri.EscapeDataString(username) + ":" + Uri.EscapeDataString(password) + "@" + URL.Substring(iDblSlashOffset, URL.Length - iDblSlashOffset);
            tabs.OpenNewTab(URL);
        }

        public static void clearCache(this IWebDriver driver)
        {
            // Selenium accumulates many temp files if it crashes.
            // These commands will prevent too many files from accumulating.
            const string cmd = @"@ECHO OFF & CD %temp% & FOR /d %D IN (*) DO RD /s /q ""%D"" & DEL /F /Q *";
            ExecuteShellCommand(cmd);

            var wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(timeDelay));

            switch (driver.GetType().Name)
            {
                case nameof(ChromiumDriver):
                case nameof(ChromeDriver):
                    // This section navigates to Chrome's privacy and security section.
                    // It was updated with ShadowRoot elements to reach the Clear Data button.
                    driver.Navigate().GoToUrl("chrome://settings/clearBrowserData");
                    ShadowRoot shadowRoot1 = driver.getShadowRootElement(By.TagName("settings-ui"));
                    ShadowRoot shadowRoot2 = driver.getShadowRootElement(shadowRoot1.FindElement(By.CssSelector("settings-main")));
                    ShadowRoot shadowRoot3 = driver.getShadowRootElement(shadowRoot2.FindElement(By.CssSelector("settings-basic-page")));
                    ShadowRoot shadowRoot4 = driver.getShadowRootElement(shadowRoot3.FindElement(By.CssSelector("settings-section > settings-privacy-page")));
                    driver.wait_A_Moment(timeDelay);  // This is necessary!
                    ShadowRoot shadowRoot5 = driver.getShadowRootElement(wait.Until(x => shadowRoot4.FindElement(By.CssSelector("settings-clear-browsing-data-dialog"))));
                    IWebElement root6 = shadowRoot5.FindElement(By.CssSelector("#clearBrowsingDataDialog"));

                    // This sections reaches the Time Range drop-down list
                    // to select "All time".
                    IWebElement root2 = root6.FindElement(By.CssSelector("iron-pages#tabs")).FindElement(By.CssSelector("settings-dropdown-menu#clearFromBasic"));
                    ShadowRoot shadowRoot7 = driver.getShadowRootElement(root2);
                    IWebElement ddlTimeRange = shadowRoot7.FindElement(By.CssSelector("select#dropdownMenu"));
                    new SelectElement(ddlTimeRange).SelectByValue("4");

                    IWebElement clearDataButton = root6.FindElement(By.CssSelector("#clearBrowsingDataConfirm"));
                    clearDataButton.Click(); // click that hard to reach button!
                    break;

                case nameof(FirefoxDriver):
                    // converted from Python
                    driver.Navigate().GoToUrl("about:preferences#privacy");
                    wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

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
                    break;

                case nameof(EdgeDriver):
                    driver.Navigate().GoToUrl("edge://settings/clearBrowserData");
                    driver.FindElement(By.XPath("//*[@id='modal-root']//button[@id='selecttrigger-35']")).Click();

                    break;

                case nameof(InternetExplorerDriver):
                    throw new NotImplementedException();

                default:
                    throw new NotImplementedException(string.Format("Driver {0} is not supported", driver.GetType().Name));
            }
        }

        public static void wait_A_Moment(this IWebDriver driver, int waitInterval)
        { // Dramatic Pause in milliseconds...
            Thread.Sleep(waitInterval);
            Thread.Yield();
        }

        public static void wait_A_Moment(this IWebDriver driver, TimeSpan timeSpan)
        { // Dramatic Pause...
            Thread.Sleep(timeSpan);
            Thread.Yield();
        }

        public static bool Exists(this IWebDriver driver, By locator)
        {
            if (locator == null)
                throw new ArgumentNullException(nameof(locator));

            bool exists = false;

            //Save implicit timeout to reset it later 
            TimeSpan tmp = driver.Manage().Timeouts().ImplicitWait;

            try
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;
                exists = driver.FindElements(locator).Count > 0;
            }
            catch { }
            finally
            {
                driver.Manage().Timeouts().ImplicitWait = tmp;
            }

            return exists;
        }

        public static bool HasChildren(this IWebDriver driver, IWebElement element)
        {  // Determine if element has **ANY** children at that moment
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            bool exists = false;

            //Save implicit timeout to reset it later 
            TimeSpan tmp = driver.Manage().Timeouts().ImplicitWait;

            try
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;
                exists = element.FindElements(By.XPath(".//*")).Count > 0;
            }
            catch { }
            finally
            {
                driver.Manage().Timeouts().ImplicitWait = tmp;
            }

            return exists;
        }

        public static bool HasChildren(this IWebElement element)
        {  // Determine if element has **ANY** children at that moment
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            IWebDriver driver = ((IWrapsDriver)element).WrappedDriver;
            bool exists = false;

            //Save implicit timeout to reset it later 
            TimeSpan tmp = driver.Manage().Timeouts().ImplicitWait;

            try
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;
                exists = element.FindElements(By.XPath(".//*")).Count > 0;
            }
            catch { }
            finally
            {
                driver.Manage().Timeouts().ImplicitWait = tmp;
            }

            return exists;
        }

        public static IEnumerable<IWebElement> getTableRows(IWebElement table)
        {  // Return read-only collection of table rows
            if (table == null)
                throw new ArgumentNullException(nameof(table));
            else if (table.TagName != "table")
                throw new ArgumentException("tag name");

            IEnumerable<IWebElement> rows = table.FindElements(By.TagName("tr"));
            return rows;
        }

        public static ReadOnlyCollection<string> getRow(IWebElement table, int rowNumber)
        {
            if (table == null)
                throw new ArgumentNullException(nameof(table));
            else if (table.TagName != "table")
                throw new ArgumentException("tag name");

            IEnumerable<IWebElement> row = getTableRows(table).ElementAt(rowNumber).FindElements(By.TagName("td"));
            List<string> result = new List<string>();
            foreach (var cell in row)
            {
                result.Add(cell.Text);
            }

            return new ReadOnlyCollection<string>(result);
        }

        public static ReadOnlyCollection<string> getColumn(IWebElement table, int colNumber)
        {
            if (table == null)
                throw new ArgumentNullException(nameof(table));
            else if (table.TagName != "table")
                throw new ArgumentException("tag name");

            List<string> result = new List<string>();
            foreach (var row in getTableRows(table))
            {
                result.Add(row.FindElements(By.TagName("td"))[colNumber].Text);
            }

            return new ReadOnlyCollection<string>(result);
        }

        public static void TakeScreenshot(this IWebDriver driver, string sPath, ImageFormat format = null)
        {  // Take snapshot of current web browser screen and save to specified file
            Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();

            if (format == null)
                format = ImageFormat.Jpeg;

            using (MemoryStream imageStream = new MemoryStream(ss.AsByteArray))
            using (FileStream fileStream = new FileStream(sPath, FileMode.Create))
            using (Image screenshotImage = Image.FromStream(imageStream))
            {
                screenshotImage.Save(fileStream, format);
            }
        }

        public static bool? determineResponse(string sInput)
        {
            bool? response = null;
            string[] acceptedResponse = positiveResponse.Concat(negativeResponse).ToArray();

            if (acceptedResponse.Any(testElement => testElement == sInput.ToLower()))
            {
                response = positiveResponse.Any(testElement => testElement == sInput.ToLower()) || negativeResponse.Any(testElement => testElement == sInput.ToLower());
            }

            return response;
        }

        public static bool IsPositiveResponse(string sInput)
        {
            /*
                Is Positive Response?
                test input for *any* response considered positive
            */

            return positiveResponse.Any(testElement => testElement == sInput.ToLower());
        }

        public static bool IsNegativeResponse(string sInput)
        {
            /*
                Is Negative Response?
                test input for *any* response considered negative
            */

            return negativeResponse.Any(testElement => testElement == sInput.ToLower());
        }

        public static bool compareAnyStr(string sInput, string[] testArray)
        {
            /*
                test input for *any* response contained in testArray
                public facing function exposing comparAny() function

                sInput; input string
                testArray; string array for comparison with input
            */

            return testArray.Any(testElement => testElement == sInput.ToLower());
        }

        public static void PauseOnBusyIndicator(this IWebDriver driver, By locator, TimeSpan? timeOut = null)
        {
            timeOut = (timeOut == null) ? TimeSpan.FromSeconds(_defaultTimeSpan) : timeOut.Value;                   // default value for TimeSpan parameter
            WebDriverWait wait = new WebDriverWait(driver, (TimeSpan)timeOut);
            wait.Until(ExpectedConditions.ElementIsVisible(locator));                                               //wait for the loader to appear
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(locator));                                   //wait for the loader to disappear
            driver.wait_A_Moment(timeDelay);
        }

        public static void CloseProcesses(BrowserType browser)
        {  // Forcefully kill off old test processes from previous iteration(s)
            switch (browser)
            {
                case BrowserType.Chrome:
                case BrowserType.Chromium:
                    foreach (Process p in Process.GetProcesses("."))
                    {
                        try
                        {
                            if (p.ProcessName.ToString().Equals("chromedriver") || p.ProcessName.ToString().Equals("chrome"))
                                p.Kill();
                        }
                        catch { }
                    }
                    break;

                case BrowserType.Firefox:
                    foreach (Process p in Process.GetProcesses("."))
                    {
                        try
                        {
                            if (p.ProcessName.ToString().Equals("geckodriver") || p.ProcessName.ToString().Equals("firefox"))
                                p.Kill();
                        }
                        catch { }
                    }
                    break;

                case BrowserType.Edge:
                    foreach (Process p in Process.GetProcesses("."))
                    {
                        try
                        {
                            if (p.ProcessName.ToString().Equals("msedgedriver") || p.ProcessName.ToString().Equals("msedge"))
                                p.Kill();
                        }
                        catch { }
                    }
                    break;

                case BrowserType.IE:
                    // Who the hell uses IE these days?!?
                    foreach (Process p in Process.GetProcesses("."))
                    {
                        try
                        {
                            if (p.ProcessName.ToString().Contains("IEDriverServer") || p.ProcessName.ToString().Equals("iexplore"))
                                p.Kill();
                        }
                        catch { }
                    }
                    break;

                default:
                    throw new NotImplementedException(string.Format("Driver {0} is not supported", browser));
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

        public static void PerformActionWithRetry(this IWebDriver driver, TimeSpan timeout, int attempts, Func<IWebDriver, bool> action)
        {
            DateTime endTime = DateTime.Now.Add(timeout);
            int iterations = 0;

            while (DateTime.Now < endTime && iterations < attempts)
            {
                try
                {
                    if (action(driver))
                    {
                        return; // Action succeeded, exit the loop
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
        }


        #endregion

        #region [Private Functions]
        private static void ExecuteShellCommand(string Command)
        {  // Windows Shell Command
            ProcessStartInfo ProcessInfo;

            ProcessInfo = new ProcessStartInfo("cmd.exe", "/C " + Command);
            ProcessInfo.CreateNoWindow = true;
            ProcessInfo.UseShellExecute = true;
            ProcessInfo.WindowStyle = ProcessWindowStyle.Hidden;

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

        private static IWebDriver getDriver(BrowserType browser, string[] browserArguments = null)
        {
            IWebDriver driver;

            switch (browser)
            {
                case BrowserType.Chrome:
                    if (browserArguments != null)
                    {
                        var options = new ChromeOptions();
                        options.AddArguments(browserArguments);
                        driver = new ChromeDriver(options);
                    }
                    else
                        driver = WebDriverFactory.CreateChromeDriver();
                    break;

                case BrowserType.Firefox:
                    if (browserArguments != null)
                    {
                        var options = new FirefoxOptions();
                        options.AddArguments(browserArguments);
                        driver = new FirefoxDriver(options);
                    }
                    else
                        driver = new FirefoxDriver();
                    break;

                case BrowserType.Edge:
                    if (browserArguments != null)
                    {
                        var options = new EdgeOptions();
                        options.AddArguments(browserArguments);
                        driver = new EdgeDriver(options);
                    }
                    else
                        driver = new EdgeDriver();
                    break;

                case BrowserType.IE:
                    if (browserArguments != null)
                    {
                        var options = new InternetExplorerOptions();

                        for (int i = 0; i < browserArguments.Length; i += 2)
                        {
                            options.AddAdditionalInternetExplorerOption(browserArguments[i], browserArguments[i + 1]);
                        }

                        driver = new InternetExplorerDriver(options);
                    }
                    else
                        driver = new InternetExplorerDriver();
                    break;

                default:
                    throw new NotImplementedException(string.Format("Driver {0} is not supported", browser));
            }

            return driver;
        }

        private static StringBuilder TrimEnd(this StringBuilder sb)
        {
            if (sb == null || sb.Length == 0) return sb;

            int i = sb.Length - 1;

            for (; i >= 0; i--)
                if (!char.IsWhiteSpace(sb[i]))
                    break;

            if (i < sb.Length - 1)
                sb.Length = i + 1;

            return sb;
        }

        private static StringBuilder TrimBeginning(this StringBuilder sb)
        {
            if (sb == null || sb.Length == 0) return sb;

            int i = 0;

            for (; i <= (sb.Length - 1); i++)
                if (!char.IsWhiteSpace(sb[i]))
                    break;

            if (i > 0)
                sb.Remove(sb.Length - i, i);

            return sb;
        }

        private static StringBuilder Trim(this StringBuilder sb)
        {
            if (sb == null || sb.Length == 0) return sb;

            // leading whitespaces
            int i = sb.Length - 1;
            for (; i >= 0; i--)
            {
                if (!char.IsWhiteSpace(sb[i]))
                    break;
            }
            if (i < sb.Length - 1) sb.Length = i + 1;

            // trailing whitespaces
            i = 0;
            for (; i <= (sb.Length - 1); i++)
            {
                if (!char.IsWhiteSpace(sb[i]))
                    break;
            }
            if (i > 0) sb.Remove(sb.Length - i, i);

            return sb;
        }
        #endregion
    }
}
