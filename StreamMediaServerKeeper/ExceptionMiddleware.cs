using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace StreamMediaServerKeeper
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private IHostingEnvironment environment;

        public ExceptionMiddleware(RequestDelegate next, IHostingEnvironment environment)
        {
            this.next = next;
            this.environment = environment;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next.Invoke(context);
                var features = context.Features;
            }
            catch (HttpResponseException e)
            {
                await MyHandleException(context, e);
            }
            catch (Exception e)
            {
                await HandleException(context, e);
            }
        }

        private async Task MyHandleException(HttpContext context, Exception e)
        {
            context.Response.StatusCode = 400;
            context.Response.ContentType = "text/json;charset=utf-8;";
            string error = e.Message;
            string info = $@"StatusCode:{context.Response.StatusCode}";
            string remoteIpAddr = context.Connection.RemoteIpAddress.ToString();
            info = $@"{info}  Body: {error}";


            await context.Response.WriteAsync(error);
        }

        private async Task HandleException(HttpContext context, Exception e)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "text/json;charset=utf-8;";
            string error = "";
            if (environment.IsDevelopment())
            {
                var json = new {message = e.Message};
                error = JsonConvert.SerializeObject(json);
            }
            else error = "抱歉，出错了";

            string info = $@"StatusCode:{context.Response.StatusCode}";
            string remoteIpAddr = context.Connection.RemoteIpAddress.ToString();
            info = $@"{info}  Body: {error}";

            await context.Response.WriteAsync(error);
        }
    }
}