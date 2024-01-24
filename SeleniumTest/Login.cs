using Element34;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.Events;
using System;
using System.Threading;
using static Element34.ExtensionClasses.WebDriverExtensions;

namespace SeleniumTest
{
    [TestFixture]
    public class Login
    {
        private IWebDriver baseDriver;
        private EventFiringWebDriver driver;
        private string baseURL;
        private readonly int timeDelay = 2000;

        [SetUp]
        public void SetupTest()
        {
            CloseProcesses(BrowserType.Chrome);
            baseDriver = new ChromeDriver();
            driver = new EventFiringWebDriver(baseDriver);
            baseURL = "https://accounts.saucelabs.com/am/XUI/#login/";
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

        [Test(Description = "Check SauceLabs Homepage for Login page.")]
        public void theTest()
        {
            driver.Navigate().GoToUrl(baseURL);

            driver.Manage().Window.Maximize();

            Thread.Sleep(timeDelay);
            Thread.Yield();

            By locator = By.XPath("//*[@id='content']/div/div/section[1]/div/h3");
            IWebElement element = driver.FindElement(locator);

            Assert.That("Sign in" == element.Text);

            driver.Close();
        }

    }
}

