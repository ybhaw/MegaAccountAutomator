using EO.Internal;

namespace AccountAutomator
{
    public class Configuration
    {
        public Password Password { get; set; }
        public Name Name { get; set; }
        public bool Confirm { get; set; } = true;
        public string MegaDir { get; set; }
        public int Wait { get; set; } = 30;
    }

    public class Password
    {
        public bool AutoGenerate = false;
        public string Default { get; set; } = null;
    }
    public class Name
    {
        public bool Keep = true;
        public bool AutoGenerate = false;
        public string Default { get; set; } = null;
    }
}