using CommonFunctions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using StreamNodeWebApi.AutoTasker;

namespace StreamNodeWebApi
{
    public class Program
    {
        /// <summary>
        /// 摄像头自动监控类实例
        /// </summary>
        public static CameraAutoKeeper? CameraAutoKeeper;

        /// <summary>
        /// 录制计划自动监控类实例
        /// </summary>
        public static RecordAutoKeeper? RecordAutoKeeper;

        /// <summary>
        /// 流媒体服务器自动监控类实例
        /// </summary>
        public static StreamNodeKeeperMonitor? StreamNodeKeeperMonitor;

        public static void Main(string[] args)
        {
            CameraAutoKeeper = new CameraAutoKeeper();
            RecordAutoKeeper = new RecordAutoKeeper();
            StreamNodeKeeperMonitor = new StreamNodeKeeperMonitor();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .UseUrls("http://*:" + Common.MySystemConfig.HttpPort.ToString());
                });
    }
}