using System.Collections.Generic;
using CommonFunctions;
using CommonFunctions.DBStructs;
using CommonFunctions.ManageStructs;
using CommonFunctions.WebApiStructs.Request;
using CommonFunctions.WebApiStructs.Response;
using LibGB28181SipGate;
using Microsoft.AspNetCore.Mvc;
using StreamNodeCtrlApis.SystemApis;


namespace StreamNodeWebApi.Controllers
{
    /// <summary>
    /// 授权访问接口类
    /// </summary>
    [ApiController]
    [Route("/DvrPlan")]
    public class DvrPlanController : ControllerBase
    {
        
       

        
        
        /// <summary>
        /// 删除一个录制计划ById
        /// </summary>
        /// <returns></returns>
        [Route("DeleteDvrPlanById")]
        [HttpGet]
        [Log]
        [AuthVerify]
        public bool DeleteDvrPlanById(long id)
        {
            ResponseStruct rs;
            var ret = DvrPlanApis.DeleteDvrPlanById(id, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }
            return ret;
        }
       

        /// <summary>
        /// 启用或停用一个录制计划
        /// </summary>
        /// <returns></returns>
        [Route("OnOrOffDvrPlanById")]
        [HttpGet]
        [Log]
        [AuthVerify]
        public bool OnOrOffDvrPlanById(long id, bool enable)
        {
            ResponseStruct rs;
            var ret = DvrPlanApis.OnOrOffDvrPlanById(id,enable, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }
            return ret;
        }
        
        

        /// <summary>
        /// 修改录制计划ById
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sdp"></param>
        /// <returns></returns>
        ///
        [Route("SetDvrPlanById")]
        [HttpPost]
        [Log]
        [AuthVerify]
        public bool SetDvrPlanById( long id, ReqStreamDvrPlan sdp)
        {
            ResponseStruct rs;
            var ret = DvrPlanApis.SetDvrPlanById(id,sdp, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }
            return ret;
        }
       

        /// <summary>
        /// 创建录制计划
        /// </summary>
        /// <returns></returns>
        [Route("CreateDvrPlan")]
        [HttpPost]
        [Log]
        [AuthVerify]
        public bool CreateDvrPlan(  ReqStreamDvrPlan sdp)
        {
            ResponseStruct rs;
            var ret = DvrPlanApis.CreateDvrPlan(sdp, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }
            return ret;
        }
      

        /// <summary>
        /// 获取录制计划
        /// </summary>
        /// <returns></returns>
        ///
        [Route("GetDvrPlan")]
        [HttpPost]
        [Log]
        [AuthVerify]
        public List<StreamDvrPlan> GetDvrPlan(ReqGetDvrPlan rdp)
        {
            ResponseStruct rs;
            var ret = DvrPlanApis.GetDvrPlanList(rdp, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }
            return ret;
        }
       
    }
}