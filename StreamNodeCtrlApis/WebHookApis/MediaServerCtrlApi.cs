using System;
using System.Linq;
using CommonFunction.ManageStructs;
using CommonFunctions;
using CommonFunctions.DBStructs;
using CommonFunctions.ManageStructs;
using CommonFunctions.MediaServerControl;
using CommonFunctions.WebApiStructs.Request;
using CommonFunctions.WebApiStructs.Response;
using GB28181.Servers.SIPMonitor;
using GB28181.Sys.Model;
using LibGB28181SipGate;
using StreamNodeCtrlApis.SystemApis;

namespace StreamNodeCtrlApis.WebHookApis
{
    public static class MediaServerCtrlApi
    {
        /// <summary>
        /// 从注册摄像头列表中获取摄像头实例
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        private static CameraInstance GetCameraFromCameraInstance(CameraSession session)
        {
            lock (Common.CameraInstanceListLock)
            {
                if (Common.CameraInstanceList != null && Common.CameraInstanceList.Count() > 0)
                {
                    foreach (var camera in Common.CameraInstanceList)
                    {
                        if (camera != null && camera.CameraChannelLable!.Equals(session.CameraEx!.Camera.DeviceID)
                                           && camera.CameraDeviceLable!.Equals(session.CameraEx!.Camera.ParentID))
                        {
                            return JsonHelper.FromJson<CameraInstance>(JsonHelper.ToJson(camera));
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 在sip设备中找到某个streamid的摄像头信息
        /// </summary>
        /// <param name="streamId"></param>
        /// <returns></returns>
        private static CameraEx getCameraFromSipDevice(uint streamId, string mediaServerId, string app, string vhost)
        {
            lock (Common.SipDeviceListLock)
            {
                if (Common.SipProcess.SipDeviceList != null && Common.SipProcess.SipDeviceList.Count > 0)
                {
                    foreach (var sipDev in Common.SipProcess.SipDeviceList)
                    {
                        if (sipDev != null && sipDev.CameraExList != null && sipDev.CameraExList.Count > 0)
                        {
                            foreach (var camera in sipDev.CameraExList)
                            {
                                if (camera.StreamId.Equals(streamId) && camera.Vhost.Equals(vhost) &&
                                    camera.App.Equals(app)
                                    && camera.MediaServerId.Equals(mediaServerId))
                                {
                                    return camera;
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        private static bool getCameraSessionInfoEx(ref CameraSession session)
        {
            uint tmpStreamid = Convert.ToUInt32("0x" + session.StreamId, 16); //16进制返转成uint型
            //先查一下在不在28181列表中
            var cameraEx = getCameraFromSipDevice(tmpStreamid, session.MediaServerId, session.App, session.Vhost);
            if (cameraEx != null)
            {
                //找到了sip设备
                session.CameraType = CameraType.GB28181;
                session.CameraEx = cameraEx;
                //获取cameraId
                var cameraInstance = GetCameraFromCameraInstance(session);
                if (cameraInstance != null)
                {
                    session.CameraId = cameraInstance.CameraId;
                    session.CameraName = cameraInstance.CameraName;
                    session.CameraDeptId = cameraInstance.DeptId;
                    session.CameraDeptName = cameraInstance.DeptName;
                    session.CameraPDeptId = cameraInstance.PDetpId;
                    session.CameraPDeptName = cameraInstance.PDetpName;
                    return true;
                }
            }
            else //应该是直播流,鉴权还要完善
            {
                session.CameraType = CameraType.LiveCast;
                session.ClientType = ClientType.Livecast;
                session.CameraEx = null;
                session.CameraId = "";
                session.CameraName = "";
                session.CameraDeptId = "";
                session.CameraDeptName = "";
                session.CameraPDeptId = "";
                session.CameraPDeptName = "";
                return true;
            }

            return false;
        }


        /// <summary>
        /// 只有rtp推流或才rtmp，rtsp推流才会有onpublish事件触发，因此这里忽略ffmpeg推流的方式
        /// </summary>
        /// <param name="req"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static ResToWebHookOnPublish OnPublishNew(ReqForWebHookOnPublish req, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var mediaServer = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(req.MediaServerId));
            if (mediaServer == null)
            {
                return new ResToWebHookOnPublish()
                {
                    Code = -1,
                    EnableHls = false,
                    EnableMp4 = false,
                    EnableRtxp = false,
                    Msg = "failed",
                };
            }

            CameraSession session = null;
            lock (Common.CameraSessionLock)
            {
                session = Common.CameraSessions.FindLast(x => x.MediaServerId.Equals(req.MediaServerId)
                                                              && x.Vhost.Equals(req.Vhost) &&
                                                              x.App.Equals(req.App) &&
                                                              x.StreamId.Equals(req.Stream) &&
                                                              (x.ClientType == ClientType.Camera ||
                                                               x.ClientType == ClientType.Livecast)
                    /*&& x.CameraIpAddress.Equals(req.Ip)*/); //为支持公网远程设备的ip不固定性，取消ip 地址校验
            }

            if (session == null)
            {
                //这是新的流，要增加
                session = new CameraSession()
                {
                    App = req.App,
                    Vhost = req.Vhost,
                    StreamId = req.Stream,
                    CameraIpAddress = req.Ip,
                    UpTime = 0f,
                    OnlineTime = DateTime.Now,
                    MediaServerId = req.MediaServerId,
                    IsRecord = false,
                    PlayUrl = "http://" + mediaServer.Ipaddress + ":" + mediaServer.MediaServerHttpPort +
                              "/" + req.App + "/" + req.Stream + ".flv",
                    IsOnline = true,
                    ForceOffline = false,
                    MediaServerIp = mediaServer.Ipaddress,
                };
                if (getCameraSessionInfoEx(ref session))
                {
                    ClientOnOffLog tmpClientLog = new ClientOnOffLog()
                    {
                        App = session.App,
                        CameraProtocolType = session.CameraType,
                        ClientType = session.ClientType,
                        CreateTime = DateTime.Now,
                        Ipaddress = session.CameraIpAddress,
                        CameraId = session.CameraId,
                        OnOff = OnOff.On,
                        PushMediaServerId = mediaServer.MediaServerId,
                        Vhost = session.Vhost,
                        StreamId = session.StreamId,
                        Schema = "like gb28181",
                    };
                    OrmService.Db.Insert<ClientOnOffLog>(tmpClientLog).ExecuteAffrows();
                    lock (Common.CameraSessionLock)
                    {
                        Common.CameraSessions.Add(session);
                    }
                }
                else
                {
                    //这里是没有找到摄像头相关信息，返回zlmediakit失败
                    return new ResToWebHookOnPublish()
                    {
                        Code = -1,
                        EnableHls = false,
                        EnableMp4 = false,
                        EnableRtxp = false,
                        Msg = "failed",
                    };
                }
            }
            else
            {
                lock (Common.CameraSessionLock)
                {
                    session.IsOnline = true;
                    session.OnlineTime = DateTime.Now;
                    session.UpTime = 0;
                    session.ForceOffline = false;
                }

                ClientOnOffLog tmpClientLog = new ClientOnOffLog()
                {
                    App = session.App,
                    CameraProtocolType = session.CameraType,
                    ClientType = session.ClientType,
                    CreateTime = DateTime.Now,
                    Ipaddress = session.CameraIpAddress,
                    CameraId = session.CameraId,
                    OnOff = OnOff.On,
                    PushMediaServerId = mediaServer.MediaServerId,
                    Vhost = session.Vhost,
                    StreamId = session.StreamId,
                    Schema = "like gb28181",
                };
                OrmService.Db.Insert<ClientOnOffLog>(tmpClientLog).ExecuteAffrows();
            }

            Logger.Logger.Info("流发布成功->" + mediaServer.MediaServerId + "->" + session.CameraId + "->" +
                               session.CameraType + "->" + session.Vhost + "->" + session.App + "->" +
                               session.StreamId + "->" + session.PlayUrl);
            return new ResToWebHookOnPublish() //返回zlmediakit成功
            {
                Code = 0,
                EnableHls = false,
                EnableMp4 = false,
                EnableRtxp = true,
                Msg = "success",
            };
        }


        public static ResToWebHookOnStreamChange OnStopNew(ReqForWebHookOnStop req, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var mediaServer = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(req.MediaServerId));
            if (mediaServer == null)
            {
                return new ResToWebHookOnStreamChange()
                {
                    Code = -1,
                    Msg = "failed",
                };
            }

            PlayerSession player = null;
            CameraSession camera = null;
            lock (Common.PlayerSessionListLock)
            {
                lock (Common.CameraSessionLock)
                {
                    switch (req.Player)
                    {
                        case true:

                            player = Common.PlayerSessions.FindLast(x => x.SessionId.Equals(req.Id)
                                                                         && x.ClientType ==
                                                                         ClientType.Player);


                            break;
                        case false:
                            camera = Common.CameraSessions.FindLast(x => x.Vhost.Equals(req.Vhost)
                                                                         && (x.ClientType ==
                                                                             ClientType.Camera ||
                                                                             x.ClientType ==
                                                                             ClientType.Livecast)
                                                                         && x.App.Equals(req.App) &&
                                                                         x.StreamId.Equals(req.Stream));
                            break;
                    }

                    if (req.Player == true && player != null)
                    {
                        ClientOnOffLog tmpClientLog = new ClientOnOffLog()
                        {
                            App = player.App,
                            CameraProtocolType = CameraType.None,
                            ClientType = player.ClientType,
                            CreateTime = DateTime.Now,
                            Ipaddress = req.Ip,
                            CameraId = "",
                            OnOff = OnOff.Off,
                            PushMediaServerId = mediaServer.MediaServerId,
                            Vhost = req.Vhost,
                            StreamId = req.Stream,
                            Schema = req.Schema,
                        };
                        OrmService.Db.Insert<ClientOnOffLog>(tmpClientLog).ExecuteAffrows();
                        lock (Common.PlayerSessionListLock)
                        {
                            Logger.Logger.Info("停止播放->" + mediaServer.MediaServerId + "->" + tmpClientLog.CameraId +
                                               "->" + tmpClientLog.Vhost + "->" + tmpClientLog.App + "->" +
                                               tmpClientLog.StreamId + "->" + req.Ip + "->" + req.Id + "->" +
                                               req.Duration + "->" + req.TotalBytes);

                            Common.PlayerSessions.Remove(player);
                        }
                    }
                    else if (req.Player == false && camera != null)
                    {
                        ClientOnOffLog tmpClientLog = new ClientOnOffLog()
                        {
                            App = req.App,
                            CameraProtocolType = camera.CameraType,
                            ClientType = camera.ClientType,
                            CreateTime = DateTime.Now,
                            Ipaddress = req.Ip,
                            CameraId = camera.ClientType == ClientType.Camera ? camera.CameraId : "",
                            OnOff = OnOff.Off,
                            PushMediaServerId = mediaServer.MediaServerId,
                            Vhost = req.Vhost,
                            StreamId = req.Stream,
                            Schema = req.Schema,
                        };
                        OrmService.Db.Insert<ClientOnOffLog>(tmpClientLog).ExecuteAffrows();
                        lock (Common.CameraSessionLock)
                        {
                            camera.IsOnline = false;
                            Logger.Logger.Info("流停止发布->" + mediaServer.MediaServerId + "->" + tmpClientLog.CameraId +
                                               "->" + camera.ClientType + "->" + tmpClientLog.Vhost + "->" +
                                               tmpClientLog.App + "->" + tmpClientLog.StreamId);
                        }
                    }
                }
            }

            return new ResToWebHookOnStreamChange()
            {
                Code = 0,
                Msg = "success",
            };
        }


        public static ResToWebHookOnStreamChange OnPlayNew(ReqForWebHookOnPlay req, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var mediaServer = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(req.MediaServerId));
            if (mediaServer == null)
            {
                return new ResToWebHookOnStreamChange()
                {
                    Code = -1,
                    Msg = "failed",
                };
            }

            CameraSession camera = null;
            lock (Common.CameraSessionLock)
            {
                camera = Common.CameraSessions.FindLast(x => x.App.Equals(req.App) &&
                                                             x.Vhost.Equals(req.Vhost) &&
                                                             x.StreamId.Equals(req.Stream) &&
                                                             (x.ClientType.Equals(ClientType.Camera) ||
                                                              x.ClientType.Equals(ClientType.Livecast)));
            }

            if (camera == null)
            {
                return new ResToWebHookOnStreamChange()
                {
                    Code = -1,
                    Msg = "not found stream",
                };
            }

            PlayerSession player = new PlayerSession()
            {
                CameraId = camera.CameraId,
                MediaServerId = camera.MediaServerId,
                ClientType = ClientType.Player,
                PlayUrl = "http://" + mediaServer.Ipaddress + ":" + mediaServer.MediaServerHttpPort +
                          "/" + req.App + "/" + req.Stream + ".flv",
                PlayerIp = req.Ip,
                UpTime = 0,
                OnlineTime = DateTime.Now,
                Vhost = req.Vhost,
                App = req.App,
                StreamId = req.Stream,
                SessionId = req.Id,
                MediaServerIp = mediaServer.Ipaddress,
            };
            lock (Common.PlayerSessionListLock)
            {
                Common.PlayerSessions.Add(player);
            }

            ClientOnOffLog tmpClientLog = new ClientOnOffLog()
            {
                App = player.App,
                CameraProtocolType = CameraType.None,
                ClientType = player.ClientType,
                CreateTime = DateTime.Now,
                Ipaddress = req.Ip,
                OnOff = OnOff.On,
                PushMediaServerId = mediaServer.MediaServerId,
                Vhost = player.Vhost,
                StreamId = player.StreamId,
            };
            OrmService.Db.Insert<ClientOnOffLog>(tmpClientLog).ExecuteAffrows();
            Logger.Logger.Info("开始播放->" + mediaServer.MediaServerId + "->" + player.CameraId + "->" + player.Vhost +
                               "->" + player.App + "->" + player.StreamId + "->" + req.Ip + "->" + req.Id + "->" +
                               player.PlayUrl);
            return new ResToWebHookOnStreamChange()
            {
                Code = 0,
                Msg = "success",
            };
        }


        /// <summary>
        /// 当有RTSP拉流事件时的处理
        /// </summary>
        /// <param name="req"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static ResToWebHookOnStreamChange OnStreamChangeNew(ReqForWebHookOnStreamChange req,
            out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var mediaServer = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(req.MediaServerId));
            if (mediaServer == null)
            {
                return new ResToWebHookOnStreamChange()
                {
                    Code = -1,
                    Msg = "failed",
                };
            }

            if (req.Regist == false && req.Schema == "rtsp")
            {
                OnStopNew(new ReqForWebHookOnStop()
                {
                    App = req.App,
                    Duration = null,
                    Id = null,
                    Ip = null,
                    MediaServerId = req.MediaServerId,
                    Params = null,
                    Player = false,
                    Port = null,
                    Schema = req.Schema,
                    Stream = req.Stream,
                    Vhost = req.Vhost,
                }, out rs);
            }

            if (req.Regist == true && req.Schema == "rtsp")
            {
                CameraSession checksession = null;
                lock (Common.CameraSessionLock)
                {
                    checksession = Common.CameraSessions.FindLast(x => x.App.Equals(req.App)
                                                                       && x.Vhost.Equals(req.Vhost) &&
                                                                       x.StreamId.Equals(req.Stream) &&
                                                                       x.MediaServerId.Equals(req.MediaServerId));
                }

                if (checksession == null)
                {
                    var ffmpegObj = mediaServer.WebApi.FFmpegProxies.FindLast(x => x.App.Equals(req.App)
                        && x.Vhost.Equals(req.Vhost) &&
                        x.StreamId.Equals(req.Stream));

                    if (ffmpegObj != null && ffmpegObj.MediaInfo != null && ffmpegObj.MediaInfo.Count > 0 &&
                        ffmpegObj.Data != null)
                    {
                        CameraInstance camera = null;
                        lock (Common.CameraInstanceListLock)
                        {
                            camera = Common.CameraInstanceList.FindLast(x =>
                                x.IfRtspUrl.Equals(ffmpegObj.Src_Url) && x.PushMediaServerId.Equals(req.MediaServerId));
                        }


                        if (camera != null)
                        {
                            CameraSession session = new CameraSession()
                            {
                                CameraId = camera.CameraId,
                                MediaServerId = camera.PushMediaServerId,
                                CameraName = camera.CameraName,
                                CameraDeptId = camera.DeptId,
                                CameraDeptName = camera.DeptName,
                                CameraPDeptId = camera.PDetpId,
                                CameraPDeptName = camera.PDetpName,
                                CameraType = camera.CameraType,
                                ClientType = ClientType.Camera,
                                CameraEx = new CameraEx()
                                {
                                    App = req.App,
                                    Camera = null,
                                    Ctype = CameraType.Rtsp,
                                    MediaServerId = mediaServer.MediaServerId,
                                    PushStreamSocketType = PushStreamSocketType.TCP,
                                    SipCameraStatus = null,
                                    StreamId = CRC32Cls.GetCRC32(ffmpegObj.Src_Url),
                                    StreamServerIp = mediaServer.Ipaddress,
                                    StreamServerPort = mediaServer.MediaServerHttpPort,
                                    Vhost = req.Vhost,
                                    InputUrl = ffmpegObj.Src_Url,
                                },
                                IsOnline = true,
                                IsRecord = false,
                                PlayUrl = ffmpegObj.Play_Url,
                                CameraIpAddress = camera.CameraIpAddress,
                                UpTime = 0,
                                OnlineTime = DateTime.Now,
                                Vhost = req.Vhost,
                                App = req.App,
                                StreamId = req.Stream,
                                MediaServerIp = mediaServer.Ipaddress,
                            };

                            lock (Common.CameraSessionLock)
                            {
                                Common.CameraSessions.Add(session);
                            }

                            ClientOnOffLog tmpClientLog = new ClientOnOffLog()
                            {
                                App = session.App,
                                CameraProtocolType = session.CameraType,
                                ClientType = session.ClientType,
                                CreateTime = DateTime.Now,
                                Ipaddress = session.CameraIpAddress,
                                CameraId = session.CameraId,
                                OnOff = OnOff.On,
                                PushMediaServerId = mediaServer.MediaServerId,
                                Vhost = session.Vhost,
                                StreamId = session.StreamId,
                                Schema = "rtsp",
                            };
                            Logger.Logger.Info("RTSP流发布->" + mediaServer.MediaServerId + "->" + tmpClientLog.CameraId +
                                               "->" + session.ClientType + "->" + tmpClientLog.Vhost + "->" +
                                               tmpClientLog.App + "->" + tmpClientLog.StreamId);

                            OrmService.Db.Insert<ClientOnOffLog>(tmpClientLog).ExecuteAffrows();
                        }
                        else
                        {
                            return new ResToWebHookOnStreamChange()
                            {
                                Code = -1,
                                Msg = "rtsp_Proxy not found",
                            };
                        }
                    }
                }
                else
                {
                    lock (Common.CameraSessionLock)
                    {
                        checksession.IsOnline = true;
                        checksession.OnlineTime = DateTime.Now;
                        checksession.UpTime = 0;
                    }

                    ClientOnOffLog tmpClientLog = new ClientOnOffLog()
                    {
                        App = checksession.App,
                        CameraProtocolType = checksession.CameraType,
                        ClientType = checksession.ClientType,
                        CreateTime = DateTime.Now,
                        Ipaddress = checksession.CameraIpAddress,
                        CameraId = checksession.CameraId,
                        OnOff = OnOff.On,
                        PushMediaServerId = mediaServer.MediaServerId,
                        Vhost = checksession.Vhost,
                        StreamId = checksession.StreamId,
                        Schema = "rtsp",
                    };
                    Logger.Logger.Info("RTSP流发布->" + mediaServer.MediaServerId + "->" + tmpClientLog.CameraId + "->" +
                                       checksession.ClientType + "->" + tmpClientLog.Vhost + "->" + tmpClientLog.App +
                                       "->" + tmpClientLog.StreamId);

                    OrmService.Db.Insert<ClientOnOffLog>(tmpClientLog).ExecuteAffrows();
                }
            }

            return new ResToWebHookOnStreamChange()
            {
                Code = 0,
                Msg = "success",
            };
        }

        public static ResToWebHookOnStreamChange OnRecordMp4Completed(ReqForWebHookOnRecordMp4Completed record,
            out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var mediaServer = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(record.MediaServerId));
            if (mediaServer == null)
            {
                return new ResToWebHookOnStreamChange()
                {
                    Code = -1,
                    Msg = "mediaserver not found."
                };
            }

            var st = Common.ConvertDateTimeToInt((long) record.Start_Time);
            DateTime currentTime = DateTime.Now;
            RecordFile tmpDvrVideo = new RecordFile();
            tmpDvrVideo.App = record.App;
            tmpDvrVideo.Vhost = record.Vhost;
            tmpDvrVideo.Streamid = record.Stream;
            tmpDvrVideo.FileSize = record.File_Size;
            tmpDvrVideo.DownloadUrl = record.Url;
            tmpDvrVideo.VideoPath = record.File_Path;
            tmpDvrVideo.StartTime = st;
            tmpDvrVideo.EndTime = st.AddSeconds((int) record.Time_Len);
            tmpDvrVideo.Duration = record.Time_Len;
            tmpDvrVideo.Undo = false;
            tmpDvrVideo.Deleted = false;
            tmpDvrVideo.PushMediaServerId = record.MediaServerId;
            tmpDvrVideo.UpdateTime = currentTime;
            tmpDvrVideo.RecordDate = st.ToString("yyyy-MM-dd");
            string tmp = tmpDvrVideo.DownloadUrl;
            tmp = tmp.Replace("\\", "/", StringComparison.Ordinal); //跨平台兼容
            if (tmp.Contains(":"))
            {
                tmp = tmp.Substring(tmp.IndexOf(':') + 1); //清除掉类似  c: 这样的字符，跨平台兼容
            }

            if (!string.IsNullOrEmpty(mediaServer.RecordFilePath) &&
                record.File_Path.Contains(mediaServer.RecordFilePath))
            {
                tmp = tmp.Replace(mediaServer.RecordFilePath, "", StringComparison.Ordinal);
                if (tmp.StartsWith("/"))
                {
                    tmp = tmp.TrimStart('/');
                }

                tmpDvrVideo.DownloadUrl = "http://" + mediaServer.Ipaddress + ":" + mediaServer.WebApiPort +
                                          "/CustomizedRecord/" + tmp;
            }
            else
            {
                //如果不包含自定义视频存储目录地址，就认为是默认地址
                if (!string.IsNullOrEmpty(tmp) && tmp.Contains("/record/"))
                {
                    tmp = tmp.Replace("/record/", "", StringComparison.Ordinal);
                }

                tmpDvrVideo.DownloadUrl = "http://" + mediaServer.Ipaddress + ":" + mediaServer.WebApiPort +
                                          "/CustomizedRecord/" + tmp;
            }

            CameraSession session = null;
            lock (Common.CameraSessionLock)
            {
                session = Common.CameraSessions.FindLast(x => x.App.Equals(record.App)
                                                              && x.StreamId.Equals(record.Stream) &&
                                                              x.Vhost.Equals(record.Vhost));
                if (session != null)
                {
                    tmpDvrVideo.CameraId = session.CameraId;
                    tmpDvrVideo.CameraIpAddress = session.CameraIpAddress;
                    tmpDvrVideo.CameraName = session.CameraName;
                    tmpDvrVideo.DeptId = session.CameraDeptId;
                    tmpDvrVideo.DeptName = session.CameraDeptName;
                    tmpDvrVideo.PDetpId = session.CameraPDeptId;
                    tmpDvrVideo.PDetpName = session.CameraPDeptName;
                }
            }

            OrmService.Db.Insert(tmpDvrVideo).ExecuteAffrows();
            Logger.Logger.Info("文件录制成功->" + mediaServer.MediaServerId + "->" + tmpDvrVideo.CameraId + "->" +
                               session.ClientType + "->" + tmpDvrVideo.Vhost + "->" + tmpDvrVideo.App + "->" +
                               tmpDvrVideo.Streamid + "->" + tmpDvrVideo.FileSize + "->" + tmpDvrVideo.VideoPath);

            return new ResToWebHookOnStreamChange()
            {
                Code = 0,
                Msg = "success"
            };
        }


        /// <summary>
        /// 有媒体服务器启动
        /// </summary>
        /// <param name="req"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static ResToWebHookOnStreamChange OnMediaServerStart(ZLMediaKitConfigForResponse req,
            out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var mediaServer = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(req.General_MediaServerId));
            if (mediaServer == null)
            {
                return new ResToWebHookOnStreamChange()
                {
                    Code = -1,
                    Msg = "failed",
                };
            }

            lock (mediaServer.Config)
            {
                mediaServer.Config = req;
            }

            return new ResToWebHookOnStreamChange()
            {
                Code = 0,
                Msg = "success",
            };
        }

        public static ResMediaServerWebApiReg ServerReg(ResMediaServerWebApiReg req, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (req != null)
            {
                var retObj = Common.MediaServerList.FindLast(x => x.Ipaddress.Equals(req.Ipaddress)
                                                                  && x.Secret.Equals(req.Secret) &&
                                                                  x.MediaServerId.Equals(req.MediaServerId));
                try
                {
                    if (retObj != null)
                    {
                        retObj.KeepAlive = DateTime.Now;
                        if (req.MediaServerSystemInfo != null)
                        {
                            retObj.MediaServerSystemInfo = req.MediaServerSystemInfo;
                        }

                        if (retObj.Config == null)
                        {
                            MediaServerApis.GetConfig(req.MediaServerId, out _); //配置信息不存在时获取配置信息 
                        }

                        return req;
                    }
                }
                catch
                {
                    // ignored
                    //为了防止MediaServer在其他地方被null掉，线程不同步情况下报错
                    return null;
                }

                MediaServerInstance msi = new MediaServerInstance(req.Ipaddress, req.WebApiServerhttpPort,
                    req.MediaServerHttpPort, req.Secret, req.MediaServerId, req.RecordFilePath,
                    req.MediaServerSystemInfo);
                msi.KeepAlive = DateTime.Now;

                lock (Common.MediaServerLock)
                {
                    Common.MediaServerList.Add(msi);
                }

                MediaServerApis.RestartMediaServer(req.MediaServerId, out _); //首次注册时重启服务
                //  MediaServerApis.GetConfig(req.MediaServerId, out _); //首次注册获取配置信息
                return req;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.FunctionInputParamsError,
                Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError],
            };
            return null;
        }
    }
}