using Common;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace MyHostApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //测试结果:createNoWindow = true
            //1 Console.Wirte不共享窗口
            //2 主进程退出或强制关闭，被调【程序MyToolkit】或【服务MyToolkitSrv】都能执行完毕
            var createNoWindow = false;
            Task.Run(() =>
            {
                CallToolkit(createNoWindow);
            });
            DemoService.Run();
        }

        static void CallToolkit(bool createNoWindow)
        {
            //var invokeAppName = "MyToolkit";
            var invokeAppName = "MyToolkitSrv";
            var currentProcessPath = Environment.ProcessPath;
            var parentRoot = Path.GetFullPath("../../../../../", currentProcessPath);
            var toolkitPath = Path.Combine(parentRoot, invokeAppName, "bin", "Debug", "net6.0", $"{invokeAppName}.exe");
            try
            {
                using (Process myProcess = new Process())
                {
                    var startInfo = myProcess.StartInfo;

                    startInfo.FileName = toolkitPath;
                    startInfo.UseShellExecute = false;
                    startInfo.CreateNoWindow = createNoWindow;
                    myProcess.Start();
                    myProcess.WaitForExit();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}