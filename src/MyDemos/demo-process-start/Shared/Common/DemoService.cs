using Common;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Common
{
    public static class DemoService
    {
        public static void Run()
        {
            for (int i = 0; i < 10; i++)
            {
                Log(i);
                Thread.Sleep(1000);
            }
            Log("== completed ==");
        }

        private static LogHelper logHelper = null;
        public static void Log(object msg)
        {
            if (logHelper == null)
            {
                var appName = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().GetName().Name);
                logHelper = LogHelper.Create(appName);
            }
            logHelper.Log(msg);
        }
    }
}