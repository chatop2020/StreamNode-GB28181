using System;
using System.IO;
using System.Threading;

namespace CommonFunctions
{
    public class LogMonitor
    {
        private int interval = 1000*60*10;

        private void processFileMove(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            string dirPath = Path.GetDirectoryName(filePath)!;
            if (!Directory.Exists(dirPath + "/logbak"))
            {
                Directory.CreateDirectory(dirPath + "/logbak");
            }

            fileName = dirPath + "/logbak/back_" + DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss") +"_"+ fileName;
            File.Copy(filePath, fileName);
            LinuxShell.Run("cat /dev/null >" + filePath);
            LogWriter.WriteLog("转存运行日志,并清空现有日志", filePath + "->" + fileName);
        }

       
       
        private void Run()
        {
            while (true)
            {
                try
                {
                    DirectoryInfo di = new DirectoryInfo(Common.SystemLogPath);
                    foreach (var fi in di.GetFiles())
                    {
                        if (fi != null && fi.Exists && fi.Length >= 1024 * 1000 * 10)
                        {
                            processFileMove(fi.FullName);
                        }
                    }

                    Thread.Sleep(interval);
                }
                catch
                {
                    //
                }
            }
        }

        public LogMonitor()
        {
            new Thread(new ThreadStart(delegate

            {
                try
                {
                    LogWriter.WriteLog("启动日志转存服务...(循环间隔：" + interval + "ms)");
                    Run();
                }
                catch (Exception ex)
                {
                    LogWriter.WriteLog("启动日志转存服务失败...", ex.Message + "\r\n" + ex.StackTrace, ConsoleColor.Yellow);
                }
            })).Start();
        }
    }
}