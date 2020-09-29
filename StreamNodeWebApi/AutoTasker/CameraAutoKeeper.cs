using System;
using System.Threading;
using CommonFunction.ManageStructs;
using CommonFunctions;
using CommonFunctions.DBStructs;
using CommonFunctions.ManageStructs;
using CommonFunctions.MediaServerControl;
using CommonFunctions.WebApiStructs.Request;
using CommonFunctions.WebApiStructs.Response;
using GB28181.Sys.Model;
using StreamNodeCtrlApis.SipGateApis;
using StreamNodeCtrlApis.SystemApis;

namespace StreamNodeWebApi.AutoTasker
{
    public class CameraAutoKeeper
    {
        public static bool Init = false;

        /// <summary>
        /// 5秒一次获取摄像头注册信息
        /// </summary>
        private static void getCameraInstanceList()
        {
            int i = 0;
            while (true)
            {
                i++;
                try
                {
                    if (i == 1 || i % 2 == 0)
                    {
                        lock (Common.CameraInstanceListLock)
                        {
                            Common.CameraInstanceList.Clear();
                            Common.CameraInstanceList.AddRange(OrmService.Db.Select<CameraInstance>().Where("1=1")
                                .ToList());
                        }
                    }

                    if (Common.CameraInstanceList != null && Common.CameraInstanceList.Count > 0)
                        //循环列表
                        foreach (var cit in Common.CameraInstanceList)
                        {
                            Thread.Sleep(100);
                            if (cit != null && cit.EnableLive) //如果cit的是启动状态
                            {
                                Thread.Sleep(1000);
                                var mediaObj =
                                    Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(cit.PushMediaServerId));
                                if (mediaObj != null && mediaObj.IsRunning)
                                {
                                    CameraSession session = null;
                                    try
                                    {
                                        if (cit.CameraType == CameraType.GB28181)
                                        {
                                            lock (Common.CameraSessionLock)
                                            {
                                                session = Common.CameraSessions.FindLast(x =>
                                                    x.ClientType == ClientType.Camera
                                                    && x.CameraType == CameraType.GB28181 &&
                                                    x.CameraIpAddress.Equals(cit.CameraIpAddress)
                                                    && x.CameraEx.Camera.DeviceID.Equals(cit.CameraChannelLable) &&
                                                    x.CameraEx.Camera.ParentID.Equals(cit.CameraDeviceLable));
                                            }
                                        }

                                        if (cit.CameraType == CameraType.Rtsp)
                                        {
                                            lock (Common.CameraSessionLock)
                                            {
                                                session = Common.CameraSessions.FindLast(x =>
                                                    x.ClientType == ClientType.Camera
                                                    && x.CameraType == CameraType.Rtsp &&
                                                    x.CameraIpAddress.Equals(cit.CameraIpAddress)
                                                    && x.CameraEx.InputUrl.Equals(cit.IfRtspUrl));
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        continue;
                                    }

                                    bool useFFmpeg = true;
                                    ResponseStruct rs;
                                    if (session == null || session.IsOnline == false) //如果没有，就启动
                                    {
                                        switch (cit.CameraType)
                                        {
                                            case CameraType.Rtsp: //rtsp的启动方式
                                                if (cit.RetryTimes < 2)
                                                {
                                                    //rtsp方式，先跳过三次，因为zlmediakit会自动维护掉线的ffmpeg,要延迟处理一下，不然会重复创建ffmpeg
                                                    cit.RetryTimes++;
                                                    continue;
                                                }

                                                cit.RetryTimes = 0;
                                                ResZLMediaKitAddFFmpegProxy ret = null;
                                                if (useFFmpeg)
                                                {
                                                    ret = MediaServerApis.AddFFmpegProxy(
                                                        mediaObj.MediaServerId,
                                                        cit.IfRtspUrl, out rs);
                                                }
                                                else
                                                {
                                                    ret = MediaServerApis.AddStreamProxy(mediaObj.MediaServerId,
                                                        cit.IfRtspUrl, out rs);
                                                }

                                                if (ret != null && rs.Code == ErrorNumber.None)
                                                {
                                                    CameraSession sessionsub = null;
                                                    lock (Common.CameraSessionLock)
                                                    {
                                                        sessionsub = Common.CameraSessions.FindLast(x =>
                                                            x.App!.Equals(ret.App)
                                                            && x.Vhost!.Equals(ret.Vhost) &&
                                                            x.StreamId!.Equals(ret.StreamId));
                                                    }

                                                    if (sessionsub != null)
                                                    {
                                                        lock (Common.CameraSessionLock)
                                                        {
                                                            sessionsub.CameraId = cit.CameraId;
                                                        }
                                                    }
                                                }

                                                break;
                                            case CameraType.GB28181: //28181的启动方式

                                                var gbRet = CommonApi.LiveVideo(cit.PushMediaServerId,
                                                    cit.CameraDeviceLable,
                                                    cit.CameraChannelLable, out rs, (bool) cit.IfGb28181Tcp!);
                                                if (gbRet != null && rs.Code == ErrorNumber.None)
                                                {
                                                    CameraSession sessionsub = null;
                                                    lock (Common.CameraSessionLock)
                                                    {
                                                        sessionsub = Common.CameraSessions.FindLast(x =>
                                                            x.App!.Equals(gbRet.App)
                                                            && x.Vhost!.Equals(gbRet.Vhost) &&
                                                            x.StreamId!.Equals(gbRet.MediaId));
                                                    }

                                                    if (sessionsub != null)
                                                    {
                                                        lock (Common.CameraSessionLock)
                                                        {
                                                            sessionsub.CameraId = cit.CameraId;
                                                        }
                                                    }
                                                }

                                                break;
                                        }
                                    }
                                    else if (session != null && string.IsNullOrEmpty(session.CameraId))
                                    {
                                        CameraSession sessionsub = null;
                                        lock (Common.CameraSessionLock)
                                        {
                                            sessionsub = Common.CameraSessions.FindLast(x =>
                                                x.App!.Equals(session.App)
                                                && x.Vhost!.Equals(session.Vhost) &&
                                                x.StreamId!.Equals(session.StreamId));
                                        }

                                        if (sessionsub != null)
                                        {
                                            lock (Common.CameraSessionLock)
                                            {
                                                sessionsub.CameraId = cit.CameraId;
                                            }
                                        }
                                    }
                                }
                            }
                            else if (cit.EnableLive == false) //如果非启动的状态，就要停掉现有的
                            {
                                MediaServerInstance mediaObj = null;
                                lock (Common.MediaServerList)
                                {
                                    mediaObj =
                                        Common.MediaServerList.FindLast(x =>
                                            x.MediaServerId.Equals(cit.PushMediaServerId));
                                }

                                if (mediaObj != null && mediaObj.IsRunning)
                                {
                                    CameraSession session = null;
                                    try
                                    {
                                        if (cit.CameraType == CameraType.GB28181)
                                        {
                                            lock (Common.CameraSessionLock)
                                            {
                                                session = Common.CameraSessions.FindLast(x =>
                                                    x.ClientType == ClientType.Camera
                                                    && x.CameraType == CameraType.GB28181 &&
                                                    x.CameraIpAddress.Equals(cit.CameraIpAddress)
                                                    && x.CameraEx.Camera.DeviceID.Equals(cit.CameraChannelLable) &&
                                                    x.CameraEx.Camera.ParentID.Equals(cit.CameraDeviceLable));
                                            }
                                        }

                                        if (cit.CameraType == CameraType.Rtsp)
                                        {
                                            lock (Common.CameraSessionLock)
                                            {
                                                session = Common.CameraSessions.FindLast(x =>
                                                    x.ClientType == ClientType.Camera
                                                    && x.CameraType == CameraType.Rtsp &&
                                                    x.CameraIpAddress.Equals(cit.CameraIpAddress)
                                                    && x.CameraEx.InputUrl.Equals(cit.IfRtspUrl));
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        continue;
                                    }


                                    ResponseStruct rs;
                                    if (session != null)
                                    {
                                        switch (cit.CameraType)
                                        {
                                            case CameraType.Rtsp:
                                                var req = new ReqZLMediaKitCloseStreams()
                                                {
                                                    App = session.App,
                                                    Force = true,
                                                    Schema = "rtmp",
                                                    Secret = "",
                                                    Stream = session.StreamId,
                                                    Vhost = session.Vhost,
                                                };
                                                var ret = MediaServerApis.CloseStreams(mediaObj.MediaServerId, req,
                                                    out rs);
                                                break;
                                            case CameraType.GB28181:
                                                var req2 = new ReqZLMediaKitCloseStreams()
                                                {
                                                    App = session.App,
                                                    Force = true,
                                                    Schema = "rtmp",
                                                    Secret = "",
                                                    Stream = session.StreamId,
                                                    Vhost = session.Vhost,
                                                };
                                                var ret2 = MediaServerApis.CloseStreams(mediaObj.MediaServerId, req2,
                                                    out rs);
                                                var req3 = new ReqZLMediaKitCloseRtpPort()
                                                {
                                                    Secret = "",
                                                    Stream_Id = session.StreamId,
                                                };
                                                var ret3 = MediaServerApis.CloseRtpPort(mediaObj.MediaServerId, req3,
                                                    out rs);
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("报错了：\r\n" + ex.Message + "\r\n" + ex.StackTrace);
                    continue; //
                }

                Thread.Sleep(5000);
            }
        }

        public CameraAutoKeeper()
        {
            new Thread(new ThreadStart(delegate
            {
                try
                {
                    getCameraInstanceList();
                }
                catch (Exception ex)
                {
                    //
                }
            })).Start();
        }
    }
}