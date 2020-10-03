using System;
using System.Net;
using CommonFunctions;
using CommonFunctions.ManageStructs;
using CommonFunctions.WebApiStructs.Request;
using CommonFunctions.WebApiStructs.Response;
using LibGB28181SipGate;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StreamNodeCtrlApis.WebHookApis;
using Swashbuckle.AspNetCore.Annotations;

namespace StreamNodeWebApi.Controllers
{
    /// <summary>
    /// Sip网关类接口
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


        [Route("OnStreamNoneReader")]
        [HttpPost]
        [Log]
        [AuthVerify]
        public ResToWebHookOnStreamNoneReader OnStreamNoneReader(ReqForWebHookOnStreamNoneReader req)
        {
            Console.WriteLine("OnStreamNoneReader:\r\n" + JsonHelper.ToJson(req));
            return new ResToWebHookOnStreamNoneReader()
            {
                Code = 0,
                Close = false,
            };
        }

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
            string[] tmpStrArr = str.Split('\n', StringSplitOptions.RemoveEmptyEntries);
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
            Console.WriteLine("reg:\r\n"+JsonHelper.ToJson(req));
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