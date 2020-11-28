using System;
using System.Net;
using CommonFunctions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace StreamNodeWebApi
{
    /// <summary>
    /// 验证session和allowkey的类
    /// 暂时没有启用
    /// </summary>
    public class AuthVerifyAttribute : Attribute, IActionFilter
    {
        /// <summary>
        /// 请求结束后
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        /// <summary>
        /// 请求进行中，判断用户session及allowkey的合法性
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (Common.IsDebug) return;
            string remoteIpAddr = context.HttpContext.Connection.RemoteIpAddress.ToString();
            string sessionCode = context.HttpContext.Request.Headers["SessionCode"];
            string allowKey = context.HttpContext.Request.Headers["Allowkey"];
            /*if (sessionCode == null || !Common.CheckSession(sessionCode))
            {
                var result = new JsonResult(ErrorMessage.ErrorDic?[ErrorNumber.SystemSessionExcept]);
                result.StatusCode = (int) HttpStatusCode.BadRequest;
                context.Result = result;
            }

            if (allowKey == null || !Common.CheckAllow(remoteIpAddr, allowKey))
            {
                var result = new JsonResult(ErrorMessage.ErrorDic?[ErrorNumber.SystemCheckAllowKeyFail]);
                result.StatusCode = (int) HttpStatusCode.BadRequest;
                context.Result = result;
            }*/
        }
    }
}