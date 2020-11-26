using System.Collections.Generic;
using System.Threading;
using CommonFunction.ManageStructs;
using CommonFunctions;
using CommonFunctions.DBStructs;
using CommonFunctions.ManageStructs;
using CommonFunctions.WebApiStructs.Request;
using CommonFunctions.WebApiStructs.Response;
using GB28181.Servers.SIPMonitor;
using GB28181.Sys.Model;

namespace StreamNodeCtrlApis.SipGateApis
{
    /// <summary>
    /// sip网关的一般接口
    /// </summary>
    public static class CommonApi
    {
        /// <summary>
        /// 停掉直播流
        /// </summary>
        /// <param name="mediaServerDeviceId"></param>
        /// <param name="deviceId"></param>
        /// <param name="cameraid"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static ResLiveVideoResponse ByeLiveVideo(string mediaServerDeviceId, string deviceId, string cameraid,
            out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (string.IsNullOrEmpty(mediaServerDeviceId) || string.IsNullOrEmpty(deviceId) ||
                string.IsNullOrEmpty(cameraid))
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.FunctionInputParamsError,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError],
                };
                return null;
            }

            if (Common.SipProcess.SipDeviceList == null || Common.SipProcess.SipDeviceList.Count == 0)
            {
                return null;
            }

            if (Common.MediaServerList == null || Common.MediaServerList.Count == 0)
            {
                return null;
            }

            var mediaServer = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(mediaServerDeviceId));
            if (mediaServer == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServerInstancesNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServerInstancesNotFound],
                };
                return null;
            }

            var dev = Common.SipProcess.SipDeviceList.FindLast(x => x.DeviceId.Equals(deviceId));
            if (dev == null || dev.CameraExList == null || dev.CameraExList.Count == 0)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SipDeviceOrCameraNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SipDeviceOrCameraNotFound],
                };
                return null;
            }

            var camera = dev.CameraExList.FindLast(x => x.Camera.DeviceID.Equals(cameraid));
            if (camera == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SipDeviceOrCameraNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SipDeviceOrCameraNotFound],
                };
                return null;
            }

            var result = new ResLiveVideoResponse()
            {
                MediaServerId = "",
                Deivceid = dev.DeviceId,
                Cameraid = cameraid,
                DeviceIpaddress = dev.IpAddress,
                MediaId = "",
                MediaServerIpaddress = "",
                MediaServerPort = 0,
                Vhost = "",
                App = "",
                PushStreamSocketType = null,
            };

            /*
            CameraInstance cameraInst = null;
            lock (Common.CameraInstanceListLock)
            {
                cameraInst = Common.CameraInstanceList.FindLast(x =>
                    x.CameraDeviceLable.Equals(dev.DeviceId) && x.CameraChannelLable.Equals(cameraid) &&
                    x.PushMediaServerId.Equals(mediaServerDeviceId));
            }
            */

            string streamid=dev.IpAddress+dev.DeviceId + camera.Camera.DeviceID ;
            /*if (cameraInst != null && cameraInst.MobileCamera == true)
            {
                streamid = cameraInst.MobileCamera + dev.DeviceId + camera.Camera.DeviceID + mediaServerDeviceId;
            }
            else
            {
                streamid = dev.IpAddress + dev.DeviceId + camera.Camera.DeviceID + mediaServerDeviceId;
            }*/

            uint stid = CRC32Cls.GetCRC32(streamid);
            string mediaStreamId = string.Format("{0:X8}", stid);
            ReqZLMediaKitCloseRtpPort req = new ReqZLMediaKitCloseRtpPort()
            {
                Stream_Id = mediaStreamId,
            };
            var apiRet = mediaServer.WebApi.CloseRtpPort(req, out rs);
            if (camera.SipCameraStatus == SipCameraStatus.RealVideo)
            {
                var ret = Common.SipProcess.ReqStopLive(cameraid);
                if (ret)
                {
                    camera.MediaServerId = "";
                    return result;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SipRealVideoExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SipRealVideoExcept],
                };
                return null;
            }


            return result;
        }


        /// <summary>
        /// 请求直播流
        /// </summary>
        /// <param name="mediaServerDeviceId"></param>
        /// <param name="deviceId"></param>
        /// <param name="cameraid"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static ResLiveVideoResponse LiveVideo(string mediaServerDeviceId, string deviceId, string cameraid,
            out ResponseStruct rs, bool tcp = false)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (string.IsNullOrEmpty(mediaServerDeviceId) || string.IsNullOrEmpty(deviceId) ||
                string.IsNullOrEmpty(cameraid))
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.FunctionInputParamsError,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError],
                };
                return null;
            }

            if (Common.SipProcess.SipDeviceList == null || Common.SipProcess.SipDeviceList.Count == 0)
            {
                return null;
            }

            if (Common.MediaServerList == null || Common.MediaServerList.Count == 0)
            {
                return null;
            }

            var mediaServer = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(mediaServerDeviceId));
            if (mediaServer == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServerInstancesNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServerInstancesNotFound],
                };
                return null;
            }

            var dev = Common.SipProcess.SipDeviceList.FindLast(x => x.DeviceId.Equals(deviceId));
            if (dev == null || dev.CameraExList == null || dev.CameraExList.Count == 0)
            {
                try
                {
                    //如果出现设备目录为空时，自动获取一次设备目录,等下次再推流
                    ActiveDeviceCatalogQuery(dev.DeviceId, out _);
                }
                catch
                {
                    //
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SipDeviceOrCameraNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SipDeviceOrCameraNotFound],
                };
                return null;
            }

            var camera = dev.CameraExList.FindLast(x => x.Camera.DeviceID.Equals(cameraid));
            if (camera == null)
            {
                try
                {
                    //如果出现设备目录为空时，自动获取一次设备目录,等下次再推流
                    ActiveDeviceCatalogQuery(dev.DeviceId, out _);
                }
                catch
                {
                    // 
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SipDeviceOrCameraNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SipDeviceOrCameraNotFound],
                };
                return null;
            }

            try
            {
                ByeLiveVideo(mediaServerDeviceId, deviceId, cameraid, out rs); //请求前先停一下直播
            }
            catch
            {
                //
            }

            Thread.Sleep(500);
            var result = new ResLiveVideoResponse()
            {
                MediaServerId = mediaServerDeviceId,
                Deivceid = dev.DeviceId,
                Cameraid = camera.Camera.DeviceID,
                DeviceIpaddress = dev.IpAddress,
                MediaServerIpaddress = camera.StreamServerIp,
                MediaServerPort = camera.StreamServerPort,
                MediaId = string.Format("{0:X8}", camera.StreamId),
                Vhost = camera.Vhost,
                App = camera.App,
                PushStreamSocketType = tcp == true ? PushStreamSocketType.TCP : PushStreamSocketType.UDP,
            };
            if (camera.SipCameraStatus == SipCameraStatus.Idle)
            {
                CameraInstance cameraInst = null;
                /*lock (Common.CameraInstanceListLock)
                {
                    cameraInst = Common.CameraInstanceList.FindLast(x =>
                        x.CameraDeviceLable.Equals(dev.DeviceId) && x.CameraChannelLable.Equals(cameraid) &&
                        x.PushMediaServerId.Equals(mediaServerDeviceId));
                }*/

                string streamid =dev.IpAddress+ dev.DeviceId + camera.Camera.DeviceID ;
                /*if (cameraInst != null && cameraInst.MobileCamera == true)
                {
                    streamid = cameraInst.MobileCamera + dev.DeviceId + camera.Camera.DeviceID + mediaServerDeviceId;
                }
                else
                {
                    streamid = dev.IpAddress + dev.DeviceId + camera.Camera.DeviceID + mediaServerDeviceId;
                }*/

                uint stid = CRC32Cls.GetCRC32(streamid);
                
                
                
                
                string mediaStreamId = string.Format("{0:X8}", stid);
                ReqZLMediaKitOpenRtpPort req = new ReqZLMediaKitOpenRtpPort()
                {
                    Stream_Id = mediaStreamId,
                };
                var retOpenRtpPort = mediaServer.WebApi.OpenRtpPort(req, out rs); //先开端口
                if (retOpenRtpPort != null && rs.Code == ErrorNumber.None)
                {
                    //再进行推流请求
                    var ret = Common.SipProcess.ReqLiveForCreateRptPort(cameraid, mediaServer.Ipaddress,
                        (ushort) retOpenRtpPort.Port, tcp);
                    if (ret)
                    {
                        camera.MediaServerId = mediaServerDeviceId;
                        var obj = new ResLiveVideoResponse()
                        {
                            MediaServerId = mediaServerDeviceId,
                            Deivceid = dev.DeviceId,
                            Cameraid = camera.Camera.DeviceID,
                            DeviceIpaddress = dev.IpAddress,
                            MediaServerIpaddress = mediaServer.Ipaddress,
                            MediaServerPort = (ushort) retOpenRtpPort.Port,
                            MediaId = mediaStreamId,
                            Vhost = camera.Vhost,
                            App = camera.App,
                            Play_Url = "http://" + mediaServer.Ipaddress + ":" + mediaServer.MediaServerHttpPort + "/" +
                                       camera.App + "/" + mediaStreamId + ".flv",
                            PushStreamSocketType = tcp == true ? PushStreamSocketType.TCP : PushStreamSocketType.UDP,
                        };
                        return obj;
                    }

                    //以下是：如果摄像头返回推流失败，再判断一次流媒体的onpublish返回中是否存在，如果存在则一样返回成功
                    CameraSession session = null;
                    lock (Common.CameraSessionLock)
                    {
                        session = Common.CameraSessions.FindLast(x =>
                            x.Vhost.Equals("__defaultVhost__")
                            && x.App.Equals("rtp") &&
                            x.StreamId.Equals(mediaStreamId));
                    }

                    if (session != null)
                    {
                        camera.MediaServerId = mediaServerDeviceId;
                        var obj = new ResLiveVideoResponse()
                        {
                            MediaServerId = mediaServerDeviceId,
                            Deivceid = dev.DeviceId,
                            Cameraid = camera.Camera.DeviceID,
                            DeviceIpaddress = dev.IpAddress,
                            MediaServerIpaddress = mediaServer.Ipaddress,
                            MediaServerPort = (ushort) retOpenRtpPort.Port,
                            MediaId = string.Format("{0:X8}", camera.StreamId),
                            Vhost = camera.Vhost,
                            App = camera.App,
                            PushStreamSocketType =
                                tcp == true ? PushStreamSocketType.TCP : PushStreamSocketType.UDP,
                        };
                        session.IsOnline = true;
                        session.ForceOffline = false;
                        return obj;
                    }

                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SipRealVideoExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SipRealVideoExcept],
                    };
                    return null;
                }
            }

            return result;
        }


        /// <summary>
        /// 主动获取一次设备目录列表
        /// </summary>
        /// <param name="deviceid"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool ActiveDeviceCatalogQuery(string deviceid, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var right = Common.SipProcess.GetDeviceList(deviceid);
            if (right == false)
            {
                Common.SipProcess.TickOutDevice(deviceid);
            }

            return right;
        }

        /// <summary>
        /// 获取自动推流状态
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool GetAutoPushStreamState(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            return Common.SipProcess.AutoPushStream;
        }

        /// <summary>
        /// 设置自动推流状态
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool SetAutoPushStreamState(bool state, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            Common.SipProcess.AutoPushStream = state;
            return true;
        }

       /// <summary>
       /// ptz控制
       /// </summary>
       /// <param name="deviceid"></param>
       /// <param name="dir"></param>
       /// <param name="speed"></param>
       /// <param name="rs"></param>
       /// <param name="channelId">当需要对多通道设备进行ptz控制时，在此传通道id(NVR对接时要用到)</param>
       /// <returns></returns>
        public static bool PtzControl(string deviceid, string dir, int speed, out ResponseStruct rs,string channelId="")
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (Common.SipProcess.SipDeviceList == null || Common.SipProcess.SipDeviceList.Count == 0)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SipDeviceOrCameraNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SipDeviceOrCameraNotFound],
                };
                return false;
            }

            var dev = Common.SipProcess.SipDeviceList.FindLast(x => x.DeviceId.Equals(deviceid));
           
            if (dev != null)
            {
                string cmd = dir.Trim().ToLower();
                if (cmd.Equals("stop") ||
                    cmd.Equals("up") ||
                    cmd.Equals("down") ||
                    cmd.Equals("left") ||
                    cmd.Equals("right") ||
                    cmd.Equals("upleft") ||
                    cmd.Equals("upright") ||
                    cmd.Equals("downleft") ||
                    cmd.Equals("downright") ||
                    cmd.Equals("focus1") || //聚焦+
                    cmd.Equals("focus2") || //聚焦-
                    cmd.Equals("zoom1") || //变倍+
                    cmd.Equals("zoom2") || //变倍-
                    cmd.Equals("iris1") || //光圈开  
                    cmd.Equals("iris2") //光圈关  
                    /*cmd.Equals("setpreset") || //设置预设位(不支持)
                    cmd.Equals("getpreset") || //调用预设位(不支持)    
                    cmd.Equals("removepreset") //删除预设位(不支持)   */
                )
                {
                    var ret = Common.SipProcess.ReqPtzControl(deviceid, cmd, speed,channelId);
                    if (ret)
                    {
                        return true;
                    }
                }
                else
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SipPtzControlCmdUnsupported,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SipPtzControlCmdUnsupported],
                    };
                }
            }
            else
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SipDeviceOrCameraNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SipDeviceOrCameraNotFound],
                };
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.SipPtzContorlExcept,
                Message = ErrorMessage.ErrorDic![ErrorNumber.SipPtzContorlExcept],
            };
            return false;
        }


        /// <summary>
        /// 获取sip设备列表
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static List<SipDevice> GetSipDeviceList(string deviceId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (string.IsNullOrEmpty(deviceId))
            {
                return Common.SipProcess.SipDeviceList;
            }

            var obj = Common.SipProcess.SipDeviceList.FindLast(x => x.DeviceId.Equals(deviceId));
            List<SipDevice> tmpList = new List<SipDevice>();
            if (obj != null)
            {
                tmpList.Add(obj);
            }

            return tmpList;
        }
    }
}