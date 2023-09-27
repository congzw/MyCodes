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
            var appName = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().GetName().Name);
            var logHelper = LogHelper.Create(appName);
            for (int i = 0; i < 10; i++)
            {
                logHelper.Log(i);
                Thread.Sleep(1000);
            }
            logHelper.Log("== completed ==");
        }
    }
}