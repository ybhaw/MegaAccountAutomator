using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EO.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium.Chrome;
using RestSharp.Serialization.Json;

namespace AccountAutomator
{
    static class Program
    {
        static Configuration CreateConfig()
        {
            Configuration config = new Configuration();
            using (StreamReader file = new StreamReader("../../../config.json"))
            {
                var t = file.ReadToEnd();
                var c = JObject.Parse(t);
                config.Confirm = Convert.ToBoolean(c["confirm"]);
                config.MegaDir = Convert.ToString(c["mega"]);
                config.Wait = Convert.ToInt32(c["wait-time"]);
                config.Password = new Password
                {
                    AutoGenerate = Convert.ToBoolean(c["password"]["auto-generate"]), 
                    Default =  Convert.ToString(c["password"]["default"])
                };
                config.Name = new Name
                {
                    Keep = Convert.ToBoolean(c["name"]["keep-name"]),
                    AutoGenerate = Convert.ToBoolean(c["name"]["auto-generate"]),
                    Default = Convert.ToString(c["name"]["default"])
                };
            }

            return config;
        }
        static void Main(string[] args)
        {
            var config = CreateConfig();
            Console.WriteLine("Mega Account Creator : https://github.com/ybhaw/MegaAccountAutomator");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Starting browser...");
            var browser = new BrowseManager(config);
            Console.WriteLine("Generating Email...");
            var email = browser.GetEmail();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"Email generated: {email}");
            Console.ForegroundColor = ConsoleColor.Green;
            var mega = new MegaManager(config.MegaDir);
            Console.WriteLine("Attempting to create mega account");
            var password = mega.CreateAccount(email, config);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Account successfully created!");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Attemping to confirm account now...");
            mega.ConfirmAccount(email, password, browser.GetConfirmation());
            Console.WriteLine("Writing details to file now...");
            using (StreamWriter file = new StreamWriter("../../../logins.txt"))
            {
                file.WriteLine($"{email} {password}");
            }
            browser.Close();
        }
    }
}