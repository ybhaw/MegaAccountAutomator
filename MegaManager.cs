using System;
using System.Diagnostics;
using EO.Internal;

namespace AccountAutomator
{
    public class MegaManager
    {
        private const string ProgramName = "MEGAClient.exe";
        private string Executable;

        public MegaManager(string directory)
        {
            this.Executable = directory + (directory.EndsWith("\\") ? ProgramName : "\\" + ProgramName);
        }

        public static string GeneratePassword()
        {
            return Guid.NewGuid().ToString("d").Substring(1,8);
        }
        public static string GenerateName(int len = 8)
        { 
            Random r = new Random();
            string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x" };
            string[] vowels = { "a", "e", "i", "o", "u", "ae", "y" };
            string Name = "";
            Name += consonants[r.Next(consonants.Length)].ToUpper();
            Name += vowels[r.Next(vowels.Length)];
            int b = 2; 
            while (b < len)
            {
                Name += consonants[r.Next(consonants.Length)];
                b++;
                Name += vowels[r.Next(vowels.Length)];
                b++;
            }
            return Name;
        }
        public string RunProcess(string argument)
        {

            Process process = new Process
            {
                StartInfo =
                {
                    FileName = Executable,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    Arguments = argument,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            process.WaitForExit(5000);
            return process.StandardOutput.ReadToEnd();
        }

        public string CreateAccount(string email, Configuration config)
        {
            var password = config.Password.AutoGenerate ? GeneratePassword() : config.Password.Default;
            var argument = $"signup {email} {password} ";
            if (config.Name.Keep == true)
            {
                string name = config.Name.AutoGenerate ? GenerateName() : config.Name.Default;
                argument += $" --name {name}";
            }
            Console.WriteLine(RunProcess(argument));
            return password;
        }

        public bool ConfirmAccount(string email, string password, string link)
        {
            var argument = $"confirm {link} {email} {password}";
            Console.WriteLine(RunProcess(argument));
            return true;
        }
    }
}