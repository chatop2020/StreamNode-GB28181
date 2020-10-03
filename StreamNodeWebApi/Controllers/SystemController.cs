using System.Collections.Generic;
using CommonFunctions;
using CommonFunctions.ManageStructs;
using CommonFunctions.MediaServerControl;
using CommonFunctions.WebApiStructs.Response;
using LibGB28181SipGate;
using Microsoft.AspNetCore.Mvc;
using StreamNodeCtrlApis.SystemApis;
using Swashbuckle.AspNetCore.Annotations;

namespace StreamNodeWebApi.Controllers
{
    /// <summary>
    /// 系统接口类
    /// </summary>
    [ApiController]
    [Route("/System")]
    [SwaggerTag("系统相关接口")]
    public class SystemController : ControllerBase
    {
        

        /// <summary>
        /// 获取全局的系统信息
        /// </summary>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("GetGlobleSystemInfo")]
        [HttpGet]
        [Log]
        [AuthVerify]
        public ResGlobleSystemInfo GetGlobleSystemInfo()
        {
            ResponseStruct rs;
            var ret = MediaServerApis.GetGlobleSystemInfo( out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }
        
        /// <summary>
        /// 获取一个流媒体服务的实例
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("GetMediaServerInstance")]
        [HttpGet]
        [Log]
        [AuthVerify]
        public MediaServerInstance GetMediaServerInstance(string mediaServerId)
        {
            ResponseStruct rs;
            var ret = MediaServerApis.GetMediaServerInstance(mediaServerId, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }

        /// <summary>
        /// 获取流媒体服务器列表 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("GetMediaServerList")]
        [HttpGet]
        [Log]
        [AuthVerify]
        public List<MediaServerInstance> GetMediaServerList()
        {
            ResponseStruct rs;
            var ret = MediaServerApis.GetMediaServerList(out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }
    }
}