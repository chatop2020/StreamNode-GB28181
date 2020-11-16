using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace Test_RunProcess
{
    class Program
    {
        private static void Process_Exited(object sender, EventArgs e)
        {
            Console.WriteLine("触发Exited事件");
        }
        
        static void Main(string[] args)
        {
            string std = "";
            string err = "";
            var pid= LinuxShell.RunProcess("/usr/local/bin/ffmpeg", "", 1000, out  std,out  err,true, Process_Exited);
           Console.WriteLine(pid);
            Console.WriteLine(std);
            Console.WriteLine(err);

            Console.WriteLine("Hello World!");
        }
    }
}