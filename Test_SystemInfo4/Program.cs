using System;
using System.Threading;
using Universe.CpuUsage;

namespace Test_SystemInfo4
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                var a = WindowsCpuUsage.Get();
                Console.WriteLine(a.Value.UserUsage);
                Thread.Sleep(100);
            }
        }
    }
}