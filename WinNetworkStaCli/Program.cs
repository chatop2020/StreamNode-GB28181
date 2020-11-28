using System;
using System.Collections.Generic;
using System.Threading;

namespace WinNetworkStaCli
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                List<NetworkAdapter> aList = WindowsNetworkStat.GetNetworkAdapters();
                if (aList.Count > 0)
                {
                    Console.WriteLine("]-[发送:" + aList[0].Send + "]-[" +
                                      "接收:" + aList[0].Recv + "]-[" +
                                      "MAC:" + aList[0].MAC + "]-[");
                }

                Thread.Sleep(1000);
            }
        }
    }
}