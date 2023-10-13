using OpenQA.Selenium;
using System;
using System.Collections.Generic;

namespace Element34
{
    public class TabManager<TWebDriver> : IDisposable where TWebDriver : IWebDriver
    {
        private TWebDriver driver;
        protected bool disposed;

        public TabManager(TWebDriver driver)
        {
            this.driver = driver;
        }

        public TabManager()
        {
            driver = Activator.CreateInstance<TWebDriver>();
        }

        public void OpenNewTab(string url)
        {
            driver.SwitchTo().NewWindow(WindowType.Tab);
            SwitchToLastTab();
            driver.Navigate().GoToUrl(url);
        }

        public void OpenNewTab()
        {
            driver.SwitchTo().NewWindow(WindowType.Tab);
            SwitchToLastTab();
        }

        public void SwitchToTab(int tabIndex)
        {
            if (tabIndex >= 0 && tabIndex < driver.WindowHandles.Count)
            {
                driver.SwitchTo().Window(driver.WindowHandles[tabIndex]);
            }
            else
            {
                throw new ArgumentOutOfRangeException("Invalid tab index.");
            }
        }

        public void SwitchToLastTab()
        {
            SwitchToTab(driver.WindowHandles.Count - 1);
        }

        public void SwitchToFirstTab()
        {
            SwitchToTab(0);
        }

        public void CloseCurrentTab()
        {
            if (driver.WindowHandles.Count > 1)
            {
                driver.Close();
                SwitchToLastTab();
            }
            else
            {
                throw new InvalidOperationException("Cannot close the last tab.");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                if (driver != null)
                {
                    driver.Quit();
                    driver = default(TWebDriver);
                }
            }

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
