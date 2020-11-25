using System;
using System.Threading;
using LibSystemInfo;

namespace Test_GetSystemInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            SystemInfo SystemInfo= new SystemInfo();
           
            while (true)
            {
                Console.WriteLine(SystemInfo.GetSystemInfoJson());
                Thread.Sleep(1000);
            }
            Console.WriteLine("Hello World!");
        }
    }
}