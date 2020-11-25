using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace StreamMediaServerKeeper
{
    /// <summary>
    /// 
    /// </summary>
    public class ProcessApis
    {
        private int _pid;

        private Process _zlmediaKitServerProcess;

        private int _isRunning
        {
            get
            {
                if (checkProcessExists())
                {
                    return _pid;
                }

                return 0;
            }
        }

        /// <summary>
        /// 检查文件是否存在
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public bool FileExists(string filePath, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            var h = File.Exists(filePath);
            Logger.Logger.Debug("检查文件是否存在 -> " + filePath + " ->" + h.ToString());
            return h;
        }

        /// <summary>
        /// 删除一个文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public bool DeleteFile(string filePath, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var h = File.Exists(filePath);
            if (h)
            {
                File.Delete(filePath);
            }

            Logger.Logger.Debug("删除文件 -> " + filePath + " ->" + h.ToString());
            return true;
        }

        /// <summary>
        /// 批量删除文件
        /// </summary>
        /// <param name="filePathList"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public bool DeleteFileList(List<string> filePathList, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (filePathList != null && filePathList.Count > 0)
            {
                foreach (var filePath in filePathList)
                {
                    var h = File.Exists(filePath);
                    if (h)
                    {
                        File.Delete(filePath);
                    }

                    Logger.Logger.Debug("删除文件 -> " + filePath + " ->" + h.ToString());
                }
            }

            return true;
        }

        /// <summary>
        /// 删除录制目录中的空目录
        /// </summary>
        public bool ClearNoFileDir(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            try
            {
                string dvrPath = Common.RecordPath;
                if (Directory.Exists(dvrPath))
                {
                    DirectoryInfo dir = new DirectoryInfo(dvrPath);
                    DirectoryInfo[] subdirs = dir.GetDirectories("*.*", SearchOption.AllDirectories);
                    foreach (DirectoryInfo subdir in subdirs)
                    {
                        FileSystemInfo[] subFiles = subdir.GetFileSystemInfos();
                        var l = subFiles.Length;
                        if (l == 0)
                        {
                            subdir.Delete();
                        }

                        Logger.Logger.Debug("清除空目录 ->" + subdir + " ->" + (l == 0 ? "true" : "false"));
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 检查流媒体是否正在运行
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public int CheckIsRunning(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            Logger.Logger.Debug("检查流媒体服务器是否运行 ->" + _isRunning.ToString());
            return _isRunning;
        }

        /// <summary>
        /// 关闭流媒体
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public bool StopServer(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            int i = 0;
            while (checkProcessExists() && i < 10)
            {
                if (Common.ProcessHelper.KillProcess(Common.ProcessOfZLMediaKit))
                {
                    Common.ProcessOfZLMediaKit = null;
                }
       
                Thread.Sleep(200);
                i++;
            }

            if (checkProcessExists())
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Other,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Other],
                };
                Logger.Logger.Debug("关闭流媒体服务器失败...");
                return false;
            }

            Logger.Logger.Debug("关闭流媒体服务器成功...");
            return true;
        }

        /// <summary>
        /// 重启流媒体服务
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public int RestartServer(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (checkProcessExists())
            {
                StopServer(out rs);
            }

            Thread.Sleep(2000);
            var t = RunServer(out rs);
            Logger.Logger.Debug("重启流媒体服务器 ->" + t.ToString());
            return t;
        }

        /// <summary>
        /// 启动流媒体服务
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public int RunServer(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (File.Exists(Common.MediaServerBinPath))
            {
                string Message=null;
                string StackTrace = null;
                if (!checkProcessExists()) //如果不存在，就执行
                {
                    string dir = Path.GetDirectoryName(Common.MediaServerBinPath) + "/";
                    if (!Directory.Exists(dir + "log"))
                    {
                        Directory.CreateDirectory(dir + "log");
                    }

                    try
                    {
                        Common.ProcessOfZLMediaKit = Common.ProcessHelper.RunProcess(Common.MediaServerBinPath, "");
                    }
                    catch(Exception ex)
                    {
                        Message = ex.Message;
                        StackTrace = ex.StackTrace;

                    }

                    int i = 0;
                    while (!checkProcessExists() && i < 50)
                    {
                        i++;
                        Thread.Sleep(100);
                    }

                    if (checkProcessExists())
                    {
                        Logger.Logger.Debug("启动流媒体服务器 -> pid->" + _pid.ToString() + " ->Alreday Exist");
                        return _pid;
                    }

                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.ZLMediaKitRunBinExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.ZLMediaKitRunBinExcept] + "\r\n" + Message +
                                  "\r\n" + StackTrace ,
                    };
                    Logger.Logger.Debug("启动流媒体服务器失败... -> pid->0 ->" + JsonHelper.ToJson(rs));
                    return 0;
                }

                Logger.Logger.Debug("启动流媒体服务器 -> pid->" + _pid.ToString());
                return _pid; //已经启动着的，不用重复启动
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.ZLMediaKitBinNotFound,
                Message = ErrorMessage.ErrorDic![ErrorNumber.ZLMediaKitBinNotFound],
            };
            Logger.Logger.Debug("启动流媒体服务器失败... -> pid->0 ->" + JsonHelper.ToJson(rs));
            return 0;
        }

        /// <summary>
        /// 用可执行文件路径确定进程是否正在运行
        /// </summary>
        /// <returns></returns>
        private bool checkProcessExists()
        {
            var processID = Common.ProcessHelper.CheckProcessExists(Common.ProcessOfZLMediaKit);
            if (processID > 0)
            {
                _pid = processID;
                return true;
            }

            return false;
            
        }

    }
}