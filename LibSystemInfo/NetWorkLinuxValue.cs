using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace LibSystemInfo
{
    public static class NetWorkLinuxValue
    {
        private static object lockObj = new object();
        private static string ethName = "";

        public static void GetInfo()
        {
            while (true)
            {
                if (string.IsNullOrEmpty(ethName))
                {
                    Thread.Sleep(1000);
                    continue;
                }

                var lines = File.ReadAllLines("/proc/net/dev");
                if (lines.Length > 0)
                {
                  
                    foreach (var str in lines)
                    {
                        if (str.Contains(ethName))
                        {
                           
                            string[] strTmpArr = str.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                            if (strTmpArr.Length > 0)
                            {
                                
                                long tmpRecv = 0;
                                long tmpSend = 0;
                                long.TryParse(strTmpArr[1], out tmpRecv);
                                long.TryParse(strTmpArr[9], out tmpSend);
                                
                                if (tmpRecv > 0 && tmpSend > 0)
                                {
                                    if (perRecvBytes == 0 && perSendBytes == 0)
                                    {
                                        lock (lockObj)
                                        {
                                            perRecvBytes = tmpRecv;
                                            perSendBytes = tmpSend;
                                            _NetWorkStat.CurrentRecvBytes = 0;
                                            _NetWorkStat.CurrentSendBytes = 0;
                                            _NetWorkStat.TotalRecvBytes = 0;
                                            _NetWorkStat.TotalSendBytes = 0;
                                            _NetWorkStat.UpdateTime = DateTime.Now;
                                        }
                                    }
                                    else
                                    {
                                        lock (lockObj)
                                        {
                                            _NetWorkStat.CurrentRecvBytes = tmpRecv - perRecvBytes;
                                            _NetWorkStat.CurrentSendBytes = tmpSend - perSendBytes;
                                            perRecvBytes += _NetWorkStat.CurrentRecvBytes;
                                            perSendBytes += _NetWorkStat.CurrentSendBytes;
                                            _NetWorkStat.TotalRecvBytes += _NetWorkStat.CurrentRecvBytes;
                                            _NetWorkStat.TotalSendBytes += _NetWorkStat.CurrentSendBytes;
                                            _NetWorkStat.UpdateTime = DateTime.Now;
                                        }
                                    }
                                }
                            }

                            break;
                        }
                    }
                }

                Thread.Sleep(1000);
            }
        }

        static NetWorkLinuxValue()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
               
                ProcessHelper tmpProcess = new ProcessHelper(null, null, null);
                tmpProcess.RunProcess("/usr/sbin/route", "", 1000, out string _std, out string _err);
                bool isFound = false;
                if (!string.IsNullOrEmpty(_std))
                {
                    
                    string[] tmpStrArr = _std.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                    if (tmpStrArr.Length > 0)
                    {
                      
                        foreach (var str in tmpStrArr)
                        {
                            if (isFound)
                            {
                                break;
                            }

                            if (!string.IsNullOrEmpty(str) && str.ToLower().Contains("default"))
                            {
                               
                                string[] s1Arr = str.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                                if (s1Arr.Length > 0)
                                {
                                   
                                    string str1 = s1Arr[s1Arr.Length - 1].Trim();
                                    if (!string.IsNullOrEmpty(str1))
                                    {
                                       
                                        ethName = str1;
                                        tmpProcess.RunProcess("/usr/sbin/ifconfig", str1, 1000, out string _std1,
                                            out string _err1);

                                        if (!string.IsNullOrEmpty(_std1))
                                        {
                                          
                                            string[] tmpStrArr1 = _std1.Split('\n',
                                                StringSplitOptions.RemoveEmptyEntries);
                                            if (tmpStrArr1.Length > 0)
                                            {
                                               
                                                foreach (var str2 in tmpStrArr1)
                                                {
                                                    if (!string.IsNullOrEmpty(str2) && str2.Contains("ether"))
                                                    {
                                                        var regex = "([0-9a-fA-F]{2})(([/\\s:-][0-9a-fA-F]{2}){5})";
                                                        var mac = Regex.Match(str2, regex);
                                                        if (mac.Value.Trim().Length == 17)
                                                        {
                                                            _NetWorkStat.Mac = mac.Value.ToUpper().Replace(":", "-")
                                                                .Trim();
                                                            isFound = true;
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                new Thread(new ThreadStart(delegate
                {
                    try
                    {
                        GetInfo();
                    }
                    catch (Exception ex)
                    {
                        //
                    }
                })).Start();
            }
        }


        private static long perSendBytes = 0;
        private static long perRecvBytes = 0;

        public static NetWorkStat _NetWorkStat = new NetWorkStat();

        public static NetWorkStat GetNetworkStat()
        {
            lock (lockObj)
            {
                return _NetWorkStat;
            }
        }
    }
}