using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using CommonFunction.ManageStructs;
using CommonFunctions.DBStructs;
using CommonFunctions.MediaServerControl;
using CommonFunctions.WebApiStructs.Response;
using LibGB28181SipGate;

namespace CommonFunctions
{
    public static class Common
    {
        public static bool IsDebug = true;
        public static string WorkPath = Environment.CurrentDirectory + "/";
        public static string SystemConfigPath = WorkPath + "/Config/system.conf";
        public static string SystemLogPath = WorkPath + "log/";
        public static int FFmpegThreadCount = 2;
        public static string FFmpegBinPath = "./ffmpeg";
        public static SystemConfig MySystemConfig = new SystemConfig();
        public static SipCoreHelper SipProcess;
        public static SessionManager SessionManager = new SessionManager();
        public static List<MediaServerInstance> MediaServerList = new List<MediaServerInstance>();
        public static List<CameraInstance> CameraInstanceList = new List<CameraInstance>();
        

        //在线的摄像头列表
        public static List<CameraSession> CameraSessions = new List<CameraSession>();

        //在线播放用户列表
        public static List<PlayerSession> PlayerSessions = new List<PlayerSession>();
        public static object CameraSessionLock = new object();
        public static object SipDeviceListLock = new object();
        public static object CameraInstanceListLock = new object();
        public static object PlayerSessionListLock = new object();

        public static ResGetSystemInfo SystemInfo = new ResGetSystemInfo();


        static Common()
        {
            if (!MySystemConfig.LoadConfig(SystemConfigPath))
            {
                KillSelf();
            }

            if (!Directory.Exists(SystemLogPath))
            {
                Directory.CreateDirectory(SystemLogPath);
            }

            ErrorMessage.Init();

            SipProcess = new SipCoreHelper(false); //启动sip服务
            SipProcess.Start();
        }

        /// <summary>
        /// 是否为Url
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsUrl(string str)
        {
            try
            {
                string Url = @"^http(s)?://([\w-]+\.)+[\w-]+(:\d*)?(/[\w- ./?%&=]*)?$";
                return Regex.IsMatch(str, Url);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 对象拷贝
        /// </summary>
        /// <param name="obj">被复制对象</param>
        /// <returns>新对象</returns>
        public static object CopyOjbect(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            Object targetDeepCopyObj;
            Type targetType = obj.GetType();
            //值类型  
            if (targetType.IsValueType == true)
            {
                targetDeepCopyObj = obj;
            }
            //引用类型   
            else
            {
                targetDeepCopyObj = Activator.CreateInstance(targetType); //创建引用对象   
                MemberInfo[] memberCollection = obj.GetType().GetMembers();

                foreach (MemberInfo member in memberCollection)
                {
                    //拷贝字段
                    if (member.MemberType == MemberTypes.Field)
                    {
                        FieldInfo field = (FieldInfo) member;
                        Object fieldValue = field.GetValue(obj);
                        if (fieldValue is ICloneable)
                        {
                            field.SetValue(targetDeepCopyObj, (fieldValue as ICloneable).Clone());
                        }
                        else
                        {
                            field.SetValue(targetDeepCopyObj, CopyOjbect(fieldValue));
                        }
                    } //拷贝属性
                    else if (member.MemberType == MemberTypes.Property)
                    {
                        PropertyInfo myProperty = (PropertyInfo) member;

                        MethodInfo info = myProperty.GetSetMethod(false);
                        if (info != null)
                        {
                            try
                            {
                                object propertyValue = myProperty.GetValue(obj, null);
                                if (propertyValue is ICloneable)
                                {
                                    myProperty.SetValue(targetDeepCopyObj, (propertyValue as ICloneable).Clone(), null);
                                }
                                else
                                {
                                    myProperty.SetValue(targetDeepCopyObj, CopyOjbect(propertyValue), null);
                                }
                            }
                            catch (Exception ex)
                            {
                                return null;
                            }
                        }
                    }
                }
            }

            return targetDeepCopyObj;
        }


        /// <summary>  
        /// 将 DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param name="time">时间</param>  
        /// <returns>long</returns>  
        public static long ConvertDateTimeToInt(DateTime time)
        {
            DateTime Time = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            long TimeStamp = (time.Ticks - Time.Ticks) / 10000; //除10000调整为13位     
            return TimeStamp;
        }

        /// <summary>  
        /// 将Unix时间戳格式 转换为DateTime时间格式
        /// </summary>  
        /// <param name="time">时间</param>  
        /// <returns>long</returns>  
        public static DateTime ConvertDateTimeToInt(long time)
        {
            DateTime Time = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            DateTime dateTime = Time.AddSeconds(time);
            return dateTime;
        }

        /// <summary>
        /// 正则获取内容
        /// </summary>
        /// <param name="str"></param>
        /// <param name="s"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetValue(string str, string s, string e)
        {
            Regex rg = new Regex("(?<=(" + s + "))[.\\s\\S]*?(?=(" + e + "))",
                RegexOptions.Multiline | RegexOptions.Singleline);
            return rg.Match(str).Value;
        }


        /// <summary>
        /// 获取两个时间差的毫秒数
        /// </summary>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <returns></returns>
        public static long GetTimeGoneMilliseconds(DateTime starttime, DateTime endtime)
        {
            TimeSpan ts = endtime.Subtract(starttime);
            return (long) ts.TotalMilliseconds;
        }


        /// <summary>
        /// 删除List<T>中null的记录
        /// </summary>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        public static void RemoveNull<T>(List<T> list)
        {
            // 找出第一个空元素 O(n)
            int count = list.Count;
            for (int i = 0; i < count; i++)
                if (list[i] == null)
                {
                    // 记录当前位置
                    int newCount = i++;

                    // 对每个非空元素，复制至当前位置 O(n)
                    for (; i < count; i++)
                        if (list[i] != null)
                            list[newCount++] = list[i];

                    // 移除多余的元素 O(n)
                    list.RemoveRange(newCount, count - newCount);
                    break;
                }
        }


        /// <summary>
        /// 检查ffmpeg是否存在
        /// </summary>
        /// <param name="ffpath"></param>
        /// <returns></returns>
        public static bool CheckFFmpegBin(string ffpath = "")
        {
            if (string.IsNullOrEmpty(ffpath))
            {
                ffpath = "ffmpeg";
            }

            LinuxShell.Run(ffpath, 1000, out string std, out string err);
            if (!string.IsNullOrEmpty(std))
            {
                if (std.ToLower().Contains("ffmpeg version"))
                {
                    return true;
                }
            }

            if (!string.IsNullOrEmpty(err))
            {
                if (err.ToLower().Contains("ffmpeg version"))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取时间戳(毫秒级)
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStampMilliseconds()
        {
            return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// 检测是否为ip 地址
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIpAddr(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        public static string? AddDoubleQuotation(string s)
        {
            return "\"" + s + "\"";
        }

        public static string? RemoveDoubleQuotation(string s)
        {
            return s.Replace("\"", "").Replace("{", "").Replace("}", "");
        }


        /// <summary>
        /// 生成guid
        /// </summary>
        /// <returns></returns>
        public static string? CreateUuid()
        {
            return Guid.NewGuid().ToString("D");
        }

        /// <summary>
        /// 是否为GUID
        /// </summary>
        /// <param name="strSrc"></param>
        /// <returns></returns>
        public static bool IsUuidByError(string strSrc)
        {
            if (String.IsNullOrEmpty(strSrc))
            {
                return false;
            }

            bool _result = false;
            try
            {
                Guid _t = new Guid(strSrc);
                _result = true;
            }
            catch
            {
            }

            return _result;
        }


        /// <summary>
        /// 检测session和allow
        /// </summary>
        /// <param name="ipAddr"></param>
        /// <param name="allowKey"></param>
        /// <param name="sessionCode"></param>
        /// <returns></returns>
        public static bool CheckAuth(string ipAddr, string allowKey, string sessionCode)
        {
            if (!CheckSession(sessionCode)) return false;
            if (!CheckAllow(ipAddr, allowKey)) return false;
            return true;
        }

        /// <summary>
        /// 检查密码
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool CheckPassword(string password)
        {
            return MySystemConfig.Password.Trim().Equals(password.Trim());
        }

        /// <summary>
        /// 检查Session是否正常
        /// </summary>
        /// <param name="sessionCode"></param>
        /// <returns></returns>
        public static bool CheckSession(string sessionCode)
        {
            Session s = SessionManager.SessionList.FindLast(x =>
                x.SessionCode.Trim().ToLower().Equals(sessionCode.Trim().ToLower()))!;
            long a = GetTimeStampMilliseconds();

            if (s != null && s.Expires > a)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 检查appkey
        /// </summary>
        /// <param name="ipAddr"></param>
        /// <param name="allowKey"></param>
        /// <returns></returns>
        public static bool CheckAllow(string ipAddr, string allowKey)
        {
            if (MySystemConfig.AllowKeys == null ||
                MySystemConfig.AllowKeys.Count == 0) return true;
            foreach (var ak in MySystemConfig.AllowKeys)
            {
                foreach (var ip in ak.IpArray)
                {
                    string[] ip_tmp;
                    string[] ipAddr_tmp;
                    string ipReal;
                    string ipAddrReal;
                    ipReal = ip;
                    ipAddrReal = ipAddr;
                    if (ip.Trim() == "*" || string.IsNullOrEmpty(ip))
                    {
                        if (allowKey.Trim().ToLower().Equals(ak.Key.Trim().ToLower()))
                        {
                            return true;
                        }

                        return false;
                    }

                    if (ip.Contains('*'))
                    {
                        ip_tmp = ip.Split('.', StringSplitOptions.RemoveEmptyEntries);
                        ipAddr_tmp = ipAddr.Split('.', StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i <= ip_tmp.Length - 1; i++)
                        {
                            if (ip_tmp[i].Trim().Equals("*"))
                            {
                                ipAddr_tmp[i] = "*";
                            }
                        }

                        ipReal = String.Join(".", ip_tmp);
                        ipAddrReal = String.Join(".", ipAddr_tmp);
                    }

                    if (ipReal.Trim().Equals(ipAddrReal.Trim()) &&
                        allowKey.Trim().ToLower().Equals(ak.Key.Trim().ToLower()))
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        /// <summary>
        /// 结束自己
        /// </summary>
        public static void KillSelf()
        {
            //  LogWriter.WriteLog("因异常结束进程...");
            Logger.Logger.Fatal("因异常结束进程...");
            string fileName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);
            var ret = GetProcessPid(fileName);
            if (ret > 0)
            {
                KillProcess(ret);
            }
        }

        public static void KillProcess(int pid)
        {
            string cmd = "kill -9 " + pid.ToString();
            LinuxShell.Run(cmd, 1000);
        }

        /// <summary>
        /// 获取pid
        /// </summary>
        /// <param name="processName"></param>
        /// <returns></returns>
        public static int GetProcessPid(string processName)
        {
            string cmd = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                cmd = "ps -aux |grep " + processName + "|grep -v grep|awk \'{print $2}\'";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                cmd = "ps -A |grep " + processName + "|grep -v grep|awk \'{print $1}\'";
            }

            LinuxShell.Run(cmd, 1000, out string std, out string err);
            if (string.IsNullOrEmpty(std) && string.IsNullOrEmpty(err))
            {
                return -1;
            }

            int pid = -1;
            if (!string.IsNullOrEmpty(std))
            {
                int.TryParse(std, out pid);
            }

            if (!string.IsNullOrEmpty(err))
            {
                int.TryParse(err, out pid);
            }

            return pid;
        }
    }
}