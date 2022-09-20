using System;
using System.Data;
using System.IO;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Chrome;
using Adjunct = Element34.Utilities.AdjunctFunctions;
using DEL = Element34.Utilities.DataManager.DataExtractionLayer;
using SharpAvi;
using Element34.Utilities;
using Element34.Utilities.VideoRecorder;

namespace SeleniumTest
{
    [TestFixture]
    public class Test1
    {
        private IWebDriver driver;
        private DataTable TestData;
        private Actions action;
        private string dataPath;
        private DataRow Data;
        private string name;

        [SetUp]
        public void SetupTest()
        {
            name = "Test1";
            Adjunct.CloseProcesses("ChromeDriver");
            driver = new ChromeDriver();
            dataPath = GetFolder.searchUpForFolder("Data");
            action = new Actions(driver);
            TestData = DEL.DataTableFromCSV(Path.Combine(dataPath, name + ".csv"));
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

        [Test(Description = "Login demo")]
        public void theTest()
        {
            using (var rec = new Recorder(new RecorderParams(Path.Combine(dataPath, name + ".avi"), "out", 5, CodecIds.MotionJpeg, 50)))
            {
                for (int iteration = 0; iteration < TestData.Rows.Count; iteration++)
                {
                    Data = TestData.Rows[iteration];

                    // Step 1) Open URL
                    driver.OpenBrowser(Data["url"].ToString());

                    // Step 2) Wait for page to be rendered
                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
                    wait.Until(driver => driver.FindElement(By.XPath("//input[@name='username']")));
                    driver.wait_A_Moment(Adjunct.timeDelay);

                    // Step 3) Type in the username
                    IWebElement Username = driver.FindElement(By.XPath("//input[@name='username']"));
                    action.MoveToElement(Username).Perform();
                    Username.Click();
                    Username.Type(Data["username"].ToString());
                    driver.wait_A_Moment(Adjunct.timeDelay);

                    // Step 4) Type in the password
                    IWebElement Password = driver.FindElement(By.XPath("//input[@name='password']"));
                    action.MoveToElement(Password).Perform();
                    Password.Click();
                    Password.Type(Data["password"].ToString());
                    driver.wait_A_Moment(Adjunct.timeDelay);

                    // Step 5) Click on login button
                    driver.FindElement(By.XPath("//input[@type='submit']")).Click();
                    driver.wait_A_Moment(Adjunct.timeDelay);

                    // Assert the following page after login contains "example.com"
                    driver.TakeScreenshot(Path.Combine(dataPath, string.Format(name + " result_{0}.jpg", iteration)));
                    Assert.IsTrue(driver.Url.Contains("example.com"));
                    driver.wait_A_Moment(Adjunct.timeDelay);
                }
            }

            driver.Close();
        }
    }
}

