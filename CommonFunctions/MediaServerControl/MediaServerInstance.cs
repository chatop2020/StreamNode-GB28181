using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using CommonFunction.ManageStructs;
using CommonFunctions.DBStructs;
using CommonFunctions.ManageStructs;
using CommonFunctions.WebApiStructs.Response;
using LibGB28181SipGate;

namespace CommonFunctions.MediaServerControl
{
    /// <summary>
    /// 流媒体服务实例控制类
    /// </summary>
    public class MediaServerInstance
    {
        private string _ipaddress; //流媒体服务器ip地址
        private ushort _webApiPort; //流媒体控制接口端口地址
        private string _secret; //流媒体服务接口验证key
        private string _mediaServerId; //流媒体服务的deviceid
        private ushort _mediaServerHttpPort; //流媒体服务接口端口
        private uint _pid = 0; //流媒体服务的pid
        private DateTime _updateTime; //最后控制时间
        private DateTime _keepAlive;
        private ZLMediaKitWebApiHelper _webApi; //流媒体服务操作类
        private string? _recordFilePath; //保存录制文件的目录

        private ZLMediaKitConfigForResponse _config = new ZLMediaKitConfigForResponse(); //流媒体服务器配置

        public string? Ipaddress
        {
            get => _ipaddress;
            set => _ipaddress = value;
        }

        public ushort WebApiPort
        {
            get => _webApiPort;
            set => _webApiPort = value;
        }

        public ushort MediaServerHttpPort
        {
            get => _mediaServerHttpPort;
            set => _mediaServerHttpPort = value;
        }

        public string Secret
        {
            get => _secret;
            set => _secret = value;
        }

        /// <summary>
        /// 是否正在运行
        /// </summary>
        public bool IsRunning => GetIsRunning(out _);

        /// <summary>
        /// 获取运行时长（秒）
        /// </summary>
        public double Uptime => getUpTime();


        public string MediaServerId
        {
            get => _mediaServerId;
            set => _mediaServerId = value;
        }

        [JsonIgnore]
        public uint Pid
        {
            get => _pid;
            set => _pid = value;
        }

        public DateTime UpdateTime
        {
            get => _updateTime;
            set => _updateTime = value;
        }

        public DateTime KeepAlive
        {
            get => _keepAlive;
            set => _keepAlive = value;
        }


        [JsonIgnore]
        public ZLMediaKitWebApiHelper WebApi
        {
            get => _webApi;
            set => _webApi = value;
        }


        private double getUpTime()
        {
            if (IsRunning)
            {
                return (DateTime.Now - _updateTime).TotalSeconds;
            }

            return 0;
        }

        public string? RecordFilePath
        {
            get => _recordFilePath;
            set => _recordFilePath = value;
        }

        public ZLMediaKitConfigForResponse Config
        {
            get => _config;
            set => _config = value;
        }

        private void checkStreamStatusNew()
        {
            while (true)
            {
                try
                {
                    if (_webApi != null && IsRunning)
                    {
                        ResponseStruct rs;
                        var ret = _webApi.GetMediaList(out rs);
                        lock (Common.CameraSessionLock)
                        {
                            if (Common.CameraSessions != null && Common.CameraSessions.Count > 0)
                                foreach (var session in Common.CameraSessions)
                                {
                                    if (session != null && session.MediaServerId.Equals(_mediaServerId))
                                    {
                                        var retObj = ret.Data.FindLast(x => x.Vhost.Equals(session.Vhost)
                                                                            && x.App.Equals(session.App) &&
                                                                            x.Stream.Equals(session.StreamId));
                                        if (retObj == null)
                                        {
                                            session.IsOnline = false;
                                            ClientOnOffLog tmpClientLog = new ClientOnOffLog()
                                            {
                                                App = session.App,
                                                CameraProtocolType = session.CameraType,
                                                ClientType = session.ClientType,
                                                CreateTime = DateTime.Now,
                                                Ipaddress = session.CameraIpAddress,
                                                CameraId = session.CameraId,
                                                OnOff = OnOff.Off,
                                                PushMediaServerId = _mediaServerId,
                                                Vhost = session.Vhost,
                                                StreamId = session.StreamId,
                                            };
                                            OrmService.Db.Insert<ClientOnOffLog>(tmpClientLog).ExecuteAffrows();
                                        }
                                    }
                                }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("报错了：\r\n" + ex.Message + "\r\n" + ex.StackTrace);
                }

                Thread.Sleep(5000);
            }
        }


        public MediaServerInstance(string ipaddress, ushort webApiPort, ushort mediaServerHttpPort, string secret,
            string mediaServerId, string recordFilePath)
        {
            _ipaddress = ipaddress;
            _webApiPort = webApiPort;
            _mediaServerHttpPort = mediaServerHttpPort;
            _secret = secret;
            _mediaServerId = mediaServerId;
            _webApi = new ZLMediaKitWebApiHelper(_ipaddress, mediaServerHttpPort, _secret);
            _updateTime = DateTime.Now;
            _recordFilePath = recordFilePath;
            new Thread(new ThreadStart(delegate

            {
                try
                {
                    checkStreamStatusNew(); //检测流的状态
                }
                catch (Exception ex)
                {
                    //
                }
            })).Start();
        }

        public bool GetIsRunning(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            string innerUrl = "http://" + _ipaddress + ":" + _webApiPort + "/CheckIsRunning";
            var httpRet = NetHelper.HttpGetRequest(innerUrl, null, "utf-8", 5000);
            if (!string.IsNullOrEmpty(httpRet))
            {
                try
                {
                    uint _tmpUint;
                    var res = uint.TryParse(httpRet, out _tmpUint);
                    if (res && _tmpUint > 0)
                    {
                        return true;
                    }

                    return false;
                }
                catch
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServerCtrlWebApiExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServerCtrlWebApiExcept],
                    };
                    lock (Common.CameraSessionLock)
                    {
                        Common.CameraSessions.Clear();
                    }

                    lock (Common.PlayerSessionListLock)
                    {
                        Common.PlayerSessions.Clear();
                    }

                    return false;
                }
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.Other,
                Message = ErrorMessage.ErrorDic![ErrorNumber.Other],
            };
            lock (Common.CameraSessionLock)
            {
                Common.CameraSessions.Clear();
            }

            lock (Common.PlayerSessionListLock)
            {
                Common.PlayerSessions.Clear();
            }

            return false;
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


            string innerUrl = "http://" + _ipaddress + ":" + _webApiPort + "/StopServer";
            var httpRet = NetHelper.HttpGetRequest(innerUrl, null, "utf-8", 5000);
            if (!string.IsNullOrEmpty(httpRet))
            {
                try
                {
                    bool _tmpBool;
                    var res = bool.TryParse(httpRet, out _tmpBool);
                    if (res && _tmpBool)
                    {
                        _pid = 0;
                        _updateTime = DateTime.Now;
                        lock (Common.CameraSessionLock)
                        {
                            for (int i = 0; i <= Common.CameraSessions.Count - 1; i++)
                            {
                                if (Common.CameraSessions[i].MediaServerId.Equals(_mediaServerId))
                                {
                                    Common.CameraSessions[i] = null;
                                }
                            }
                            Common.RemoveNull(Common.CameraSessions);
                        }

                        lock (Common.PlayerSessionListLock)
                        {
                            for (int i = 0; i <= Common.PlayerSessions.Count - 1; i++)
                            {
                                if (Common.PlayerSessions[i].MediaServerId.Equals(_mediaServerId))
                                {
                                    Common.PlayerSessions[i] = null;
                                }
                            }
                            Common.RemoveNull(Common.PlayerSessions);

                        }

                        return true;
                    }

                    return false;
                }
                catch
                {
                    // ignored
                }
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.Other,
                Message = ErrorMessage.ErrorDic![ErrorNumber.Other],
            };
            return false;
        }


        /// <summary>
        /// 重启流媒体服务
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public bool RestartServer(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            string innerUrl = "http://" + _ipaddress + ":" + _webApiPort + "/RestartServer";
            var httpRet = NetHelper.HttpGetRequest(innerUrl, null, "utf-8", 5000);
            if (!string.IsNullOrEmpty(httpRet))
            {
                try
                {
                    uint _tmpPid;
                    var res = uint.TryParse(httpRet, out _tmpPid);
                    if (res)
                    {
                        if (_tmpPid > 0)
                        {
                            _pid = _tmpPid;
                            _updateTime = DateTime.Now;
                            lock (Common.CameraSessionLock)
                            {
                                for (int i = 0; i <= Common.CameraSessions.Count - 1; i++)
                                {
                                    if (Common.CameraSessions[i].MediaServerId.Equals(_mediaServerId))
                                    {
                                        Common.CameraSessions[i] = null;
                                    }
                                }
                                Common.RemoveNull(Common.CameraSessions);
                            }

                            lock (Common.PlayerSessionListLock)
                            {
                                for (int i = 0; i <= Common.PlayerSessions.Count - 1; i++)
                                {
                                    if (Common.PlayerSessions[i].MediaServerId.Equals(_mediaServerId))
                                    {
                                        Common.PlayerSessions[i] = null;
                                    }
                                }
                                Common.RemoveNull(Common.PlayerSessions);

                            }

                            return true;
                        }

                    }
                }
                catch
                {
                    // ignored
                }
            }
            
            return false;
        }

        /// <summary>
        /// 启动流媒体服务
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public bool RunServer(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            string innerUrl = "http://" + _ipaddress + ":" + _webApiPort + "/StartServer";
            var httpRet = NetHelper.HttpGetRequest(innerUrl, null, "utf-8", 5000);
            if (!string.IsNullOrEmpty(httpRet))
            {
                try
                {
                    uint _tmpPid;
                    var res = uint.TryParse(httpRet, out _tmpPid);
                    if (res)
                    {
                        if (_tmpPid > 0)
                        {
                            _updateTime = DateTime.Now;
                            _pid = _tmpPid;
                            lock (Common.CameraSessionLock)
                            {
                                for (int i = 0; i <= Common.CameraSessions.Count - 1; i++)
                                {
                                    if (Common.CameraSessions[i].MediaServerId.Equals(_mediaServerId))
                                    {
                                        Common.CameraSessions[i] = null;
                                    }
                                }
                                Common.RemoveNull(Common.CameraSessions);
                            }

                            lock (Common.PlayerSessionListLock)
                            {
                                for (int i = 0; i <= Common.PlayerSessions.Count - 1; i++)
                                {
                                    if (Common.PlayerSessions[i].MediaServerId.Equals(_mediaServerId))
                                    {
                                        Common.PlayerSessions[i] = null;
                                    }
                                }
                                Common.RemoveNull(Common.PlayerSessions);

                            }

                            return true;
                        }

                        return false;
                    }
                }
                catch
                {
                    // ignored
                }
            }

            

            return false;
        }


        /// <summary>
        /// 删除一批文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public bool DeleteFileList(List<string> filePath, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            string innerUrl = "http://" + _ipaddress + ":" + _webApiPort + "/DeleteFileList";
            string reqData = JsonHelper.ToJson(filePath);
            var httpRet = NetHelper.HttpPostRequest(innerUrl, null, reqData, "utf-8", 60000);
            if (!string.IsNullOrEmpty(httpRet))
            {
                try
                {
                    bool _ok = false;
                    var res = bool.TryParse(httpRet, out _ok);
                    if (res && _ok)
                    {
                        return true;
                    }

                    return false;
                }
                catch
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServerCtrlWebApiExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServerCtrlWebApiExcept],
                    };

                    return false;
                }
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.Other,
                Message = ErrorMessage.ErrorDic![ErrorNumber.Other],
            };

            return false;
        }


        /// <summary>
        /// 添加一个裁剪与合并任务
        /// </summary>
        /// <param name="task"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public CutMergeTaskStatusResponse GetMergeTaskStatus(string taskId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            string innerUrl = "http://" + _ipaddress + ":" + _webApiPort + "/GetMergeTaskStatus?taskId=" +
                              taskId.Trim();
            var httpRet = NetHelper.HttpGetRequest(innerUrl, null, "utf-8", 5000);

            if (!string.IsNullOrEmpty(httpRet))
            {
                try
                {
                    var resCutMergeTaskStatusResponse = JsonHelper.FromJson<CutMergeTaskStatusResponse>(httpRet);
                    if (resCutMergeTaskStatusResponse != null)
                    {
                        return resCutMergeTaskStatusResponse;
                    }


                    return null;
                }
                catch
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServerCtrlWebApiExcept,
                        Message = httpRet,
                    };
                    return null;
                }
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.Other,
                Message = ErrorMessage.ErrorDic![ErrorNumber.Other],
            };
            return null;
        }

        /// <summary>
        /// 获取裁剪积压任务列表
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public List<CutMergeTaskStatusResponse> GetBacklogTaskList(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            string innerUrl = "http://" + _ipaddress + ":" + _webApiPort + "/GetBacklogTaskList";
            var httpRet = NetHelper.HttpGetRequest(innerUrl, null, "utf-8", 5000);

            if (!string.IsNullOrEmpty(httpRet))
            {
                try
                {
                    var resCutMergeTaskStatusResponseList =
                        JsonHelper.FromJson<List<CutMergeTaskStatusResponse>>(httpRet);
                    if (resCutMergeTaskStatusResponseList != null)
                    {
                        return resCutMergeTaskStatusResponseList;
                    }


                    return null;
                }
                catch
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServerCtrlWebApiExcept,
                        Message = httpRet,
                    };
                    return null;
                }
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.Other,
                Message = ErrorMessage.ErrorDic![ErrorNumber.Other],
            };
            return null;
        }

        /// <summary>
        /// 添加一个裁剪与合并任务
        /// </summary>
        /// <param name="task"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public CutMergeTaskResponse AddCutOrMergeTask(CutMergeTask task, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            string innerUrl = "http://" + _ipaddress + ":" + _webApiPort + "/AddCutOrMergeTask";
            string reqData = JsonHelper.ToJson(task);
            var httpRet = NetHelper.HttpPostRequest(innerUrl, null, reqData, "utf-8", 60000);
            if (!string.IsNullOrEmpty(httpRet))
            {
                try
                {
                    var resCutMergeTaskResponse = JsonHelper.FromJson<CutMergeTaskResponse>(httpRet);
                    if (resCutMergeTaskResponse != null)
                    {
                        return resCutMergeTaskResponse;
                    }


                    return null;
                }
                catch
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServerCtrlWebApiExcept,
                        Message = httpRet,
                    };
                    return null;
                }
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.Other,
                Message = ErrorMessage.ErrorDic![ErrorNumber.Other],
            };
            return null;
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

            string innerUrl = "http://" + _ipaddress + ":" + _webApiPort + "/DeleteFile?filePath=" + filePath;
            var httpRet = NetHelper.HttpGetRequest(innerUrl, null, "utf-8", 5000);
            if (!string.IsNullOrEmpty(httpRet))
            {
                try
                {
                    bool _ok = false;
                    var res = bool.TryParse(httpRet, out _ok);
                    if (res && _ok)
                    {
                        return true;
                    }

                    return false;
                }
                catch
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServerCtrlWebApiExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServerCtrlWebApiExcept],
                    };

                    return false;
                }
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.Other,
                Message = ErrorMessage.ErrorDic![ErrorNumber.Other],
            };

            return false;
        }


        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public bool CheckFileExists(string filePath, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            string innerUrl = "http://" + _ipaddress + ":" + _webApiPort + "/FileExists?filePath=" + filePath;
            var httpRet = NetHelper.HttpGetRequest(innerUrl, null, "utf-8", 5000);
            if (!string.IsNullOrEmpty(httpRet))
            {
                try
                {
                    bool _ok = false;
                    var res = bool.TryParse(httpRet, out _ok);
                    if (res && _ok)
                    {
                        return true;
                    }

                    return false;
                }
                catch
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServerCtrlWebApiExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServerCtrlWebApiExcept],
                    };

                    return false;
                }
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.Other,
                Message = ErrorMessage.ErrorDic![ErrorNumber.Other],
            };

            return false;
        }


        /// <summary>
        /// 清理空目录
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public bool ClearNoFileDir(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            string innerUrl = "http://" + _ipaddress + ":" + _webApiPort + "/ClearNoFileDir";
            var httpRet = NetHelper.HttpGetRequest(innerUrl, null, "utf-8", 5000);
            if (!string.IsNullOrEmpty(httpRet))
            {
                try
                {
                    bool _ok = false;
                    var res = bool.TryParse(httpRet, out _ok);
                    if (res && _ok)
                    {
                        return true;
                    }

                    return false;
                }
                catch
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServerCtrlWebApiExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServerCtrlWebApiExcept],
                    };

                    return false;
                }
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.Other,
                Message = ErrorMessage.ErrorDic![ErrorNumber.Other],
            };

            return false;
        }
    }
}