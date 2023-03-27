using System;
using System.Collections.Generic;
using NUnit.Framework.Internal;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Chrome;
using Element34;
using static Element34.WebDriverExtensions;


namespace SeleniumTest
{
    public class TableSearch
    {
        #region Fields
        private IWebDriver driver;
        private Actions action;
        private readonly string name = "Table Search";
        #endregion

        [SetUp]
        public void SetupTest()
        {
            CloseProcesses(browserType.Chrome);
            driver = new ChromeDriver();
            action = new Actions(driver);
        }

        [TearDown]
        public void TeardownTest()
        {
            try
            {
                driver.Quit();
            }
            catch (Exception)
            {
                // Ignore errors if unable to close the browser
            }
        }

        [Test(Description = "Search through paginated HTML table")]
        public void gyupo9Table()
        {
            // This web site ended up being a bad example as the names are randomly generated after each refresh!
            Gyupo9GridType grid = new Gyupo9GridType();
            Gyupo9GridType.NoneSelectionType noneSelectionType = new Gyupo9GridType.NoneSelectionType();

            driver.OpenBrowser("https://gyupo9.sse.codesandbox.io/");
            driver.wait_A_Moment(timeDelay);

            Dictionary<string, By> Locators = new Dictionary<string, By>
            {
                { "nextButtonLocator", By.XPath("//button[.='>']") },
                { "previousButtonLocator", By.XPath("//button[.='<']") },
                { "busySpinnerLocator", By.XPath("//div[@class='loader']") },
                { "gridContainerLocator", By.XPath("/html/body/div/div/table") },
                { "tableRowsLocator", By.XPath("//table/tbody/tr") }
            };

            List<string> criteria = new List<string>
            {
                "Antonietta",
                "Grady",
                "single"
            };

            bool blnResult = grid.GridSearch(driver, Locators, noneSelectionType, criteria, true);
            driver.wait_A_Moment(timeDelay);

            driver.Close();

            // Test Case test result
            Assert.IsTrue(blnResult);
        }

        [Test(Description = "Search through paginated HTML table")]
        public void mdbootstrapTable()
        {
            MdbootstrapGridType grid = new MdbootstrapGridType();
            MdbootstrapGridType.NoneSelectionType noneSelectionType = new MdbootstrapGridType.NoneSelectionType();

            driver.OpenBrowser("https://mdbootstrap.com/docs/b4/jquery/tables/pagination/");
            driver.wait_A_Moment(timeDelay);

            Dictionary<string, By> Locators = new Dictionary<string, By>
            {
                { "nextButtonLocator", By.XPath("//a[.='Next']") },
                { "previousButtonLocator", By.XPath("//a[.='Previous']") },
                { "busySpinnerLocator", By.XPath("//div[@class='loader']") },
                { "gridContainerLocator", By.XPath("//div[@id='dtBasicExample_wrapper']") },
                { "tableRowsLocator", By.XPath("//table/tbody/tr") }
            };

            List<string> criteria = new List<string>
            {
                "Prescott Bartlett",
                "Technical Author"
            };

            bool blnResult = grid.GridSearch(driver, Locators, noneSelectionType, criteria, true);
            driver.wait_A_Moment(timeDelay);

            driver.Close();

            // Test Case test result
            Assert.IsTrue(blnResult);
        }
    }
}

