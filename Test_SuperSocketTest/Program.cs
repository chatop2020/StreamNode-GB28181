using System;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;


namespace Test_SuperSocketTest
{
    class Program
    {
        public static int clientCount=0;
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var appServer = new AppServer();
            if (!appServer.Setup(new ServerConfig
            {
                Ip = "Any",
                Port = 2020,
                Mode = SocketMode.Udp
            }))//配置服务器
            {
                Console.WriteLine("配置服务器失败！");
                Console.ReadKey();
                return;
            }
            Console.WriteLine();
            //尝试启动服务器
            if (!appServer.Start())
            {
                Console.WriteLine("启动失败！");
                Console.ReadKey();
                return;
            }
            Console.WriteLine("服务器启动成功，按 ‘q’ 键停止服务器！");
                        
            //注册新连接
            appServer.NewSessionConnected += new SessionHandler<AppSession>(appServer_NewSessionConnected);
            //注册命令响应
            appServer.NewRequestReceived += new RequestHandler<AppSession, StringRequestInfo>(appServer_NewRequestReceived);
                       
            while (Console.ReadKey().KeyChar != 'q')
            {
                Console.WriteLine();
                continue;
            }
            //停止服务器
            appServer.Stop();
            Console.WriteLine("服务器已停止！");
            Console.ReadKey();
            
        }
        
        /// <summary>
        /// 处理第一次连接
        /// </summary>
        /// <param name="session">socket会话</param>
        public static void appServer_NewSessionConnected(AppSession session)
        {
            clientCount++;
            session.Send("Hello,you are the " + clientCount + "th connected client!");
        }

        /// <summary>
        /// 处理命令
        /// </summary>
        /// <param name="session">socket会话</param>
        /// <param name="requestInfo">请求的内容，详见官方文档</param>
        public static void appServer_NewRequestReceived(AppSession session, StringRequestInfo requestInfo)
        {
            Console.WriteLine(requestInfo.Body);
            switch (requestInfo.Key.ToUpper())
            {
                case ("001")://同样添加一条命令，更多命令的使用请查阅文档
                    session.Send("Hello,I'm UDP server! The parameter you send is " + requestInfo.Body);
                    break;
            }

        }
        
        
    }
}