using System;
using System.Collections.Concurrent;
using System.Runtime.Loader;
using System.Text;
using System.Threading;
using DNLiCore_Socket.Server;


namespace Test_SIPSocketServer
{
    class Program
    {
        private static UdpServer udpServer = null;
     private static void UdpServer_OnSend(System.Net.EndPoint arg1, int arg2)
            {
               Console.WriteLine("【消息发送成功】【客户端地址："+arg1+"】【长度:"+arg2+"】");
            }
    
            private static void UdpServer_OnReceive(System.Net.EndPoint arg1, byte[] arg2, int arg3, int arg4)
            {
                string test = Encoding.UTF8.GetString(arg2);
                Console.WriteLine("【消息接收成功】【客户端地址：" + arg1 + "】【数据:" + test + "】【偏移量:" + arg3 + "】【长度:" + arg4 + "】");
                Thread.Sleep(500);
                udpServer.Send(arg1,arg2,arg3,arg4);
            }
  
        static void Main(string[] args)
        { 
            
            udpServer= new UdpServer(1500);
            udpServer.OnReceive += UdpServer_OnReceive;  //接收数据事件
            udpServer.OnSend += UdpServer_OnSend; //发送数据事件
            udpServer.Start(8099,true);

            Console.ReadLine();
            Console.WriteLine("Hello World!");
        }
    }
}