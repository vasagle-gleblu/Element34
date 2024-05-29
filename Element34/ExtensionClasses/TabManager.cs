using OpenQA.Selenium;
using System;

namespace Element34
{
    /// <summary>
    /// Manages browser tabs using a specified WebDriver.
    /// </summary>
    /// <typeparam name="TWebDriver">The type of WebDriver to use, which must implement IWebDriver.</typeparam>
    public class TabManager<TWebDriver> : IDisposable where TWebDriver : IWebDriver
    {
        private TWebDriver driver;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="TabManager{TWebDriver}"/> class with the specified WebDriver.
        /// </summary>
        /// <param name="driver">The WebDriver instance to use.</param>
        public TabManager(TWebDriver driver)
        {
            if (driver == null)
                throw new ArgumentNullException(nameof(driver));

            this.driver = driver;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TabManager{TWebDriver}"/> class with a new WebDriver instance.
        /// </summary>
        public TabManager()
        {
            driver = Activator.CreateInstance<TWebDriver>();
        }

        /// <summary>
        /// Opens a new tab and navigates to the specified URL.
        /// </summary>
        /// <param name="url">The URL to navigate to.</param>
        public void OpenNewTab(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("URL cannot be null or empty.", nameof(url));

            driver.SwitchTo().NewWindow(WindowType.Tab);
            driver.Navigate().GoToUrl(url);
        }

        /// <summary>
        /// Opens a new tab.
        /// </summary>
        public void OpenNewTab()
        {
            driver.SwitchTo().NewWindow(WindowType.Tab);
        }

        /// <summary>
        /// Switches to the tab at the specified index.
        /// </summary>
        /// <param name="tabIndex">The zero-based index of the tab to switch to.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the tab index is out of range.</exception>
        public void SwitchToTab(int tabIndex)
        {
            if (tabIndex < 0 || tabIndex >= driver.WindowHandles.Count)
                throw new ArgumentOutOfRangeException(nameof(tabIndex), "Invalid tab index.");

            driver.SwitchTo().Window(driver.WindowHandles[tabIndex]);
        }

        /// <summary>
        /// Switches to the last tab.
        /// </summary>
        public void SwitchToLastTab()
        {
            driver.SwitchTo().Window(driver.WindowHandles[driver.WindowHandles.Count - 1]);
        }

        /// <summary>
        /// Switches to the first tab.
        /// </summary>
        public void SwitchToFirstTab()
        {
            driver.SwitchTo().Window(driver.WindowHandles[0]);
        }

        /// <summary>
        /// Closes the current tab. If it is the last tab, throws an exception.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when attempting to close the last remaining tab.</exception>
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

        /// <summary>
        /// Releases the resources used by the <see cref="TabManager{TWebDriver}"/> class.
        /// </summary>
        /// <param name="disposing">True if managed resources should be disposed; otherwise, false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (driver != null)
                    {
                        driver.Quit();
                        driver = default;
                    }
                }

                disposed = true;
            }
        }

        /// <summary>
        /// Releases all resources used by the <see cref="TabManager{TWebDriver}"/> class.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}