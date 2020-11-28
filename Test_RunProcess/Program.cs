using System;
using System.Diagnostics;
using System.Threading;

namespace Test_RunProcess
{
    class Program
    {
        private static Process _process = null;
        private static ProcessHelper _processHelper = null;

        private static void p_Process_Exited(object sender, EventArgs e)
        {
            // 执行结束后触发
            Console.WriteLine("进程退出了");
        }

        private static void p_StdOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                Console.WriteLine("STD->" + e.Data);
            }
        }

        private static void p_ErrOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                Console.WriteLine("ERR->" + e.Data);
            }
        }

        static void run()
        {
            while (true)
            {
                string cmd = Console.ReadLine();
                if (cmd.ToUpper().Trim().Equals("E"))
                {
                    Environment.Exit(0);
                }

                if (cmd.ToUpper().Trim().Equals("K"))
                {
                    var h = _processHelper.KillProcess(_process);
                    Console.WriteLine(h);
                }

                if (cmd.ToUpper().Trim().Equals("R"))
                {
                    var h = _processHelper.RunProcess("/usr/bin/top", "");
                    if (h != null && h.Id > 0)
                    {
                        Console.WriteLine(h.Id);
                        _process = h;
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            _processHelper = new ProcessHelper(p_StdOutputDataReceived, p_ErrOutputDataReceived, p_Process_Exited);
            _process = _processHelper.RunProcess("/usr/bin/top", "");
            Thread t = new Thread(run);
            t.Start();
            while (_process != null && _process.HasExited == false)
            {
                Console.WriteLine("Pid------------------->" + _process.Id);
                Thread.Sleep(1000);
            }

            Console.WriteLine("进程结束，已经退出");
            Console.ReadLine();
        }
    }
}