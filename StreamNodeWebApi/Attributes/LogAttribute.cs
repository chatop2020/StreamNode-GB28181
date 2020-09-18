using System;
using System.Net;
using CommonFunctions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace StreamNodeWebApi
{
    /// <summary>
    /// 日志记录
    /// </summary>
    public class LogAttribute : Attribute, IActionFilter
    {
        /// <summary>
        /// 请求后
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            try
            {
                string info = $@"StatusCode:{context.HttpContext.Response.StatusCode}";
                string remoteIpAddr = context.HttpContext.Connection.RemoteIpAddress.ToString();
                if (context.HttpContext.Response.StatusCode == (int) HttpStatusCode.OK)
                {
                    if (!context.HttpContext.Request.Path.Equals("/WebHook/MediaServerRegister"))
                    {
                        info =
                            $@"{info}  Body: {JsonConvert.SerializeObject(((context.Result as ObjectResult)!).Value)}";
                        LogWebApiWriter.WriteWebApiLog(
                            $@"OUTPUT    {remoteIpAddr}    {context.HttpContext.Request.Method}    {context.HttpContext.Request.Path}",
                            info,
                            ConsoleColor.Gray);
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// 请求中
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                string remoteIpAddr = context.HttpContext.Connection.RemoteIpAddress.ToString();
                if (context.HttpContext.Request.Method.Equals("get", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (!context.HttpContext.Request.Path.Equals("/WebHook/MediaServerRegister"))
                    {
                        LogWebApiWriter.WriteWebApiLog(
                            $@"INPUT    {remoteIpAddr}    {context.HttpContext.Request.Method}    {context.HttpContext.Request.Path}",
                            $@"{JsonConvert.SerializeObject(context.ActionArguments)}", ConsoleColor.Gray);
                    }
                }
                else
                {
                    if (!context.HttpContext.Request.Path.Equals("/WebHook/MediaServerRegister"))
                    {
                        LogWebApiWriter.WriteWebApiLog(
                            $@"INPUT    {remoteIpAddr}    {context.HttpContext.Request.Method}    {context.HttpContext.Request.Path}",
                            $@"{JsonConvert.SerializeObject(context.ActionArguments)}", ConsoleColor.Gray);
                    }
                }
            }
            catch
            {
            }
        }
    }
}