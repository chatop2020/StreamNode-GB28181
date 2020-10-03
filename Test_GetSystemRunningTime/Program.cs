using System;
using System.IO;

namespace Test_GetSystemRunningTime
{
    class Program
    {
        
        
      public static string GetSystemRunningTimeText(long d)
        {
            long tickcount = d;

            long days = tickcount / (24 * 60 * 60 * 1000); //24*60*60*1000
            tickcount = tickcount % (24 * 60 * 60 * 1000);

            long hours = tickcount / (60 * 60 * 1000); //60*60*1000
            tickcount = tickcount % (60 * 60 * 1000);

            long minutes = tickcount / (60 * 1000); //60*1000
            tickcount = tickcount % (60 * 1000);

            long seconds = tickcount / 1000; //1000
            tickcount = tickcount % 1000;

            long milliseconds = tickcount;

            return $"{days}天{hours}时{minutes}分{seconds}秒{milliseconds}毫秒";
        }
        
        
       

        
        
        public static long  GetSystemRunningTime()
        {
            string std = "";
            if (File.Exists("/proc/uptime"))
            {
                std = File.ReadAllText("/proc/uptime");
                string[] s_arr = std.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (s_arr.Length == 2)
                {
                    double r;
                    var ret = double.TryParse(s_arr[0], out r);
                    if (ret)
                    {
                        return (long) (r*1000);
                    }
                }
            }
           
            return 0;
        }
        
        
        
        static void Main(string[] args)
        {
            Console.WriteLine(GetSystemRunningTime());
            Console.WriteLine(GetSystemRunningTimeText(GetSystemRunningTime()));
            Console.WriteLine("Hello World!");
        }
    }
}