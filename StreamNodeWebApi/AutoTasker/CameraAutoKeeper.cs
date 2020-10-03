using System;
using System.Threading;
using CommonFunction.ManageStructs;
using CommonFunctions;
using CommonFunctions.DBStructs;
using CommonFunctions.ManageStructs;
using CommonFunctions.WebApiStructs.Request;
using CommonFunctions.WebApiStructs.Response;
using GB28181.Sys.Model;
using LibGB28181SipGate;
using StreamNodeCtrlApis.SipGateApis;
using StreamNodeCtrlApis.SystemApis;

namespace StreamNodeWebApi.AutoTasker
{
    public class CameraAutoKeeper
    {
        public static bool Init = false;


        /// <summary>
        /// 获取摄像头session
        /// </summary>
        /// <param name="cil"></param>
        /// <returns></returns>
        private static CameraSession getCameraCurrentSession(CameraInstance cil)
        {
            try
            {
                CameraSession session = null;
                if (cil.CameraType == CameraType.GB28181)
                {
                    lock (Common.CameraSessionLock)
                    {
                        if (cil.MobileCamera == true) //如果是移动摄像头，就不再检查ip地址
                        {
                            session = Common.CameraSessions.FindLast(x =>
                                x.ClientType == ClientType.Camera
                                && x.CameraType == CameraType.GB28181 &&
                                x.CameraEx.Camera.DeviceID.Equals(cil.CameraChannelLable) &&
                                x.CameraEx.Camera.ParentID.Equals(cil.CameraDeviceLable));
                        }
                        else
                        {
                            session = Common.CameraSessions.FindLast(x =>
                                x.ClientType == ClientType.Camera
                                && x.CameraType == CameraType.GB28181 &&
                                x.CameraIpAddress.Equals(cil.CameraIpAddress) //为支持公网远程设备的ip不固定性，取消ip 地址校验
                                && x.CameraEx.Camera.DeviceID.Equals(cil.CameraChannelLable) &&
                                x.CameraEx.Camera.ParentID.Equals(cil.CameraDeviceLable));
                        }
                    }
                }


                if (cil.CameraType == CameraType.Rtsp) //rtsp摄像头一定是固定ip地址的
                {
                    lock (Common.CameraSessionLock)
                    {
                        session = Common.CameraSessions.FindLast(x =>
                            x.ClientType == ClientType.Camera
                            && x.CameraType == CameraType.Rtsp &&
                            x.CameraIpAddress.Equals(cil.CameraIpAddress)
                            && x.CameraEx.InputUrl.Equals(cil.IfRtspUrl));
                    }
                }

                return session;
            }
            catch (Exception ex)
            {
                Console.WriteLine("getCameraCurrentSession Except:\r\n" + ex.Message + "\r\n" + ex.StackTrace);
                return null;
            }
        }


        /// <summary>
        /// 启动gb28181摄像头
        /// </summary>
        /// <param name="cil"></param>
        private static void liveGB28181(CameraInstance cil)
        {
            try
            {
                ResponseStruct rs = null;
                var gbRet = CommonApi.LiveVideo(cil.PushMediaServerId,
                    cil.CameraDeviceLable,
                    cil.CameraChannelLable, out rs, (bool) cil.IfGb28181Tcp!);

                if (gbRet != null && rs.Code == ErrorNumber.None)
                {
                    Console.WriteLine("GB28181推流成功->" + cil.CameraId + "->" + cil.CameraDeviceLable + "->" +
                                      cil.CameraChannelLable+"->"+"(TCP:"+cil.IfGb28181Tcp+")");
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
                            sessionsub.CameraId = cil.CameraId;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("GB28181推流失败->" + cil.CameraId + "->" + cil.CameraDeviceLable + "->" +
                                      cil.CameraChannelLable + "->" +"(TCP:"+cil.IfGb28181Tcp+")->"+ JsonHelper.ToJson(rs));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("liveGB28181 Except:\r\n" + ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// 启动rtsp摄像头
        /// </summary>
        /// <param name="cil"></param>
        private static void liveRtsp(CameraInstance cil)
        {
            try
            {
                ResponseStruct rs = null;
                var mediaObj =
                    Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(cil.PushMediaServerId));

                if (mediaObj == null || mediaObj.IsRunning == false)
                {
                    return;
                }

                bool useFFmpeg = true;

                if (cil.RetryTimes < 2)
                {
                    //rtsp方式，先跳过三次，因为zlmediakit会自动维护掉线的ffmpeg,要延迟处理一下，不然会重复创建ffmpeg
                    cil.RetryTimes++;
                    return;
                }

                cil.RetryTimes = 0;
                ResZLMediaKitAddFFmpegProxy ret = null;
                if (useFFmpeg)
                {
                    ret = MediaServerApis.AddFFmpegProxy(
                        mediaObj.MediaServerId,
                        cil.IfRtspUrl, out rs);
                }
                else
                {
                    ret = MediaServerApis.AddStreamProxy(mediaObj.MediaServerId,
                        cil.IfRtspUrl, out rs);
                }

                if (ret != null && rs.Code == ErrorNumber.None)
                {
                 Console.WriteLine("Rtsp推流成功->" + cil.CameraId + "->" + cil.IfRtspUrl);
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
                            sessionsub.CameraId = cil.CameraId;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Rtsp推流失败->" + cil.CameraId + "->" +cil.IfRtspUrl+"->" + JsonHelper.ToJson(rs));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("liveRtsp Except:\r\n" + ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// 停止rtsp摄像头
        /// </summary>
        /// <param name="session"></param>
        private static void stopRtsp(CameraSession session)
        {
            try
            {
                ResponseStruct rs = null;
                var mediaObj =
                    Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(session.MediaServerId));

                if (mediaObj == null || mediaObj.IsRunning == false)
                {
                    return;
                }

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
                if (ret.Code == 0)
                {
                    Console.WriteLine("Rtsp结束成功->" + session.CameraId + "->" + session.CameraEx.InputUrl);
                }
                else
                {
                    Console.WriteLine("Rtsp结束失败->" + session.CameraId + "->" + session.CameraEx.InputUrl);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("stopRtsp Except:\r\n" + ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// 停止gb28181摄像头
        /// </summary>
        /// <param name="session"></param>
        private static void stopGB28181(CameraSession session)
        {
            try
            {
                ResponseStruct rs = null;
                var mediaObj =
                    Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(session.MediaServerId));

                if (mediaObj == null || mediaObj.IsRunning == false)
                {
                    return;
                }

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
                if (ret3.Code == 0)
                {
                    Console.WriteLine("GB28181结束成功->" + session.CameraId + "->" + session.CameraEx.Camera.ParentID + "->" +
                                      session.CameraEx.Camera.DeviceID );
                }
                else
                {
                    Console.WriteLine("GB28181结束失败->" +  session.CameraId + "->" + session.CameraEx.Camera.ParentID + "->" +
                                      session.CameraEx.Camera.DeviceID+"->"+ JsonHelper.ToJson(rs));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("stopGB28181 Except:\r\n" + ex.Message + "\r\n" + ex.StackTrace);
            }
        }


        /// <summary>
        /// 停止摄像头
        /// </summary>
        /// <param name="cil"></param>
        private static void stopCamera(CameraInstance cil)
        {
            try
            {
                if (cil != null && cil.EnableLive == false)
                {
                    CameraSession cameraSession = null;
                    cameraSession = getCameraCurrentSession(cil);

                    if (cameraSession != null)
                    {
                        switch (cil.CameraType)
                        {
                            case CameraType.Rtsp:

                                stopRtsp(cameraSession);
                                break;
                            case CameraType.GB28181:

                                stopGB28181(cameraSession);
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("stopCamera Except:\r\n" + ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// 启动摄像头
        /// </summary>
        /// <param name="cil"></param>
        private static void liveCamera(CameraInstance cil)
        {
            try
            {
                CameraSession cameraSession = null;
                if (cil != null && cil.EnableLive)
                {
                    cameraSession = getCameraCurrentSession(cil);

                    if (cameraSession == null || (cameraSession != null && cameraSession.IsOnline == false)
                    ) //camera没有，或者isonline是false要推流
                    {
                        CameraType ctype = cameraSession != null ? cameraSession.CameraType : cil.CameraType;
                        switch (ctype)
                        {
                            case CameraType.Rtsp:

                                liveRtsp(cil);

                                break;
                            case CameraType.GB28181:

                                liveGB28181(cil);

                                break;
                        }
                    }
                    else if (cameraSession != null && string.IsNullOrEmpty(cameraSession.CameraId)
                                                   && cameraSession.IsOnline == true
                    ) //当camera不为空，但camera.cameraid为空时，不需要重新推，但要补全这个id
                    {
                        CameraSession sessionsub = null;
                        lock (Common.CameraSessionLock)
                        {
                            sessionsub = Common.CameraSessions.FindLast(x =>
                                x.App!.Equals(cameraSession.App)
                                && x.Vhost!.Equals(cameraSession.Vhost) &&
                                x.StreamId!.Equals(cameraSession.StreamId));
                        }


                        if (sessionsub != null)
                        {
                            lock (Common.CameraSessionLock)
                            {
                                cameraSession.CameraId = cil.CameraId;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("liveCamera Except:\r\n" + ex.Message + "\r\n" + ex.StackTrace);
            }
        }


        /// <summary>
        /// 监控摄像头状态，启动或停止
        /// </summary>
        private static void keeperCamera()
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

                    if (Common.CameraInstanceList == null || Common.CameraInstanceList.Count <= 0)
                    {
                        Thread.Sleep(5000);
                        continue;
                    }

                    foreach (var cit in Common.CameraInstanceList)
                    {
                        if (cit != null && cit.EnableLive && cit.Activated==true) //启动摄像头,必须是activated为true时才能启动
                        {
                            liveCamera(cit);
                        }

                        if (cit != null && (cit.EnableLive == false || cit.Activated==false)) //停止摄像头,如果activated为False,就一定要停止
                        {
                            stopCamera(cit);
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
                    keeperCamera();
                }
                catch (Exception ex)
                {
                    //
                }
            })).Start();
        }
    }
}