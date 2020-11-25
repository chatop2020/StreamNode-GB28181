using System;
using System.Runtime.InteropServices;
using System.Threading;
using LibSystemInfo;

namespace Test_CPUUsage
{
    class Program
    {
        static NetWorkStat net;

        static void Main(string[] args)
        {
           
            while (true)
            {
               
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                     net = NetWorkLinuxValue.GetNetworkStat();
                }
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    net = NetWorkWinValue.GetNetworkStat();
                }

                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    net = NetWorkMacValue.GetNetworkStat();
                }


                Console.WriteLine(net.Mac+"->发送:"+Math.Round(net.CurrentSendBytes/1024f,2)+"KB"+"->接收:"+Math.Round(net.CurrentRecvBytes/1024f,2)+"KB"+"->总发送:"+Math.Round(net.TotalSendBytes/1024f/1024f,2)+"MB"+"->总接收:"+Math.Round(net.TotalRecvBytes/1024f/1024f,2)+"MB");
                Thread.Sleep(1000);
            }

            Console.WriteLine("Hello World!");
        }
    }
}