using System;

namespace Test_DotNettySocket
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            UdpServer.startServer();
            Console.Read();
        }
    }
}