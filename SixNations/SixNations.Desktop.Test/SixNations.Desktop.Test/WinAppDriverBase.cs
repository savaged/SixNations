using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Remote;

namespace SixNations.Desktop.Test
{
    /// <summary>
    /// Based on sample from https://github.com/Microsoft/WinAppDriver
    /// Remember to run the exe (probably at C:\Program Files (x86)\Windows Application Driver)
    /// </summary>
    public class WinAppDriverBase
    {
        private const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723";
        private const string AutomationAppId = "Microsoft.WindowsCalculator_8wekyb3d8bbwe!App";

        protected static WindowsDriver<WindowsElement> session;
        private static WindowsElement header;
        private static WindowsElement result;

        static WinAppDriverBase()
        {
            // Launch Calculator application if it is not yet launched
            if (session == null)
            {
                // Create a new session to bring up an instance of the Calculator application
                // Note: Multiple calculator windows (instances) share the same process Id
                DesiredCapabilities appCapabilities = new DesiredCapabilities();
                appCapabilities.SetCapability("app", AutomationAppId);
                appCapabilities.SetCapability("deviceName", "WindowsPC");
                session = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), appCapabilities);
                Assert.IsNotNull(session);

                // Set implicit timeout to 1.5 seconds to make element search to retry every 500 ms for at most three times
                session.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(1.5));
            }

            try
            {
                header = session.FindElementByAccessibilityId("Header");
            }
            catch
            {
                header = session.FindElementByAccessibilityId("ContentPresenter");
            }

            // Ensure that calculator is in standard mode
            if (!header.Text.Equals("Standard", StringComparison.OrdinalIgnoreCase))
            {
                session.FindElementByAccessibilityId("NavButton").Click();
                Thread.Sleep(TimeSpan.FromSeconds(1));
                var splitViewPane = session.FindElementByClassName("SplitViewPane");
                splitViewPane.FindElementByName("Standard Calculator").Click();
                Thread.Sleep(TimeSpan.FromSeconds(1));
                Assert.IsTrue(header.Text.Equals("Standard", StringComparison.OrdinalIgnoreCase));
            }

            // Locate the calculatorResult element
            result = session.FindElementByAccessibilityId("CalculatorResults");
            Assert.IsNotNull(result);
        }

        ~WinAppDriverBase()
        {
            // Close the application and delete the session
            if (session != null)
            {
                session.Quit();
                session = null;
            }
        }

        protected string GetCalculatorResultText()
        {
            return result.Text.Replace("Display is", string.Empty).Trim();
        }
    }
}
