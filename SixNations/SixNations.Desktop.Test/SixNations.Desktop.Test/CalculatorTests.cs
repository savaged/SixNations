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
    [TestClass]
    public class CalculatorTests
    {
        private const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723";
        private const string CalculatorAppId = "Microsoft.WindowsCalculator_8wekyb3d8bbwe!App";

        protected static WindowsDriver<WindowsElement> session;
        private static WindowsElement header;
        private static WindowsElement calculatorResult;

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            // Launch Calculator application if it is not yet launched
            if (session == null)
            {
                // Create a new session to bring up an instance of the Calculator application
                // Note: Multiple calculator windows (instances) share the same process Id
                DesiredCapabilities appCapabilities = new DesiredCapabilities();
                appCapabilities.SetCapability("app", CalculatorAppId);
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
            calculatorResult = session.FindElementByAccessibilityId("CalculatorResults");
            Assert.IsNotNull(calculatorResult);
        }

        [ClassCleanup]
        public static void CleanupClass()
        {
            // Close the application and delete the session
            if (session != null)
            {
                session.Quit();
                session = null;
            }
        }


        [TestInitialize()]
        public void Initialize()
        {
            session.FindElementByName("Clear").Click();
            Assert.AreEqual("0", GetCalculatorResultText());
        }

        [TestCleanup]
        public void CleanupTest()
        {

        }

        [TestMethod]
        public void Addition()
        {
            // Find the buttons by their names and click them in sequence to peform 1 + 7 = 8
            session.FindElementByName("One").Click();
            session.FindElementByName("Plus").Click();
            session.FindElementByName("Seven").Click();
            session.FindElementByName("Equals").Click();
            Assert.AreEqual("8", GetCalculatorResultText());
        }

        [TestMethod]
        public void Division()
        {
            // Find the buttons by their accessibility ids and click them in sequence to perform 88 / 11 = 8
            session.FindElementByAccessibilityId("num8Button").Click();
            session.FindElementByAccessibilityId("num8Button").Click();
            session.FindElementByAccessibilityId("divideButton").Click();
            session.FindElementByAccessibilityId("num1Button").Click();
            session.FindElementByAccessibilityId("num1Button").Click();
            session.FindElementByAccessibilityId("equalButton").Click();
            Assert.AreEqual("8", GetCalculatorResultText());
        }

        [TestMethod]
        public void Multiplication()
        {
            // Find the buttons by their names using XPath and click them in sequence to perform 9 x 9 = 81
            session.FindElementByXPath("//Button[@Name='Nine']").Click();
            session.FindElementByXPath("//Button[@Name='Multiply by']").Click();
            session.FindElementByXPath("//Button[@Name='Nine']").Click();
            session.FindElementByXPath("//Button[@Name='Equals']").Click();
            Assert.AreEqual("81", GetCalculatorResultText());
        }

        [TestMethod]
        public void Subtraction()
        {
            // Find the buttons by their accessibility ids using XPath and click them in sequence to perform 9 - 1 = 8
            session.FindElementByXPath("//Button[@AutomationId=\"num9Button\"]").Click();
            session.FindElementByXPath("//Button[@AutomationId=\"minusButton\"]").Click();
            session.FindElementByXPath("//Button[@AutomationId=\"num1Button\"]").Click();
            session.FindElementByXPath("//Button[@AutomationId=\"equalButton\"]").Click();
            Assert.AreEqual("8", GetCalculatorResultText());
        }

        [TestMethod]
        [DataRow("One", "Plus", "Seven", "8")]
        [DataRow("Nine", "Minus", "One", "8")]
        [DataRow("Eight", "Divide by", "Eight", "1")]
        public void Templatized(string input1, string operation, string input2, string expectedResult)
        {
            // Run sequence of button presses specified above and validate the results
            session.FindElementByName(input1).Click();
            session.FindElementByName(operation).Click();
            session.FindElementByName(input2).Click();
            session.FindElementByName("Equals").Click();
            Assert.AreEqual(expectedResult, GetCalculatorResultText());
        }

        private string GetCalculatorResultText()
        {
            return calculatorResult.Text.Replace("Display is", string.Empty).Trim();
        }
    }
}
