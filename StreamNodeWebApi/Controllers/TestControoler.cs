using System;
using System.Collections.Generic;
using CommonFunction.ManageStructs;
using CommonFunctions;
using CommonFunctions.ManageStructs;
using CommonFunctions.WebApiStructs.Response;
using GB28181.Sys.Model;
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
    [Route("/Test")]
    [SwaggerTag("测试接口")]
    public class TestController : ControllerBase
    {
        [Route("GetSystemInfo")]
        [HttpPost]
        [Log]
        [AuthVerify]
        public ResGetSystemInfo TestGetSystemInfo()
        {
            return Common.SystemInfo;
        }

        [Route("GetTask")]
        [HttpPost]
        [Log]
        [AuthVerify]
        public void GetTask(CutMergeTaskResponse response)
        {
            if (response != null)
            {
                Logger.Logger.Debug("收到task参数 ->" + JsonHelper.ToJson(response));
              
            }
        }

        /// <summary>
        /// 测试AddStreamProxy接口
        /// </summary>
        /// <param name="deviceid"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("AddStreamProxy")]
        [HttpGet]
        [Log]
        [AuthVerify]
        public ResZLMediaKitAddStreamProxy AddStreamProxy(string deviceid, string url)
        {
            ResponseStruct rs;
            var ret = MediaServerApis.AddStreamProxy(deviceid, url, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }


        /// <summary>
        /// 获取sip设备列表 
        /// </summary>
        /// <returns></returns>
        /// 
        [Route("GetSipDeviceList")]
        [HttpGet]
        [Log]
        [AuthVerify]
        public List<SipDevice> GetSipDeviceList()
        {
            return Common.SipProcess.SipDeviceList;
        }


        /// <summary>
        /// 获取所有摄像头列表
        /// </summary>
        /// <returns></returns>
        [Route("GetAllCameraSession")]
        [HttpGet]
        [Log]
        [AuthVerify]
        public List<CameraSession> GetAllCameraSession()
        {
            return Common.CameraSessions;
        }
    }
}