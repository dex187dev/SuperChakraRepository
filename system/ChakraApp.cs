using System;
using System.IO;

namespace SuperChakra.system
{
    public class ChakraApp
    {
        // DATENQUELLEN

        private readonly string version = "1.3.6";
        private readonly string appName = "SuperChakra";

        // FELDER

        public string Version => version;
        public string AppName => appName;

        public bool IsDebugMode { get; }

        public string DotNetVersion { get; }
        public string OsVersion { get; }

        public DateTime StartUpTime { get; }

        public string BaseDirectory { get; }

        public string SqlDirectory { get; }        
        public string ExportDirectory { get; }
        public string WebviewDirectory { get; }


        // KONSTRUKTOR

        public ChakraApp() 
        { 
            StartUpTime = DateTime.Now;

            BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            SqlDirectory = Path.Combine(BaseDirectory, "system", "sql");            
            ExportDirectory = Path.Combine(BaseDirectory, "exports");
            WebviewDirectory = Path.Combine(BaseDirectory, "webview");

            DotNetVersion = Environment.Version.ToString();
            OsVersion = Environment.OSVersion.ToString();

#if DEBUG
            IsDebugMode = true;
#else
            IsDebugMode = false;
#endif


        }

        // METHODEN

        public TimeSpan GetUpTime() 
        { 
            return DateTime.Now - StartUpTime;
        }
    }
}
