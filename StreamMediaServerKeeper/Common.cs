using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using IniParser;
using IniParser.Model;

namespace StreamMediaServerKeeper
{
    public static class Common
    {
        public static string WorkPath = Environment.CurrentDirectory + "/";
        public static string ConfigPath = WorkPath + "/Config/config.conf";
        public static string FFmpegBinPath = WorkPath + "ffmpeg";
        public static string StaticFilePath = WorkPath + "www/";
        public static string CutOrMergePath = StaticFilePath + "CutMergeFile/";
        public static string CutOrMergeTempPath = StaticFilePath + "CutMergeDir/";
        public static string RecordPath = StaticFilePath + "record/";
        public static int FFmpegThreadCount = 2;
        public static string MediaServerBinPath = null!;
        public static string StreamNodeServerUrl = null!;
        public static ushort HttpPort;
        public static string Secret = null!;
        public static ushort MediaServerHttpPort;
        public static string MediaServerId = null!;
        public static ProcessApis ProcessApis = new ProcessApis();
        public static string MyIPAddress = "";
        public static ResGetSystemInfo MySystemInfo = null;

        /// <summary>
        /// 自定义的录制文件存储位置
        /// </summary>
        public static string CustomizedRecordFilePath = "";

      //  public static LogMonitor LogMonitor = new LogMonitor();


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


        private static string checkMediaServerConfig()
        {
            string dir = Path.GetDirectoryName(MediaServerBinPath) + "/";
            if (Directory.Exists(dir))
            {
                DirectoryInfo di = new DirectoryInfo(dir);
                if (di != null)
                {
                    foreach (var file in di.GetFiles())
                    {
                        if (file != null && file.Extension.ToLower().Equals(".ini"))
                        {
                            return file.FullName;
                        }
                    }
                }
            }

            return null!;
        }

        /// <summary>
        /// 获取随机字符串
        /// </summary>
        /// <returns></returns>
        private static string generalGuid()
        {
            Random rand = new Random((int) DateTime.Now.Ticks);
            string random_str = "";
            // std::string random_str("");
            for (int i = 0; i < 6; ++i)
            {
                for (int j = 0; j < 8; j++)
                    switch (rand.Next() % 2)
                    {
                        case 1:
                            random_str += (char) ('A' + rand.Next() % 26);
                            break;
                        default:
                            random_str += (char) ('0' + rand.Next() % 10);
                            break;
                    }

                if (i < 5)
                    random_str += "-";
            }

            return random_str;
        }


        
       
        /// <summary>
        /// 替换#开头的所有行为;开头
        /// </summary>
        /// <param name="filePath"></param>
        private static void processZLMediaKitConfigFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                var list = File.ReadAllLines(filePath).ToList();
                var tmp_list = new List<string>();
                foreach (var str in list)
                {
                    if (!str.StartsWith('#'))
                    {
                        tmp_list.Add(str);
                    }
                    else
                    {
                        int index = str.IndexOf("#", StringComparison.Ordinal);
                        tmp_list.Add(str.Remove(index,index).Insert(index,";"));
                    }
                }
                File.WriteAllLines(filePath,tmp_list);
            }
        }

        /// <summary>
        /// 读取流媒体配置文件中关键信息
        /// </summary>
        private static void getMediaServerConfig()
        {
            string iniPath = checkMediaServerConfig();
            processZLMediaKitConfigFile(iniPath); //处理FileIniDataParser碰到#开头的行，解析错误的问题
            var parser = new FileIniDataParser();
            
            IniData data = parser.ReadFile(iniPath);
            var _tmpStr = data["general"]["mediaServerId"];
            if (!string.IsNullOrEmpty(_tmpStr))
            {
                MediaServerId = _tmpStr.Trim();
            }
            else
            {
                //生成一个id
                data["general"]["mediaServerId"] = generalGuid();
                MediaServerId = data["general"]["mediaServerId"];
                parser.WriteFile(iniPath, data);
            }

            _tmpStr = data["http"]["port"];
            if (string.IsNullOrEmpty(_tmpStr))
            {
                data["http"]["port"] = "8818";
                MediaServerHttpPort = 8818;
                parser.WriteFile(iniPath, data);
            }
            else
            {
                MediaServerHttpPort = ushort.Parse(_tmpStr);
            }

            _tmpStr = data["api"]["secret"];
            if (!string.IsNullOrEmpty(_tmpStr))
            {
                Secret = _tmpStr;
            }
            else
            {
                throw new DataException();
            }

            Uri streamNodeUri = new Uri(StreamNodeServerUrl);
            data["hook"]["enable"] = "1";
            data["hook"]["on_flow_report"] =
                "http://" + streamNodeUri.Host + ":" + streamNodeUri.Port +
                "/WebHook/OnStop"; //流量统计，断开连接时超过flowThreshold值时会触发
            data["hook"]["on_http_access"] = ""; //http事件，不作支持
            data["hook"]["on_play"] =
                "http://" + streamNodeUri.Host + ":" + streamNodeUri.Port + "/WebHook/OnPlay"; //有流被客户端播放时
            data["hook"]["on_publish"] =
                "http://" + streamNodeUri.Host + ":" + streamNodeUri.Port + "/WebHook/OnPublish"; //有流发布时
            data["hook"]["on_record_mp4"] =
                "http://" + streamNodeUri.Host + ":" + streamNodeUri.Port + "/WebHook/OnRecordMp4Completed"; //当录制mp4完成时
            data["hook"]["on_record_ts"] =
                "http://" + streamNodeUri.Host + ":" + streamNodeUri.Port + "/WebHook/OnRecordTsCompleted"; //当录制ts完成时
            data["hook"]["on_rtsp_auth"] = ""; //rtsp鉴权，不作支持
            data["hook"]["on_rtsp_realm"] = ""; //rtsp专用鉴权，不作支持
            data["hook"]["on_shell_login"] = ""; //shell鉴权，不作支持
            data["hook"]["on_stream_changed"] =
                "http://" + streamNodeUri.Host + ":" + streamNodeUri.Port + "/WebHook/OnStreamChange"; //流注册或注销时
            data["hook"]["on_stream_none_reader"] =
                "http://" + streamNodeUri.Host + ":" + streamNodeUri.Port + "/WebHook/OnStreamNoneReader"; //流无人观看时
            data["hook"]["on_stream_not_found"] = ""; //请求没有找到对应流的时候，不作支持
            data["hook"]["on_server_started"] = "http://" + streamNodeUri.Host + ":" + streamNodeUri.Port +
                                                "/WebHook/OnMediaServerStart"; //当流媒体启动时
            data["hook"]["timeoutSec"] = "5"; //httpclient超时时间5秒
            data["general"]["flowThreshold"] = "1"; //当用户超过1byte流量时，将触发on_flow_report的webhook(/WebHook/OnStop)

            parser.WriteFile(iniPath, data);
        }


        private static bool checkConfigFile()
        {
            if (File.Exists(ConfigPath))
            {
                return true;
            }

            return false;
        }

        public static bool GetConfig()
        {
            if (checkConfigFile())
            {
                var ret = File.ReadAllLines(ConfigPath);
                foreach (var str in ret)
                {
                    if (!string.IsNullOrEmpty(str) && !str.Trim().StartsWith("#") && str.Trim().EndsWith(";"))
                    {
                        if (str.ToLower().Contains("streamnodeserverurl"))
                        {
                            string[] tmpArr = str.Trim().Split("::", StringSplitOptions.RemoveEmptyEntries);
                            if (tmpArr.Length == 2)
                            {
                                StreamNodeServerUrl = tmpArr[1].Trim().TrimEnd(';');
                            }
                            else
                            {
                                return false;
                            }
                        }

                        if (str.ToLower().Contains("mediaserverbinpath"))
                        {
                            string[] tmpArr = str.Trim().Split("::", StringSplitOptions.RemoveEmptyEntries);
                            if (tmpArr.Length == 2)
                            {
                                MediaServerBinPath = tmpArr[1].Trim().TrimEnd(';');
                            }
                            else
                            {
                                return false;
                            }
                        }

                        if (str.ToLower().Contains("httpport"))
                        {
                            string[] tmpArr = str.Trim().Split("::", StringSplitOptions.RemoveEmptyEntries);
                            if (tmpArr.Length == 2)
                            {
                                HttpPort = ushort.Parse(tmpArr[1].Trim().TrimEnd(';'));
                            }
                            else
                            {
                                return false;
                            }
                        }

                        if (str.ToLower().Contains("ipaddress"))
                        {
                            string[] tmpArr = str.Trim().Split("::", StringSplitOptions.RemoveEmptyEntries);
                            if (tmpArr.Length == 2)
                            {
                                MyIPAddress = tmpArr[1].Trim().TrimEnd(';');
                            }
                            else
                            {
                                return false;
                            }
                        }

                        if (str.ToLower().Contains("customizedrecordfilepath"))
                        {
                            string[] tmpArr = str.Trim().Split("::", StringSplitOptions.RemoveEmptyEntries);
                            if (tmpArr.Length == 2)
                            {
                                CustomizedRecordFilePath = tmpArr[1].Trim().TrimEnd(';');
                                if (!string.IsNullOrEmpty(CustomizedRecordFilePath))
                                {
                                    DirectoryInfo di = null;
                                    if (!Directory.Exists(CustomizedRecordFilePath))
                                    {
                                        di = Directory.CreateDirectory(CustomizedRecordFilePath);
                                    }
                                    else
                                    {
                                        di = new DirectoryInfo(CustomizedRecordFilePath);
                                    }

                                    if (di != null && di.Exists)
                                    {
                                        //如果自定义的存储位置不为空，则使用自定义存储位置替换原有存储位置
                                        RecordPath = CustomizedRecordFilePath;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(HttpPort.ToString()) && !string.IsNullOrEmpty(MediaServerBinPath) &&
                !string.IsNullOrEmpty(StreamNodeServerUrl))
            {
                return true;
            }

            return false;
        }

        private static bool checkMediaServerBin(string filePath)
        {
            if (File.Exists(filePath))
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// 结束自己
        /// </summary>
        public static void KillSelf(string message = "")
        {
            Logger.Logger.Fatal("异常情况，结束自己进程..." + "-> " + message);
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


        private static void KeepAlive()
        {
            int i = 0;
            while (true)
            {
                i++;
                if (i > 999999999)
                {
                    i = 1;
                    MySystemInfo = null;
                }

                try
                {
                    ReqMediaServerReg req = null;
                    if (i == 1 || i % 5 == 0 || MySystemInfo == null)
                    {
                        MySystemInfo = new ResGetSystemInfo();
                        req = new ReqMediaServerReg()
                        {
                            Ipaddress = MyIPAddress,
                            MediaServerHttpPort = MediaServerHttpPort,
                            MediaServerId = MediaServerId,
                            Secret = Secret,
                            WebApiServerhttpPort = HttpPort,
                            RecordFilePath = RecordPath,
                            SystemInfo = MySystemInfo,
                        };
                    }
                    else
                    {
                        req = new ReqMediaServerReg()
                        {
                            Ipaddress = MyIPAddress,
                            MediaServerHttpPort = MediaServerHttpPort,
                            MediaServerId = MediaServerId,
                            Secret = Secret,
                            WebApiServerhttpPort = HttpPort,
                            RecordFilePath = RecordPath,
                            SystemInfo = null,
                        };
                    }

                    string reqData = JsonHelper.ToJson(req);
                    try
                    {
                        var httpRet = NetHelper.HttpPostRequest(StreamNodeServerUrl, null!, reqData, "utf-8", 5000);
                        if (string.IsNullOrEmpty(httpRet))
                        {
                            MySystemInfo = null;
                            if (ProcessApis.CheckIsRunning(out _) > 0)
                            {
                                ProcessApis.StopServer(out _); //发现streamctrl异常回复，就关掉流媒体服务器
                            }
                        }
                        else
                        {
                            if (ProcessApis.CheckIsRunning(out _) == 0)
                            {
                                ProcessApis.RunServer(out _); //如果正常返回，但是流媒体没启动，则启动流媒体
                            }
                        }
                    }
                    catch
                    {
                        MySystemInfo = null;
                        if (ProcessApis.CheckIsRunning(out _) > 0)
                        {
                            ProcessApis.StopServer(out _); //发现streamctrl异常回复，就关掉流媒体服务器
                        }
                    }

                    Thread.Sleep(1000 * 2);
                }
                catch (Exception ex)
                {
                    MySystemInfo = null;
                    Logger.Logger.Error("报错了-> " + ex.Message + " -> " + ex.StackTrace);
                    continue;
                }
            }
        }

        static Common()
        {
            try
            {
                CutMergeService.start = true;
                if (checkConfigFile())
                {
                    if (GetConfig())
                    {
                        if (!checkMediaServerBin(MediaServerBinPath))
                        {
                            KillSelf("流媒体可执行文件不存在");
                        }

                        if (string.IsNullOrEmpty(checkMediaServerConfig()))
                        {
                            KillSelf("流媒体配置文件不存在");
                        }

                        getMediaServerConfig();
                    }
                    else
                    {
                        KillSelf("读取配置文件异常");
                    }
                }
                else
                {
                    KillSelf("配置文件不存在");
                }

                ErrorMessage.Init();
            }
            catch (Exception ex)
            {
                KillSelf(ex.Message);
            }

            new Thread(new ThreadStart(delegate
            {
                try
                {
                    KeepAlive();
                }
                catch (Exception ex)
                {
                    //
                }
            })).Start();
        }
    }
}