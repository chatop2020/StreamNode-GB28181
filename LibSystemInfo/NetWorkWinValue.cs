using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace LibSystemInfo
{
    /// <summary>
    /// 由于.net core在非托管的内存读写上存在bug,无法运行，因此此模块在Fw4.0上实现了核心代码，并由Fw4.0编译了exe文件供
    /// 模块调用，因此在使用此模块时，必须要包含WinNetworkStaCli.exe可执行文件
    /// </summary>
    public static class NetWorkWinValue
    {
        private static object lockObj= new object();
        private static string binPath = AppDomain.CurrentDomain.BaseDirectory + "/WinNetworkStaCli.exe";
        private static ProcessHelper SystemInfoProcessHelper = new ProcessHelper(p_StdOutputDataReceived, null, p_Process_Exited);
        private static long perSendBytes=0;
        private static long perRecvBytes=0;
        private static string mac = "";
        public static NetWorkStat _NetWorkStat = new NetWorkStat();
        private static void p_Process_Exited(object sender, EventArgs e)
        {
            SystemInfoProcessHelper.RunProcess(binPath, "");
        }

        private static void p_StdOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                long tmpSendBytes = 0;
                long tmpRecvBytes = 0;
                string[] tmpStrArr = e.Data.Trim().Split("]-[",StringSplitOptions.RemoveEmptyEntries);
                if (tmpStrArr.Length == 3)
                {
                    foreach (var tmpStr in tmpStrArr)
                    {
                        if (tmpStr.Contains("MAC"))
                        {
                            _NetWorkStat.Mac = tmpStr.Replace("MAC:", "").Trim();
                        }
                        if (tmpStr.Contains("发送"))
                        {
                            var per = tmpStr.Replace("发送:", ""); 
                            if(!long.TryParse(per.Trim(),out tmpSendBytes))
                            {
                                break;
                            }
                        }
                        if (tmpStr.Contains("接收"))
                        {
                            var per = tmpStr.Replace("接收:", ""); 
                            if(!long.TryParse(per.Trim(),out tmpRecvBytes))
                            {
                                break;
                            }
                        }
                    }

                    if (tmpSendBytes > 0 && tmpRecvBytes > 0)
                    {
                        if (perSendBytes == 0 && perRecvBytes == 0) //第一次
                        {
                            lock (lockObj)
                            {
                                perSendBytes = tmpSendBytes;
                                perRecvBytes = tmpRecvBytes;
                                _NetWorkStat.CurrentRecvBytes = 0;
                                _NetWorkStat.CurrentSendBytes = 0;
                                _NetWorkStat.TotalRecvBytes = 0;
                                _NetWorkStat.TotalSendBytes = 0;
                                _NetWorkStat.UpdateTime = DateTime.Now;
                            }
                        }
                        else//有数据以后，每次计算差值
                        {
                            lock (lockObj)
                            {
                                long subSendBytes = tmpSendBytes - perSendBytes;
                                long subRecvBytes = tmpRecvBytes - perRecvBytes;
                                perSendBytes =tmpSendBytes;
                                perRecvBytes =tmpRecvBytes;
                                _NetWorkStat.CurrentRecvBytes = subRecvBytes;
                                _NetWorkStat.CurrentSendBytes = subSendBytes;
                                _NetWorkStat.TotalRecvBytes += subRecvBytes;
                                _NetWorkStat.TotalSendBytes += subSendBytes;
                                _NetWorkStat.UpdateTime = DateTime.Now;
                            }
                        }
                    }
                }
                
            }
        }

        
        static NetWorkWinValue()
        {
            if (File.Exists(binPath))
            {
                //Windows下，父亲进程退出后，子进程没有被退出
                Process[] processes = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(binPath));
                if (processes != null && processes.Length > 0)
                {
                    foreach (var process in processes)
                    {
                        if (process != null && process.HasExited == false)
                        {
                            process.Kill();
                        }
                    }
                }
                SystemInfoProcessHelper.RunProcess(binPath, "");
            }
            else
            {
                throw  new FileNotFoundException(binPath+" not found.");
            }
        }

        public static NetWorkStat GetNetworkStat()
        {
            lock (lockObj)
            {
                return _NetWorkStat;
            }
        }
        
    }
}