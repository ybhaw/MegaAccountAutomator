using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using AngleSharp.Dom;
using BrowseSharp;
using EO.Internal;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace AccountAutomator
{
    class BrowseManager
    {
        private readonly IWebDriver _driver;
        private const string Link = "https://www.minuteinbox.com/";
        private readonly int Wait = 30;
        public BrowseManager(IWebDriver driver, Configuration config)
        {
            this._driver = driver;
            Wait = config.Wait;
        }

        public BrowseManager(Configuration config)
        {
            var options = new FirefoxOptions();
            options.AddArgument("-headless");
            options.LogLevel = FirefoxDriverLogLevel.Fatal;
            this._driver = new FirefoxDriver(options);
            Wait = config.Wait;
        }

        public static void ExtractEmails(string inFilePath, string outFilePath)
        {
            string data = File.ReadAllText(inFilePath); //read File 
            Regex emailRegex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*",
                RegexOptions.IgnoreCase);
            MatchCollection emailMatches = emailRegex.Matches(data);
            StringBuilder sb = new StringBuilder();
            foreach (Match emailMatch in emailMatches)
                sb.AppendLine(emailMatch.Value);
            File.WriteAllText(outFilePath, sb.ToString());
        }

        public static string ExtractLink(string txt)
        {
            
            foreach (Match item in Regex.Matches(txt, 
                @"(http|ftp|https):\/\/([\w\-_]+(?:(?:\.[\w\-_]+)+))([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?"))
            {
                return item.Value;
            }

            return "";
        }
        
        private void RemoveHiddenClass()
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor) _driver;
            js.ExecuteScript("$(\".hidden-xs, .hidden-sm, .hidden-md, .hidden-lg\").removeClass(\"hidden-md\").removeClass(\"hidden-lg\").removeClass(\"hidden-sm\").removeClass(\"hidden-xs\")");
        }
        
        public IWebElement FindElement(By by, int timeoutInSeconds = 500)
        {
            if (timeoutInSeconds > 0)
            {
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(timeoutInSeconds));
                return wait.Until(drv => drv.FindElement(by));
            }
            return _driver.FindElement(by);
        }
        ~BrowseManager()
        {
            // _driver.Close();
        }
        public string GetEmail()
        {
            _driver.Navigate().GoToUrl(Link);
            return _driver.FindElement(By.Id("email")).Text;
        }

        public string GetConfirmation()
        {
            Console.WriteLine($"Waiting for {Wait} seconds...");
            Thread.Sleep(Wait * 1000);
            RemoveHiddenClass();
            var element = _driver.FindElement(By.CssSelector("[data-href=\"2\"]"));
            IJavaScriptExecutor js = (IJavaScriptExecutor) _driver;
            js.ExecuteScript("arguments[0].click();", element);
            Thread.Sleep(Wait*500);
            var frame = _driver.SwitchTo().Frame(_driver.FindElement(By.Id("iframeMail")));
            var text = frame.FindElement(By.CssSelector("body")).Text;
            return ExtractLink(text);
        }

        public void Close()
        {
            _driver.Close();
        }
    }
}