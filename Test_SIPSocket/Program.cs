using System;
using System.Net;
using socket.core.Server;

namespace Test_SIPSocket
{
    class Program
    {
        static UdpServer udpServer; 
        static TcpPullServer tcpPullServer;
        static void Main(string[] args)
        {
            tcpPullServer = new TcpPullServer(1000, 10240, 5);
            tcpPullServer.OnAccept += Server_OnAccept;
            tcpPullServer.OnReceive += Server_OnReceive;
            tcpPullServer.OnSend += Server_OnSend;
            tcpPullServer.OnClose += Server_OnClose;
            tcpPullServer.Start(10899);
            udpServer = new UdpServer(10240);
            udpServer.Start(10899, true);
            udpServer.OnReceive += UdpServer_OnReceive;
            udpServer.OnSend += UdpServer_OnSend;
            //Console.WriteLine("Hello World!");
            Console.Read();
            
            Console.WriteLine("Hello World!");
        }
        
        private static void UdpServer_OnSend(EndPoint arg1, int arg2)
        {
            //Console.WriteLine("服务端发送长度：" + arg2);
        }

        private static void UdpServer_OnReceive(EndPoint arg1, byte[] arg2, int arg3, int arg4)
        {
            //Console.WriteLine("服务端接收长度：" + arg4);
            udpServer.Send(arg1, arg2, arg3, arg4);
        }
        
        private static void Server_OnAccept(int obj)
        {
            //server.SetAttached(obj, 555);
            //Console.WriteLine($"Pull已连接{obj}");
        }

        private static void Server_OnSend(int arg1, int arg2)
        {
            //Console.WriteLine($"Pull已发送:{arg1} 长度:{arg2}");
        }

        private static void Server_OnReceive(int arg1, int arg2)
        {
            //int aaa = server.GetAttached<int>(arg1);
            //Console.WriteLine($"Pull已接收:{arg1} 长度:{arg2}");
            byte[] data = tcpPullServer.Fetch(arg1, tcpPullServer.GetLength(arg1));
            tcpPullServer.Send(arg1, data, 0, data.Length);
        }

        private static void Server_OnClose(int obj)
        {
            int aaa = tcpPullServer.GetAttached<int>(obj);
            //Console.WriteLine($"Pull断开{obj}");
        }
    }
}