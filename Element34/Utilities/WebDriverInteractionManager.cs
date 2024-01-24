using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Element34.Utilities
{
    class WebDriverInteractionManager
    {
        public void PerformActionWithRetry(IWebDriver driver, TimeSpan timeout, int attempts, Func<IWebDriver, bool> action)
        {
            var wait = new WebDriverWait(driver, timeout);
            wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
            int iterations = 0;

            while (iterations < attempts)
            {
                wait.Until((d) =>
                {
                    try
                    {
                        return action(d);
                    }
                    catch (StaleElementReferenceException)
                    {
                        iterations++;
                        return false;
                    }
                });
            }
        }
    }
}
