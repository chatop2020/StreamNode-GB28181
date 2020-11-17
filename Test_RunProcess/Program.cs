using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;

namespace Test_RunProcess
{

    class Program{
        
    static void Main(string[] args)
    {
        Process _process = null;
          
            TestProcess testProcess = new TestProcess();
            _process=testProcess.RealAction("/sbin/ping","192.168.2.1");
            while (_process!=null)
            {
                Console.WriteLine("Pid->"+_process.Id);
                Thread.Sleep(1000);
            }
            Console.ReadLine();


        }
    }
}