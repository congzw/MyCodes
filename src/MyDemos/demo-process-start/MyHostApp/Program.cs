using Common;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MyHostApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Task.Run(() => {
                CallToolkit(false);
            });

            DemoService.Run();
        }

        static void CallToolkit(bool runNewWindow)
        {
            var currentProcessPath = Environment.ProcessPath;
            var parentRoot = Path.GetFullPath("../../../../../", currentProcessPath);
            var toolkitPath = Path.Combine(parentRoot, "MyToolkit", "bin", "Debug", "net6.0", "MyToolkit.exe");
            try
            {
                using (Process myProcess = new Process())
                {
                    var startInfo = myProcess.StartInfo;

                    startInfo.FileName = toolkitPath;
                    startInfo.UseShellExecute = false;
                    startInfo.CreateNoWindow = !runNewWindow;
                    myProcess.Start();
                    // This code assumes the process you are starting will terminate itself.
                    // Given that it is started without a window so you cannot terminate it
                    // on the desktop, it must terminate itself or you can do it programmatically
                    // from this application using the Kill method.
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}