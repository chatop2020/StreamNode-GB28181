using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace StreamMediaServerKeeper
{
    public class Program
    {
        public static string WorkPath = Environment.CurrentDirectory + "/";

        public static void Main(string[] args)
        {
            Console.WriteLine("systeminfo:\r\n" + JsonHelper.ToJson(new ResGetSystemInfo()));


            ///启动一下，Common对象
            if (string.IsNullOrEmpty(Common.CustomizedRecordFilePath))
            {
                Common.CustomizedRecordFilePath = "";
            }

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().UseUrls("http://*:" + Common.HttpPort);
                });
    }
}