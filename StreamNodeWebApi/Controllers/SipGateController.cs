#nullable enable
using System.Collections.Generic;
using CommonFunctions;
using CommonFunctions.ManageStructs;
using CommonFunctions.WebApiStructs.Response;
using GB28181.Sys.Model;
using LibGB28181SipGate;
using Microsoft.AspNetCore.Mvc;
using StreamNodeCtrlApis.SipGateApis;
using Swashbuckle.AspNetCore.Annotations;

namespace StreamNodeWebApi.Controllers
{
    /// <summary>
    /// Sip网关类接口
    /// </summary>
    [ApiController]
    [Route("/SipGate")]
    [SwaggerTag("Sip网关类接口")]
    public class SipGateController : ControllerBase
    {
        /// <summary>
        /// 获取Sip网关自动推流状态
        /// </summary>
        /// <param name="deviceid"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("ActiveDeviceCatalogQuery")]
        [HttpGet]
        [Log]
        [AuthVerify]
        public bool ActiveDeviceCatalogQuery(string deviceid)
        {
            ResponseStruct rs;
            var ret = CommonApi.ActiveDeviceCatalogQuery(deviceid, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }

        /// <summary>
        /// 获取Sip网关自动推流状态
        /// </summary>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("GetAutoPushStreamState")]
        [HttpGet]
        [Log]
        [AuthVerify]
        public bool GetAutoPushStreamState()
        {
            ResponseStruct rs;
            var ret = CommonApi.GetAutoPushStreamState(out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }

        /// <summary>
        /// 设置Sip网关自动推流状态
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("SetAutoPushStreamState")]
        [HttpGet]
        [Log]
        [AuthVerify]
        public bool SetAutoPushStreamState(bool state)
        {
            ResponseStruct rs;
            var ret = CommonApi.SetAutoPushStreamState(state, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }

        /// <summary>
        /// 请求实时视频
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <param name="deviceid"></param>
        /// <param name="cameraid"></param>
        /// <param name="tcp"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("LiveVideo")]
        [HttpGet]
        [Log]
        [AuthVerify]
        public ResLiveVideoResponse LiveVideo(string mediaServerId, string deviceid, string cameraid, bool tcp = false)
        {
            ResponseStruct rs;
            var ret = CommonApi.LiveVideo(mediaServerId, deviceid, cameraid, out rs,
                tcp);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }


        /// <summary>
        /// 停止实时视频预览
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <param name="deviceid"></param>
        /// <param name="cameraid"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("ByeLiveVideo")]
        [HttpGet]
        [Log]
        [AuthVerify]
        public ResLiveVideoResponse ByeLiveVideo(string mediaServerId, string deviceid, string cameraid)
        {
            ResponseStruct rs;
            var ret = CommonApi.ByeLiveVideo(mediaServerId, deviceid, cameraid, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }


        /// <summary>
        /// 获取已注册的设备列表
        /// </summary>
        /// <param name="deviceid"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("GetSipDeviceList")]
        [HttpGet]
        [Log]
        [AuthVerify]
        public List<SipDevice> GetSipDeviceList(string? deviceid)
        {
            ResponseStruct rs;
            var ret = CommonApi.GetSipDeviceList(deviceid, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }


        /// <summary>
        /// ptz控制
        /// </summary>
        /// <param name="deviceid"></param>
        /// <param name="dir"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("PtzControl")]
        [HttpGet]
        [Log]
        [AuthVerify]
        public bool PtzControl(string deviceid, string dir, int speed)
        {
            ResponseStruct rs;

            if (speed > 255) speed = 255;
            if (speed <= 0) speed = 1;

            var ret = CommonApi.PtzControl(deviceid, dir, speed, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }
    }
}