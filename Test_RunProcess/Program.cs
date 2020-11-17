using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;

namespace Test_RunProcess
{

    class Program{
        
        private static void p_Process_Exited(object sender, EventArgs e)
        {
            // 执行结束后触发
            Console.WriteLine("进程退出了");
        }
        private static void p_StdOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                Console.WriteLine("STD->"+e.Data);
            }
        }
        
        private static void p_ErrOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                Console.WriteLine("ERR->"+e.Data);
            }
        }

        
    static void Main(string[] args)
    {
        Process _process = null;
        ProcessHelper processHelper = new ProcessHelper(null,null,p_Process_Exited);
            _process=processHelper.RunProcess("/usr/bin/top","");
            while (_process!=null && _process.HasExited==false)
            {
                Console.WriteLine("Pid------------------->"+_process.Id);
                Thread.Sleep(1000);
            }
            Console.WriteLine("进程结束，已经退出");
            Console.ReadLine();


        }
    }
}