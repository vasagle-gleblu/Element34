using System;
using System.Text;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.Events;
using OpenQA.Selenium.Support.UI;


namespace SeleniumTest
{
    [TestFixture]
    public class Login
    {
        private IWebDriver baseDriver;
        private EventFiringWebDriver driver;
        private string baseURL;

        [SetUp]
        public void SetupTest()
        {
            baseDriver = new ChromeDriver();
            driver = new EventFiringWebDriver(baseDriver);
            baseURL = "https://www.SauceLabs.com";
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

        [Test(Description = "Check SauceLabs Homepage for Login Link")]
        public void theTest()
        {
            driver.Navigate().GoToUrl(baseURL);

            WebDriverWait wait = new WebDriverWait(driver, System.TimeSpan.FromSeconds(15));
            wait.Until(driver => driver.FindElement(By.XPath("//a[@href='/beta/login']")));

            IWebElement element = driver.FindElement(By.XPath("//a[@href='/beta/login']"));
            Assert.AreEqual("Sign In", element.GetAttribute("text"));

            driver.Close();
        }

    }
}

