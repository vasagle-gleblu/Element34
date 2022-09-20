using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using NLog;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Chromium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace Element34.Utilities
{
    public static class AdjunctFunctions
    {
        private static bool _acceptNextAlert = true;
        private static readonly int _defaultTimeSpan = 30;
        private static Logger _logger;
        public static readonly int timeDelay = 3000;

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
            IJavaScriptExecutor js = (IJavaScriptExecutor)element;
            js.ExecuteScript(script, args);
        }

        public static T ExecuteJavaScript<T>(this IWebElement element, string script, params object[] args)
        {  // Execute client-side JavaScript on IWebElement with return type
            IJavaScriptExecutor js = (IJavaScriptExecutor)element;
            return (T)js.ExecuteScript(script, args);
        }

        public static void ExecuteJavaScript(this SelectElement selectElement, string script, params object[] args)
        {  // Execute client-side JavaScript on Select Element
            IJavaScriptExecutor js = (IJavaScriptExecutor)selectElement;
            js.ExecuteScript(script, args);
        }

        public static T ExecuteJavaScript<T>(this SelectElement selectElement, string script, params object[] args)
        {  // Execute client-side JavaScript on Select Element with return type
            IJavaScriptExecutor js = (IJavaScriptExecutor)selectElement;
            return (T)js.ExecuteScript(script, args);
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
                    act.Perform();
                    flag = true;
                }
                catch (ElementNotInteractableException)
                {
                    var e = driver.FindElement(By.XPath(".."));
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
                    act.Perform();
                    flag = true;
                }
                catch (ElementNotInteractableException)
                {
                    var e = driver.FindElement(By.XPath(".."));
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

        public static Dictionary<string, string> GetAttributes(this IWebElement element)
        {
            const string script = "var items = {};" +
                                  "for (index = 0; index < arguments[0].attributes.length; ++index) " +
                                  "{" +
                                  "     items[arguments[0].attributes[index].name] = arguments[0].attributes[index].value" +
                                  "}" +
                                  "return items;";

            return element.ExecuteJavaScript<Dictionary<string, string>>(script, element);
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
            IWebElement element = driver.FindElement(locator);

            if (element == null)
                throw new ArgumentNullException("element");
            else
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView();", element);
        }

        public static void ScrollToElement(this IWebDriver driver, IWebElement element)
        {  // Scroll to element (JavaScript call)
            if (element == null)
                throw new ArgumentNullException("element");
            else
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView();", element);
        }

        public static void Type(this IWebDriver driver, By locator, string sInput)
        {  // Experimental: Method extension of Type() for WebDriver
            IWebElement webElement = driver.FindElement(locator);
            webElement.Type(sInput);
        }

        public static void Type(this IWebElement webElement, string sInput)
        {  // Experimental: Consolidated steps for typing in a string value
            if (webElement == null)
                throw new ArgumentNullException("webElement");
            else if (webElement.TagName != "input")
                throw new ArgumentException("tag name");

            string type = webElement.GetAttribute("type");
            if (!compareAnyStr(type.ToLower(), new string[] { "text", "password", "email", "date", "datetime-local", "month", "number", "search", "tel", "time", "url", "week" }))
                throw new ArgumentException("type attribute");

            if (!string.IsNullOrEmpty(sInput))
            {
                Click(webElement);
                webElement.Clear();
                webElement.SendKeys(sInput);
            }
        }

        public static void Click(this IWebDriver driver, By locator)
        {  // Experimental: Method extension of Click for WebDriver
           // DON'T PUT THIS IN A WHILE LOOP!!
            IWebElement webElement = driver.FindElement(locator);
            Click(webElement);
        }

        public static void Click(IWebElement webElement)
        {  // Experimental: If element is not clickable, move up to parent element and try again.
           // DON'T PUT THIS IN A WHILE LOOP!! 
            try
            {
                webElement.Click();
            }
            catch (ElementClickInterceptedException)
            {
                webElement.FindElement(By.XPath("..")).Click();
            }
        }

        public static void Check(this IWebDriver driver, By locator, string sInput)
        {  // Method extension of Check for WebDriver
            IWebElement radioBox = driver.FindElement(locator);
            radioBox.Check(sInput);
        }

        public static void Check(this IWebElement radioBox, string sInput)
        {  // Ensure checkbox or radio button is what the specified value of sInput is regardless of initial state
            if (radioBox == null)
                throw new ArgumentNullException("radioBox");
            else if (radioBox.TagName != "input")
                throw new ArgumentException("tag name");

            string type = radioBox.GetAttribute("type");
            if (!compareAnyStr(type.ToLower(), new string[] { "radio", "checkbox" }))
                throw new ArgumentException("type attribute");

            IJavaScriptExecutor js = (IJavaScriptExecutor)radioBox;
            js.ExecuteScript("arguments[0].focus();", radioBox);

            bool blnTest = radioBox.Selected;

            if (isPositiveResponse(sInput))
            {
                if (!blnTest)
                {
                    radioBox.Click();
                }
            }
            else if (isNegativeResponse(sInput))
            {
                if (blnTest)
                {
                    radioBox.Click();
                }
            }
        }

        public static bool IsElementPresent(this IWebDriver driver, By locator)
        {  // Generated by Katalon Recorder
            try
            {
                driver.FindElement(locator);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public static bool IsAlertPresent(this IWebDriver driver)
        {  // Generated by Katalon Recorder
            try
            {
                driver.SwitchTo().Alert();
                return true;
            }
            catch (NoAlertPresentException)
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

            driver.Manage().Timeouts().ImplicitWait = (TimeSpan)timeOut;
            driver.Navigate().GoToUrl(URL);
            driver.Manage().Window.Maximize();
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
                    ShadowRoot shadowRoot1 = driver.getShadowRootElement(By.CssSelector("settings-ui"));
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
                    IWebElement ddnTimeRange = shadowRoot7.FindElement(By.CssSelector("select#dropdownMenu"));
                    new SelectElement(ddnTimeRange).SelectByValue("4");

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
                    break;

                case nameof(InternetExplorerDriver):
                    break;

                default:
                    throw new NotImplementedException(string.Format("Driver {0} is not supported", driver.GetType().Name));
            }
        }

        public static IWebElement FindElement(this IWebDriver driver, By locator, TimeSpan? timeOut = null)
        {  //FindElement() with timeout as TimeSpan
            if (locator == null)
            {
                throw new ArgumentNullException("locator", "locator cannot be null");
            }

            timeOut = (timeOut == null) ? TimeSpan.FromSeconds(_defaultTimeSpan) : timeOut.Value;     // default for timeOut
            var task = Task.Run(() => locator.FindElement(driver));
            if (task.Wait((TimeSpan)timeOut))
                return task.Result;
            else
                throw new Exception("Timed out");
        }

        public static ReadOnlyCollection<IWebElement> FindElements(this IWebDriver driver, By locator, TimeSpan? timeOut = null)
        {  //FindElements() with timeout as TimeSpan
            if (locator == null)
            {
                throw new ArgumentNullException("by", "by cannot be null");
            }

            timeOut = (timeOut == null) ? TimeSpan.FromSeconds(60) : timeOut.Value;     // default for timeOut
            var task = Task.Run(() => locator.FindElements(driver));
            if (task.Wait((TimeSpan)timeOut))
                return task.Result;
            else
                throw new Exception("Timed out");
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

        public static bool HasChildren(this IWebDriver driver, IWebElement element)
        {  // Determine if element has **ANY** children at that moment
            bool hasChildren;

            if (element == null)
                throw new ArgumentNullException("element");

            //Save implicit timeout to reset it later 
            TimeSpan tmp = driver.Manage().Timeouts().ImplicitWait;

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;

            try
            { hasChildren = element.FindElements(By.XPath(".//*")).Count > 0; }
            catch
            { hasChildren = false; }

            driver.Manage().Timeouts().ImplicitWait = tmp;

            return hasChildren;
        }

        public static ReadOnlyCollection<IWebElement> getTableRows(this IWebDriver driver, IWebElement table)
        {  // Return read-only collection of table rows
            if (table == null)
                throw new ArgumentNullException("table");
            else if (table.TagName != "table")
                throw new ArgumentException("tag name");

            ReadOnlyCollection<IWebElement> rows = table.FindElements(By.TagName("tr"));
            return rows;
        }

        public static ReadOnlyCollection<string> getRow(IWebElement table, int rowNumber)
        {
            if (table == null)
                throw new ArgumentNullException("table");
            else if (table.TagName != "table")
                throw new ArgumentException("tag name");

            var driver = ((IWrapsDriver)table).WrappedDriver;
            ReadOnlyCollection<IWebElement> row = driver.getTableRows(table)[rowNumber].FindElements(By.TagName("td"));
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
                throw new ArgumentNullException("table");
            else if (table.TagName != "table")
                throw new ArgumentException("tag name");

            List<string> result = new List<string>();
            var driver = ((IWrapsDriver)table).WrappedDriver;
            foreach (var row in driver.getTableRows(table))
            {
                result.Add(row.FindElements(By.TagName("td"))[colNumber].Text);
            }

            return new ReadOnlyCollection<string>(result);
        }

        public static void TakeScreenshot(this IWebDriver driver, string sPath, ScreenshotImageFormat format = ScreenshotImageFormat.Jpeg)
        {  // Take snapshot of current web browser screen and save to specified file
            Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();
            ss.SaveAsFile(sPath, format);
        }

        public static bool isPositiveResponse(string sInput)
        {
            /*
                Is Positive Response?
                test input for *any* response considered positive

                baseTest; base object for extension
                sInput; input string
            */

            return compareAny(sInput.ToLower(), new string[] { "y", "yes", "t", "true", "1", "+", "affirmative", "positive" });
        }

        public static bool isNegativeResponse(string sInput)
        {
            /*
                Is Negative Response?
                test input for *any* response considered negative

                baseTest; base object for extension
                sInput; input string
            */

            return compareAny(sInput.ToLower(), new string[] { "n", "no", "f", "false", "0", "-", "negative" });
        }

        public static bool compareAnyStr(string sInput, string[] testArray)
        {
            /*
                test input for *any* response contained in testArray
                public facing function exposing comparAny() function

                baseTest; base object for extension
                sInput; input string
                testArray; string array for comparison with input
            */

            return compareAny(sInput, testArray);
        }

        public static int findRow(IWebElement table, List<string> criteria, bool blnAllTrue = true, bool blnExactMatch = false)
        {
            /*
                findRow: extension for HtmlTable and support function for all Grid Search functions.
                Returns the index of the first row that matches given criteria (0 is returned if not found).
                Subtract 1 to use in zero-based array.

                table: base object for extension
                criteria: List of string of search criteria
                blnAllTrue: all criteria must match if true, any one of criteria can match if false
                blnExactMatch: text comparison method (Equals if true, Contains if false)

                FUNCTION IS VERY SLOW!!!
            */
            int iRowFound = 0; int iRowIndex;
            bool blnCellMatch, blnPerCriterion;
            bool? blnRowTest;
            string pattern, cellContents, decodedContents;
            var driver = ((IWrapsDriver)table).WrappedDriver;
            ReadOnlyCollection<IWebElement> tableRows = driver.getTableRows(table);
            IWebElement row;

            // Check all rows
            for (iRowIndex = 0; iRowIndex < tableRows.Count; iRowIndex++)
            {
                row = tableRows[iRowIndex];
                blnRowTest = null;
                // Check all criteria against cell contents
                foreach (string criterion in criteria)
                {
                    blnPerCriterion = false;
                    if (!string.IsNullOrEmpty(criterion))
                    {
                        pattern = criterion.ToLower().Trim();
                        ReadOnlyCollection<IWebElement> rowCells = row.FindElements(By.TagName("td"));
                        // Check all cells
                        foreach (IWebElement cell in rowCells)
                        {
                            // Compare pattern to cell contents
                            cellContents = NormalizeWhiteSpace(cell.Text.ToLower().Trim());
                            decodedContents = decodeString(cellContents, 'L');

                            if (blnExactMatch)
                            {
                                if (cellContents.Length == pattern.Length)
                                {
                                    blnCellMatch = decodedContents.Equals(pattern) || cellContents.ToLower().Equals(pattern);
                                }
                                else
                                    blnCellMatch = false;
                            }
                            else
                                blnCellMatch = decodedContents.Contains(pattern) || cellContents.ToLower().Contains(pattern);

                            // Criterion search successful
                            if (blnCellMatch)
                            {
                                blnPerCriterion = true;
                                break;
                            }
                        }

                        // Row test (null for first cell)
                        // All true (i.e. condition1 AND condition2 AND condition3 ...)
                        // Else     (i.e. condition1 OR condition2 OR condition3 ...)
                        if (blnRowTest != null)
                            if (blnAllTrue)
                                blnRowTest &= blnPerCriterion;
                            else
                                blnRowTest |= blnPerCriterion;
                        else
                            blnRowTest = blnPerCriterion;
                    }
                }

                if (blnRowTest != null)
                {
                    if (blnRowTest == true)
                    {
                        iRowFound = (iRowIndex + 1);
                        break;
                    }
                }
            }

            return iRowFound;
        }

        public static void CloseProcesses(string driver)
        {  // Forcefully kill off old test processes from previous iteration(s)
            switch (driver)
            {
                case "ChromeDriver":
                case "ChromiumDriver":
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

                case "FirefoxDriver":
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

                case "EdgeDriver":
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

                case "InternetExplorerDriver":
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
                    throw new NotImplementedException(string.Format("Driver {0} is not supported", driver));
            }
        }
        #endregion

        #region [Private Functions]
        private static void ExecuteShellCommand(string Command)
        {  // Windows Shell Command
            Process p = new Process();
            ProcessStartInfo processInfo;

            processInfo = new ProcessStartInfo("cmd.exe", "/C " + Command);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = true;
            processInfo.WindowStyle = ProcessWindowStyle.Hidden;

            p.StartInfo = processInfo;
            p.Start();
        }

        private static string decodeString(string sInput, char instance = ' ')
        {
            /*
             decodeString: decode input using HttpUtility.HtmlDecode()

             sInput: input string under examination.
             instance: to indicate if the output should be in upper (with u or U) or lower (with l or L) case
             or neither.
            */

            string sOutput;
            sInput = sInput.Trim();

            switch (instance)
            {
                case 'u':
                case 'U':
                    sOutput = HttpUtility.HtmlDecode(sInput).ToUpper();
                    break;

                case 'l':
                case 'L':
                    sOutput = HttpUtility.HtmlDecode(sInput).ToLower();
                    break;

                default:
                    sOutput = HttpUtility.HtmlDecode(sInput);
                    break;
            }

            return sOutput;
        }

        private static string NormalizeWhiteSpace(string sInput, char normalizeTo = ' ')
        {
            /*
             NormalizeWhiteSpace: To replace all characters considered to be white spaces character
             with a new character.

             sInput: input string under examination.
             normalizeTo: replacement character for white spaces (default: a normal space, i.e. ' ').
            */

            if (string.IsNullOrEmpty(sInput))
            {
                return string.Empty;
            }

            StringBuilder output = new StringBuilder();
            bool skipped = false;

            foreach (char c in sInput)
            {
                if (char.IsWhiteSpace(c))
                {
                    if (!skipped)
                    {
                        output.Append(normalizeTo);
                        skipped = true;
                    }
                }
                else
                {
                    skipped = false;
                    output.Append(c);
                }
            }

            return output.ToString();
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

        private static bool compareAny(string sInput, string[] testArray)
        {
            /* compareAny: compare input string with all values in testArray (true if *any* elements match up)

               sInput: input string under examination.
               testArray: string array with test values
            */
            return testArray.Any(testElement => testElement == sInput);
        }

        private static DriverOptions CreateDriverOptions(this IWebDriver driver, string[] browserArguments = null)
        {
            switch (driver.GetType().Name)
            {
                case nameof(ChromiumDriver):
                case nameof(ChromeDriver):
                    var ChromeOptions = new ChromeOptions();

                    if (browserArguments != null)
                        ChromeOptions.AddArguments(browserArguments);

                    ChromeOptions.Proxy = new Proxy()
                    {
                        IsAutoDetect = false,
                        Kind = ProxyKind.Direct
                    };

                    return ChromeOptions;

                case nameof(FirefoxDriver):
                    var FirefoxOptions = new FirefoxOptions();

                    if (browserArguments != null)
                        FirefoxOptions.AddArguments(browserArguments);

                    FirefoxOptions.Proxy = new Proxy()
                    {
                        IsAutoDetect = false,
                        Kind = ProxyKind.Direct
                    };

                    FirefoxOptions.Profile = new FirefoxProfile() { DeleteAfterUse = true };

                    return FirefoxOptions;

                case nameof(EdgeDriver):
                    var EdgeOptions = new EdgeOptions();

                    if (browserArguments != null)
                        EdgeOptions.AddArguments(browserArguments);

                    EdgeOptions.Proxy = new Proxy()
                    {
                        IsAutoDetect = false,
                        Kind = ProxyKind.Direct
                    };

                    return EdgeOptions;

                case nameof(InternetExplorerDriver):
                    var InternetExplorerOptions = new InternetExplorerOptions();

                    if (browserArguments != null)
                    {
                        for (int i = 0; i < browserArguments.Length; i += 2)
                        {
                            InternetExplorerOptions.AddAdditionalInternetExplorerOption(browserArguments[i], browserArguments[i + 1]);
                        }
                    }

                    InternetExplorerOptions.Proxy = new Proxy()
                    {
                        IsAutoDetect = false,
                        Kind = ProxyKind.Direct
                    };

                    return InternetExplorerOptions;

                default:
                    throw new NotImplementedException(string.Format("Driver {0} is not supported", driver.GetType().Name));
            }
        }
        #endregion
    }
}
