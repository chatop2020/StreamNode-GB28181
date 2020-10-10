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
            string info=$@"StatusCode:{context.HttpContext.Response.StatusCode}";
            string remoteIpAddr=context.HttpContext.Connection.RemoteIpAddress.ToString();
            try
            {
             
                if (context.HttpContext.Response.StatusCode == (int) HttpStatusCode.OK)
                {
                    if (!context.HttpContext.Request.Path.Equals("/WebHook/MediaServerRegister"))
                    {
                        info =
                            $@"{info}  Body: {JsonConvert.SerializeObject(((context.Result as ObjectResult)!).Value)}";
                        Logger.Logger.Debug($@"OUTPUT    {remoteIpAddr}    {context.HttpContext.Request.Method}    {context.HttpContext.Request.Path} ->"+ info);
                        /*LogWebApiWriter.WriteWebApiLog(
                           c,
                            info,
                            ConsoleColor.Gray);*/
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.Logger.Error($@"OUTPUT    {remoteIpAddr}    {context.HttpContext.Request.Method}    {context.HttpContext.Request.Path} ->"+info+" -> "+ex.Message+" -> "+ex.StackTrace);
            }
        }

        /// <summary>
        /// 请求中
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            string remoteIpAddr = context.HttpContext.Connection.RemoteIpAddress.ToString();
            try
            {
                if (context.HttpContext.Request.Method.Equals("get", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (!context.HttpContext.Request.Path.Equals("/WebHook/MediaServerRegister"))
                    {
                        Logger.Logger.Debug($@"OUTPUT    {remoteIpAddr}    {context.HttpContext.Request.Method}    {context.HttpContext.Request.Path} ->"+ $@"{JsonConvert.SerializeObject(context.ActionArguments)}");

                        /*LogWebApiWriter.WriteWebApiLog(
                            $@"INPUT    {remoteIpAddr}    {context.HttpContext.Request.Method}    {context.HttpContext.Request.Path}",
                            $@"{JsonConvert.SerializeObject(context.ActionArguments)}", ConsoleColor.Gray);*/
                    }
                }
                else
                {
                    if (!context.HttpContext.Request.Path.Equals("/WebHook/MediaServerRegister"))
                    {
                        Logger.Logger.Debug($@"INPUT    {remoteIpAddr}    {context.HttpContext.Request.Method}    {context.HttpContext.Request.Path} -> "+ $@"{JsonConvert.SerializeObject(context.ActionArguments)}");

                        /*LogWebApiWriter.WriteWebApiLog(
                            $@"INPUT    {remoteIpAddr}    {context.HttpContext.Request.Method}    {context.HttpContext.Request.Path}",
                            $@"{JsonConvert.SerializeObject(context.ActionArguments)}", ConsoleColor.Gray);*/
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.Logger.Error($@"OUTPUT    {remoteIpAddr}    {context.HttpContext.Request.Method}    {context.HttpContext.Request.Path} ->"+remoteIpAddr+" -> "+ex.Message+" -> "+ex.StackTrace);

            }
        }
    }
}