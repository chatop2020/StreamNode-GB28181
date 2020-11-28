using System;
using System.Net;
using CommonFunction.ManageStructs;
using CommonFunctions;
using CommonFunctions.ManageStructs;
using CommonFunctions.WebApiStructs.Request;
using CommonFunctions.WebApiStructs.Response;
using LibGB28181SipGate;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StreamNodeCtrlApis.SystemApis;
using StreamNodeCtrlApis.WebHookApis;
using Swashbuckle.AspNetCore.Annotations;

namespace StreamNodeWebApi.Controllers
{
    /// <summary>
    /// Webhook类接口
    /// ZLMediaKit的config.ini上配置接口的相关地址，在ZLMeidakit发生一些重要事件时将通过这些地址回调StreamNode-GB28181
    /// 在收到ZLMediaKit的Webhook回调后，在此做相关的处理，比如有流发布，有播放者开始播放，流停止发布等事件，都在此处理
    /// 除了给ZLMediaKit提供Webhook回调方法以外，同时还提供给StreamNodeKeeper流媒体治理服务自注册接口，StreamNodeKeeper启动后
    /// 自动向Webhook接口上报其自己的信息，接口根据Commonn.MediaServerList中是否存在此流媒体信息为依据将此请求做为添加流媒体服务器
    /// 或做为心跳处理
    /// 在Sip设备上线时，此接口类提供接收未知Sip设备的自动数据库写入功能，以完成Sip设备、通道自动发现的功能，此类自动发现设备需要通过激活接口
    /// 对其进行激活以获取此类设备的合法身份，以便后续流程正常使用。
    /// </summary>
    [ApiController]
    [Route("/WebHook")]
    [SwaggerTag("WebHook相关接口类")]
    public class WebHookController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// 注入httpcontext
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        public WebHookController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }


        /// <summary>
        /// 当有新设备注册时（目录查询到时），自动写入摄像头数据库
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("OnSipDeviceRegister")]
        [HttpPost]
        [Log]
        [AuthVerify]
        public bool OnSipDeviceRegister(ReqAddCameraInstance req)
        {
            ResponseStruct rs;
            var ret = MediaServerApis.AddSipDeviceToDB(req, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }

        /// <summary>
        /// 录制文件完成时
        /// </summary>
        /// <returns></returns>
        [Route("OnRecordMp4Completed")]
        [HttpPost]
        [Log]
        [AuthVerify]
        public ResToWebHookOnStreamChange OnRecordMp4Completed(ReqForWebHookOnRecordMp4Completed record)
        {
            ResponseStruct rs;
            var ret = MediaServerCtrlApi.OnRecordMp4Completed(record, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }


        /// <summary>
        /// 当有流被发布时
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("OnPublish")]
        [HttpPost]
        [Log]
        [AuthVerify]
        public ResToWebHookOnPublish OnPublish(ReqForWebHookOnPublish req)
        {
            ResponseStruct rs;
            var ret = MediaServerCtrlApi.OnPublishNew(req, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }


        /// <summary>
        /// 当流状态改变时
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("OnStreamChange")]
        [HttpPost]
        [Log]
        [AuthVerify]
        public ResToWebHookOnStreamChange OnStreamChange(ReqForWebHookOnStreamChange req)
        {
            ResponseStruct rs;
            var ret = MediaServerCtrlApi.OnStreamChangeNew(req, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }


        /// <summary>
        /// 当有流无播放者时
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("OnStreamNoneReader")]
        [HttpPost]
        [Log]
        [AuthVerify]
        public ResToWebHookOnStreamNoneReader OnStreamNoneReader(ReqForWebHookOnStreamNoneReader req)
        {
            lock (Common.PlayerSessionListLock)
            {
                for (int i = Common.PlayerSessions.Count - 1; i >= 0; i--)
                {
                    if (Common.PlayerSessions[i] != null &&
                        Common.PlayerSessions[i].App!.Equals(req.App) &&
                        Common.PlayerSessions[i].Vhost!.Equals(req.Vhost)
                        && Common.PlayerSessions[i].StreamId!
                            .Equals(req.Stream) && Common.PlayerSessions[i]
                            .MediaServerId!.Equals(req.MediaServerId))
                    {
                        Common.PlayerSessions[i] = null!;
                        break;
                    }
                }

                Common.RemoveNull(Common.PlayerSessions);
            }

            return new ResToWebHookOnStreamNoneReader()
            {
                Code = 0,
                Close = false,
            };
        }

        /// <summary>
        /// 当有停止事件时
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("OnStop")]
        [HttpPost]
        [Log]
        [AuthVerify]
        public ResToWebHookOnStreamChange OnStop(ReqForWebHookOnStop req)
        {
            ResponseStruct rs;
            var ret = MediaServerCtrlApi.OnStopNew(req, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }


        /// <summary>
        /// 当有播放事件时
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("OnPlay")]
        [HttpPost]
        [Log]
        [AuthVerify]
        public ResToWebHookOnStreamChange OnPlay(ReqForWebHookOnPlay req)
        {
            ResponseStruct rs;
            var ret = MediaServerCtrlApi.OnPlayNew(req, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }


        /// <summary>
        /// 流媒体服务启动事件
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("OnMediaServerStart")]
        [HttpPost]
        [Log]
        [AuthVerify]
        public ResToWebHookOnStreamChange OnMediaServerStart(Object req)
        {
            var str = Convert.ToString(req);
            string[] tmpStrArr = str!.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            str = "";
            foreach (var tmpstr in tmpStrArr)
            {
                if (!string.IsNullOrEmpty(tmpstr) && !tmpstr.Trim().StartsWith("\"."))
                {
                    str += tmpstr + "\r\n";
                }
            }


            var tmpObj = JsonHelper.FromJson<ZLMediaKitConfigForResponse>(str);


            ResponseStruct rs;
            var ret = MediaServerCtrlApi.OnMediaServerStart(tmpObj, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }


        /// <summary>
        /// 用于流媒体控制器注册到控制中心
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("MediaServerRegister")]
        [HttpPost]
        [Log]
        [AuthVerify]
        public ResMediaServerWebApiReg MediaServerRegister(ResMediaServerWebApiReg req)
        {
            ResponseStruct rs;
            if (string.IsNullOrEmpty(req.Ipaddress))
            {
                req.Ipaddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                IPAddress thisip = IPAddress.Parse(req.Ipaddress);
                req.Ipaddress = thisip.MapToIPv4().ToString();
            }


            var ret = MediaServerCtrlApi.ServerReg(req, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }
    }
}