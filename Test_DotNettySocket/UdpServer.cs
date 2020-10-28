using System;
using System.Text;
using System.Threading.Tasks;
using Coldairarrow.DotNettySocket;

namespace Test_DotNettySocket
{
    public static class UdpServer
    {
        public static  async Task  startServer()
        {
            var theServer = await SocketBuilderFactory.GetUdpSocketBuilder(6003)
                .OnClose(server =>
                {
                    Console.WriteLine($"服务端关闭");
                })
                .OnException(ex =>
                {
                    Console.WriteLine($"服务端异常:{ex.Message}");
                })
                .OnRecieve((server, point, bytes) =>
                {
                    Console.WriteLine($"服务端:收到来自[{point.ToString()}]数据:{Encoding.UTF8.GetString(bytes)}");
                    server.Send(bytes, point);
                })
                .OnSend((server, point, bytes) =>
                {
                    Console.WriteLine($"服务端发送数据:目标[{point.ToString()}]数据:{Encoding.UTF8.GetString(bytes)}");
                })
                .OnStarted(server =>
                {
                    Console.WriteLine($"服务端启动");
                }).BuildAsync();
        }
    }
}