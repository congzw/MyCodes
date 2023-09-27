using System;
using System.IO;

namespace Common
{
    public class LogHelper
    {
        public static LogHelper Create(string appName) => new LogHelper(appName);

        public LogHelper(string appName)
        {
            AppName = appName;
            if (File.Exists(LogFile))
            {
                File.Delete(LogFile);
            }
        }

        public string AppName { get; set; }

        public string LogFile => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"log.txt");

        public void Log(object msg)
        {
            var log = $"{AppName} {DateTime.Now} => {msg}{Environment.NewLine}";
            Console.Write(log);
            File.AppendAllText(LogFile, log);
        }
    }
}