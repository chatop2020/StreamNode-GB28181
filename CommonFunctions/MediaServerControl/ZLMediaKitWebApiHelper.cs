using System;
using System.Collections.Generic;
using System.Threading;
using CommonFunctions.ManageStructs;
using CommonFunctions.WebApiStructs.Request;
using CommonFunctions.WebApiStructs.Response;
using GB28181.Servers.SIPMonitor;
using LibGB28181SipGate;

namespace CommonFunctions.MediaServerControl
{
    public class ZLMediaKitWebApiHelper
    {
        private string serverip;
        private ushort serverport;
        private string secret;
        private string url;
        private List<ResZLMediaKitAddFFmpegProxy> _fFmpegProxies = new List<ResZLMediaKitAddFFmpegProxy>();

        public string Serverip
        {
            get => serverip;
            set => serverip = value;
        }

        public ushort Serverport
        {
            get => serverport;
            set => serverport = value;
        }

        public string Secret
        {
            get => secret;
            set => secret = value;
        }

        public List<ResZLMediaKitAddFFmpegProxy> FFmpegProxies => _fFmpegProxies;

        public ZLMediaKitWebApiHelper(string serverip, ushort serverport, string secret)
        {
            this.serverip = serverip;
            this.serverport = serverport;
            this.secret = secret;
            url = "http://" + serverip + ":" + serverport + "/index/api/";
        }


        public ResZLMediaKitRtpPortList GetRtpPortList(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            ReqZLMediaKitRtpPortList req = new ReqZLMediaKitRtpPortList();
            req.Secret = secret;
            string innerUrl = url + "listRtpServer";
            string reqData = JsonHelper.ToJson(req);
            var httpRet = NetHelper.HttpPostRequest(innerUrl, null, reqData, "utf-8", 60000);
            if (!string.IsNullOrEmpty(httpRet))
            {
                try
                {
                    var resZlMediaKitRtpPortList = JsonHelper.FromJson<ResZLMediaKitRtpPortList>(httpRet);
                    if (resZlMediaKitRtpPortList != null)
                    {
                        return resZlMediaKitRtpPortList;
                    }

                    var resError = JsonHelper.FromJson<ResZLMediaKitErrorResponse>(httpRet);
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.ZLMediaKitHttpWebApiExcept,
                        Message = httpRet,
                    };
                    return null;
                }
                catch
                {
                    var resError = JsonHelper.FromJson<ResZLMediaKitErrorResponse>(httpRet);
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.ZLMediaKitHttpWebApiExcept,
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
        /// 关闭动态rtp端口
        /// </summary>
        /// <param name="req"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public ResZLMediaKitCloseRtpPort CloseRtpPort(ReqZLMediaKitCloseRtpPort req, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            req.Secret = secret;
            string innerUrl = url + "closeRtpServer";
            string reqData = JsonHelper.ToJson(req);
            var httpRet = NetHelper.HttpPostRequest(innerUrl, null, reqData, "utf-8", 60000);
            if (!string.IsNullOrEmpty(httpRet))
            {
                try
                {
                    var resZlMediaKitCloseRtp = JsonHelper.FromJson<ResZLMediaKitCloseRtpPort>(httpRet);
                    if (resZlMediaKitCloseRtp != null)
                    {
                        return resZlMediaKitCloseRtp;
                    }

                    var resError = JsonHelper.FromJson<ResZLMediaKitErrorResponse>(httpRet);
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.ZLMediaKitHttpWebApiExcept,
                        Message = httpRet,
                    };
                    return null;
                }
                catch
                {
                    var resError = JsonHelper.FromJson<ResZLMediaKitErrorResponse>(httpRet);
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.ZLMediaKitHttpWebApiExcept,
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
        /// 生成动态rtp端口
        /// </summary>
        /// <param name="req"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public ResZLMediaKitOpenRtpPort OpenRtpPort(ReqZLMediaKitOpenRtpPort req, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            req.Secret = secret;
            req.Port = 0;
            req.Enable_Tcp = true;
            string innerUrl = url + "openRtpServer";
            string reqData = JsonHelper.ToJson(req);
            var httpRet = NetHelper.HttpPostRequest(innerUrl, null, reqData, "utf-8", 60000);
            if (!string.IsNullOrEmpty(httpRet))
            {
                try
                {
                    var resZlMediaKitOpenRtp = JsonHelper.FromJson<ResZLMediaKitOpenRtpPort>(httpRet);
                    if (resZlMediaKitOpenRtp != null)
                    {
                        return resZlMediaKitOpenRtp;
                    }

                    var resError = JsonHelper.FromJson<ResZLMediaKitErrorResponse>(httpRet);
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.ZLMediaKitHttpWebApiExcept,
                        Message = httpRet,
                    };
                    return null;
                }
                catch
                {
                    var resError = JsonHelper.FromJson<ResZLMediaKitErrorResponse>(httpRet);
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.ZLMediaKitHttpWebApiExcept,
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
        /// 获取录制状态
        /// </summary>
        /// <param name="req"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public ResZLMediaKitIsRecord GetRecordStatus(ReqZLMediaKitStopRecord req, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            req.Secret = secret;
            req.Type = 1;
            string innerUrl = url + "isRecording";
            string reqData = JsonHelper.ToJson(req);
            var httpRet = NetHelper.HttpPostRequest(innerUrl, null, reqData, "utf-8", 60000);
            if (!string.IsNullOrEmpty(httpRet))
            {
                try
                {
                    var resIsRecord = JsonHelper.FromJson<ResZLMediaKitIsRecord>(httpRet);
                    if (resIsRecord != null)
                    {
                        return resIsRecord;
                    }

                    var resError = JsonHelper.FromJson<ResZLMediaKitErrorResponse>(httpRet);
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.ZLMediaKitHttpWebApiExcept,
                        Message = httpRet,
                    };
                    return null;
                }
                catch
                {
                    var resError = JsonHelper.FromJson<ResZLMediaKitErrorResponse>(httpRet);
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.ZLMediaKitHttpWebApiExcept,
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
        /// 停止录制
        /// </summary>
        /// <param name="serverip"></param>
        /// <param name="serverport"></param>
        /// <param name="secret"></param>
        public ResZLMediaKitStartStopRecord StopRecord(ReqZLMediaKitStopRecord req, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            req.Secret = secret;
            req.Type = 1;
            string innerUrl = url + "stopRecord";
            string reqData = JsonHelper.ToJson(req);
            var httpRet = NetHelper.HttpPostRequest(innerUrl, null, reqData, "utf-8", 60000);
            if (!string.IsNullOrEmpty(httpRet))
            {
                try
                {
                    var resStart_StopRecord = JsonHelper.FromJson<ResZLMediaKitStartStopRecord>(httpRet);
                    if (resStart_StopRecord != null)
                    {
                        return resStart_StopRecord;
                    }

                    var resError = JsonHelper.FromJson<ResZLMediaKitErrorResponse>(httpRet);
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.ZLMediaKitHttpWebApiExcept,
                        Message = httpRet,
                    };
                    return null;
                }
                catch
                {
                    var resError = JsonHelper.FromJson<ResZLMediaKitErrorResponse>(httpRet);
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.ZLMediaKitHttpWebApiExcept,
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
        /// 启动录制
        /// </summary>
        /// <param name="serverip"></param>
        /// <param name="serverport"></param>
        /// <param name="secret"></param>
        public ResZLMediaKitStartStopRecord StartRecord(ReqZLMediaKitStartRecord req, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            req.Secret = secret;
            req.Type = 1;
            string innerUrl = url + "startRecord";
            string reqData = JsonHelper.ToJson(req);
            var httpRet = NetHelper.HttpPostRequest(innerUrl, null, reqData, "utf-8", 60000);
            if (!string.IsNullOrEmpty(httpRet))
            {
                try
                {
                    var resStart_StopRecord = JsonHelper.FromJson<ResZLMediaKitStartStopRecord>(httpRet);
                    if (resStart_StopRecord != null)
                    {
                        return resStart_StopRecord;
                    }

                    var resError = JsonHelper.FromJson<ResZLMediaKitErrorResponse>(httpRet);
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.ZLMediaKitHttpWebApiExcept,
                        Message = httpRet,
                    };
                    return null;
                }
                catch
                {
                    var resError = JsonHelper.FromJson<ResZLMediaKitErrorResponse>(httpRet);
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.ZLMediaKitHttpWebApiExcept,
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
        /// 获取流媒体列表
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public ResZLMediaKitMediaList GetMediaList(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var reqParams = new ReqZLMediaKitRequestBase();
            reqParams.Secret = secret;
            string innerUrl = url + "getMediaList";
            string reqData = JsonHelper.ToJson(reqParams);
            var httpRet = NetHelper.HttpPostRequest(innerUrl, null, reqData, "utf-8", 5000);
            if (!string.IsNullOrEmpty(httpRet))
            {
                try
                {
                    var resZlMediaList = JsonHelper.FromJson<ResZLMediaKitMediaList>(httpRet);
                    if (resZlMediaList != null)
                    {
                        return resZlMediaList;
                    }

                    var resError = JsonHelper.FromJson<ResZLMediaKitErrorResponse>(httpRet);
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.ZLMediaKitHttpWebApiExcept,
                        Message = httpRet,
                    };
                    return null;
                }
                catch
                {
                    var resError = JsonHelper.FromJson<ResZLMediaKitErrorResponse>(httpRet);
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.ZLMediaKitHttpWebApiExcept,
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
        /// 重启流媒体服务器
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public ResZLMediaKitErrorResponse RestartMediaServer(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var reqParams = new ReqZLMediaKitRequestBase();
            reqParams.Secret = secret;
            string innerUrl = url + "restartServer";
            string reqData = JsonHelper.ToJson(reqParams);
            var httpRet = NetHelper.HttpPostRequest(innerUrl, null, reqData, "utf-8", 5000);
            if (!string.IsNullOrEmpty(httpRet))
            {
                try
                {
                    return JsonHelper.FromJson<ResZLMediaKitErrorResponse>(httpRet);
                }
                catch
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.Other,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.Other],
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
        /// 专用于关掉ffmpegproxy的流
        /// </summary>
        /// <param name="req"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        private ResZLMediaKitCloseStreams CloseFFmpegProxy(ReqZLMediaKitCloseStreams req, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            ResZLMediaKitAddFFmpegProxy foundObj = null;
            foreach (var obj in _fFmpegProxies)
            {
                if (!string.IsNullOrEmpty(req.App) && !string.IsNullOrEmpty(obj.App) && req.App.Equals(obj.App)
                    && !string.IsNullOrEmpty(req.Stream) && !string.IsNullOrEmpty(obj.StreamId) &&
                    req.Stream.Equals(obj.StreamId))
                {
                    if (!string.IsNullOrEmpty(req.Vhost) && obj.MediaInfo != null && obj.MediaInfo.Count > 0)
                    {
                        var tmpObj = obj.MediaInfo.FindLast(x => x.Vhost != null && x.Vhost.Equals(req.Vhost));
                        if (tmpObj != null)
                        {
                            foundObj = obj;
                            break;
                        }
                    }
                    else
                    {
                        foundObj = obj;
                        break;
                    }
                }
            }

            if (foundObj == null || foundObj.Data == null)
                return null;


            ReqZLMediaKitDelFFmpegSource newReq = new ReqZLMediaKitDelFFmpegSource();
            newReq.Secret = secret;
            newReq.Key = foundObj.Data.Key;

            string innerUrl = url + "delFFmpegSource";
            string reqData = JsonHelper.ToJson(newReq);
            var httpRet = NetHelper.HttpPostRequest(innerUrl, null, reqData, "utf-8", 5000);
            if (!string.IsNullOrEmpty(httpRet))
            {
                try
                {
                    var resZlMediaKitDelFFMpegSource = JsonHelper.FromJson<ResZLMeidaKitDelFfMpegSource>(httpRet);
                    if (resZlMediaKitDelFFMpegSource != null)
                    {
                        ResZLMediaKitCloseStreams resultNew = new ResZLMediaKitCloseStreams();
                        resultNew.Count_Closed = 1;
                        resultNew.Count_Hit = 1;
                        resultNew.Code = 0;
                        resultNew.MediaInfo.AddRange(foundObj.MediaInfo);
                        lock (_fFmpegProxies)
                        {
                            _fFmpegProxies.Remove(foundObj);
                        }


                        return resultNew;
                    }

                    var resError = JsonHelper.FromJson<ResZLMediaKitErrorResponse>(httpRet);
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.ZLMediaKitHttpWebApiExcept,
                        Message = httpRet,
                    };
                    return null;
                }
                catch
                {
                    var resError = JsonHelper.FromJson<ResZLMediaKitErrorResponse>(httpRet);
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.ZLMediaKitHttpWebApiExcept,
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
        /// 关闭一个流
        /// </summary>
        /// <param name="req"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public ResZLMediaKitCloseStreams CloseStreams(ReqZLMediaKitCloseStreams req, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var retFFmpegClose = CloseFFmpegProxy(req, out rs);
            if (rs.Code == ErrorNumber.None && retFFmpegClose != null)
            {
                return retFFmpegClose;
            }

            req.Secret = secret;
            string innerUrl = url + "close_streams";
            string reqData = JsonHelper.ToJson(req);
            var oldMediaList = GetMediaList(out _);
            var httpRet = NetHelper.HttpPostRequest(innerUrl, null, reqData, "utf-8", 5000);
            if (!string.IsNullOrEmpty(httpRet))
            {
                try
                {
                    var resZlMediaKitCloseStreams = JsonHelper.FromJson<ResZLMediaKitCloseStreams>(httpRet);
                    if (resZlMediaKitCloseStreams != null)
                    {
                        if (oldMediaList != null && oldMediaList.Data != null && oldMediaList.Data.Count > 0)
                        {
                            var tmp = oldMediaList.Data;
                            resZlMediaKitCloseStreams.MediaInfo.AddRange(tmp.FindAll(x => x.Vhost.Equals(req.Vhost)
                                && x.App.Equals(req.App) &&
                                x.Stream.Equals(req.Stream) &&
                                x.Schema.Equals(req.Schema)));
                        }

                        return resZlMediaKitCloseStreams;
                    }

                    var resError = JsonHelper.FromJson<ResZLMediaKitErrorResponse>(httpRet);
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.ZLMediaKitHttpWebApiExcept,
                        Message = httpRet,
                    };
                    return null;
                }
                catch
                {
                    var resError = JsonHelper.FromJson<ResZLMediaKitErrorResponse>(httpRet);
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.ZLMediaKitHttpWebApiExcept,
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
        /// 添加ffmpeg代理的视频流
        /// </summary>
        /// <param name="src_url"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public ResZLMediaKitAddFFmpegProxy TryAddRtspProxy(string src_url, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            ResZLMediaKitAddFFmpegProxy result = null;
            int i = 0;
            do
            {
                try
                {
                    i++;
                    result = AddRtspProxy(src_url, out rs);
                }
                catch
                {
                }
            } while ((result == null || result.Code != 0) && i < 2); //做两次尝试

            Thread.Sleep(2000); //这里稍等一会，等mediainfo里的tracks出来后再继续
            if (result != null && result.Code == 0)
            {
                lock (_fFmpegProxies)
                {
                    var obj = _fFmpegProxies.FindLast(x => x.Data.Key.Equals(result.Data.Key));
                    if (obj == null)
                    {
                        ResponseStruct rs2;
                        var httpRet = GetMediaList(out rs2);
                        if (rs2.Code == ErrorNumber.None)
                        {
                            var finder = httpRet.Data.FindAll(x => x.App.Equals(result.App) &&
                                                                   x.Stream.Equals(result.StreamId));
                            if (finder != null && finder.Count > 0)
                            {
                                foreach (var o in finder)
                                {
                                    result.MediaInfo.Add(o);
                                }
                            }
                            else
                            {
                                rs = new ResponseStruct()
                                {
                                    Code = ErrorNumber.Other,
                                    Message = ErrorMessage.ErrorDic![ErrorNumber.Other],
                                };
                                return null;
                            }
                        }

                        _fFmpegProxies.Add(result);
                    }
                    else
                    {
                        ResponseStruct rs2;
                        var httpRet = GetMediaList(out rs2);
                        if (rs2.Code == ErrorNumber.None)
                        {
                            var finder = httpRet.Data.FindAll(x => x.App.Equals(result.App) &&
                                                                   x.Stream.Equals(result.StreamId));
                            if (finder != null && finder.Count > 0)
                            {
                                result.MediaInfo.Clear();
                                foreach (var o in finder)
                                {
                                    result.MediaInfo.Add(o);
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 添加一个ZLMediaKit内部的Rtsp拉流代理
        /// </summary>
        /// <param name="src_url"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public ResZLMediaKitAddStreamProxy AddStreamProxy(string src_url, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var reqParams = new ReqZLMediaKitAddStreamProxy
            {
                App = "rtsp_proxy",
                Enable_Hls = true,
                Enable_Mp4 = true,
                Enable_Rtmp = true,
                Enable_Rtsp = true,
                Rtp_Type = 0,
                Stream = string.Format("{0:X8}", CRC32Cls.GetCRC32(src_url)),
                Url = src_url,
                Vhost = "__defaultVhost__",
            };
            reqParams.Secret = secret;
            string innerUrl = url + "addStreamProxy";
            string reqData = JsonHelper.ToJson(reqParams);
            var httpRet = NetHelper.HttpPostRequest(innerUrl, null, reqData, "utf-8", 60000);
            if (!string.IsNullOrEmpty(httpRet))
            {
                try
                {
                    var resAddStreamProxy = JsonHelper.FromJson<ResZLMediaKitAddStreamProxy>(httpRet);
                    if (resAddStreamProxy != null)
                    {
                        resAddStreamProxy.App = reqParams.App;
                        resAddStreamProxy.Httpserverport = serverport;
                        resAddStreamProxy.Serverip = serverip;
                        resAddStreamProxy.Src_Url = reqParams.Url;
                        resAddStreamProxy.StreamId = reqParams.Stream;
                        resAddStreamProxy.UpdateTime = DateTime.Now;
                        resAddStreamProxy.Vhost = "__defaultVhost__";
                        return resAddStreamProxy;
                    }

                    var resError = JsonHelper.FromJson<ResZLMediaKitErrorResponse>(httpRet);
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.ZLMediaKitHttpWebApiExcept,
                        Message = httpRet,
                    };
                    return null;
                }
                catch
                {
                    var resError = JsonHelper.FromJson<ResZLMediaKitErrorResponse>(httpRet);
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.ZLMediaKitHttpWebApiExcept,
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

        private ResZLMediaKitAddFFmpegProxy AddRtspProxy(string src_url, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var reqParams = new ReqZLMediaKitAddFFmpegProxy(src_url, "127.0.0.1");
            reqParams.Secret = secret;
            string innerUrl = url + "addFFmpegSource";
            string reqData = JsonHelper.ToJson(reqParams);
            var httpRet = NetHelper.HttpPostRequest(innerUrl, null, reqData, "utf-8", 60000);
            if (!string.IsNullOrEmpty(httpRet))
            {
                try
                {
                    var resAddRtspProxy = JsonHelper.FromJson<ResZLMediaKitAddFFmpegProxy>(httpRet);
                    if (resAddRtspProxy != null)
                    {
                        resAddRtspProxy.App = reqParams.App;
                        resAddRtspProxy.Httpserverport = serverport;
                        resAddRtspProxy.Serverip = serverip;
                        resAddRtspProxy.Src_Url = reqParams.Src_Url;
                        resAddRtspProxy.StreamId = reqParams.StreamId;
                        resAddRtspProxy.UpdateTime = DateTime.Now;
                        resAddRtspProxy.Vhost = "__defaultVhost__";
                        return resAddRtspProxy;
                    }

                    var resError = JsonHelper.FromJson<ResZLMediaKitErrorResponse>(httpRet);
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.ZLMediaKitHttpWebApiExcept,
                        Message = httpRet,
                    };
                    return null;
                }
                catch
                {
                    var resError = JsonHelper.FromJson<ResZLMediaKitErrorResponse>(httpRet);
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.ZLMediaKitHttpWebApiExcept,
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
        /// 获取流媒体配置信息
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public ResZLMediaKitConfig GetConfig(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var reqParams = new ReqZLMediaKitGetSystemConfig();
            reqParams.Secret = this.secret;
            string innerUrl = url + "getServerConfig";
            string reqData = JsonHelper.ToJson(reqParams);
            var httpRet = NetHelper.HttpPostRequest(innerUrl, null, reqData, "utf-8", 5000);
            if (!string.IsNullOrEmpty(httpRet))
            {
                try
                {
                    string[] tmpStrArr = httpRet.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                    httpRet = "";
                    foreach (var tmpstr in tmpStrArr)
                    {
                        if (!string.IsNullOrEmpty(tmpstr) && !tmpstr.Trim().StartsWith("\"."))
                        {
                            httpRet += tmpstr + "\r\n";
                        }
                    }

                    var resConfig = JsonHelper.FromJson<ResZLMediaKitConfig>(httpRet);
                    if (resConfig != null)
                    {
                        return resConfig;
                    }

                    var resError = JsonHelper.FromJson<ResZLMediaKitErrorResponse>(httpRet);
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.ZLMediaKitHttpWebApiExcept,
                        Message = httpRet,
                    };
                    return null;
                }
                catch
                {
                    var resError = JsonHelper.FromJson<ResZLMediaKitErrorResponse>(httpRet);
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.ZLMediaKitHttpWebApiExcept,
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
    }
}