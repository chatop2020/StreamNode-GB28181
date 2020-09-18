using CommonFunctions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using StreamNodeWebApi.AutoTasker;

namespace StreamNodeWebApi
{
    public class Program
    {
        public static CameraAutoKeeper CameraAutoKeeper;
        public static RecordAutoKeeper RecordAutoKeeper;

        public static void Main(string[] args)
        {
            CameraAutoKeeper = new CameraAutoKeeper();
            RecordAutoKeeper = new RecordAutoKeeper();
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