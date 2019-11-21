using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyApp.Tasks
{
    public class AsyncCancelDemo
    {
        private static Task DoLoopTask(CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                Console.WriteLine("do some task");
                while (true)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Console.WriteLine("cancellationToken IsCancellationRequested!");
                        break;
                    }
                    Console.Write(">");

                    ////demo 1: no exception with thread sleep
                    //Thread.Sleep(100);

                    ////demo 2: TaskCanceledException with Task.Delay
                    //try
                    //{
                    //    //引发的异常:“System.Threading.Tasks.TaskCanceledException”(位于 mscorlib.dll 中)
                    //    await Task.Delay(100, cancellationToken);
                    //}
                    //catch (TaskCanceledException e)
                    //{
                    //    Console.WriteLine();
                    //    Console.WriteLine(e);
                    //}

                    //demo 3: no cancellationToken with Task.Delay
                    await Task.Delay(100);
                }
            }, cancellationToken);
        }

        public static void Run()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            DoLoopTask(cts.Token);

            for (int i = 0; i < 3; i++)
            {
                Task.Delay(200).Wait();
            }
            //引发的异常:“System.Threading.Tasks.TaskCanceledException”(位于 mscorlib.dll 中)
            cts.Cancel();
            Console.WriteLine();
            Console.WriteLine("Mission Completed!");
        }

    }
}
