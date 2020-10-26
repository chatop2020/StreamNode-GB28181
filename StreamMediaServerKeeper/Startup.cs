using System.IO;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;

namespace StreamMediaServerKeeper
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //配置跨域处理，允许所有来源
            services.AddCors(options =>
            {
                options.AddPolicy("cors",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                );
            });


            // 注册Swagger服务
            services.AddSwaggerGen(c =>
            {
                // 添加文档信息
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "MediaServerProcessApi", Version = "v1"});
                c.IncludeXmlComments(Path.Combine(Common.WorkPath, "Swagger.xml"));
            });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddControllers().AddJsonOptions(
                options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
            ).AddJsonOptions(configure =>
            {
                configure.JsonSerializerOptions.Converters.Add(new DatetimeJsonConverter());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            /*
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            */


            app.UseHttpsRedirection();
            // 启用Swagger中间件
            app.UseSwagger();
            // 配置SwaggerUI
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "MediaServerProcessApi"); });
            app.UseRouting();
            //注意：UseCors必须放在UseRouting和UseEndpoints之间
            app.UseCors("cors");
            app.UseMiddleware<ExceptionMiddleware>(); //ExceptionMiddleware 加入管道
            app.UseAuthorization();


            if (!Directory.Exists(Common.CutOrMergePath))
            {
                Directory.CreateDirectory(Common.CutOrMergePath);
            }

            if (!Directory.Exists(Common.CutOrMergeTempPath))
            {
                Directory.CreateDirectory(Common.CutOrMergeTempPath);
            }

            /*var staticfile = new StaticFileOptions();
            staticfile.FileProvider = new PhysicalFileProvider(Common.StaticFilePath); //指定静态文件服务器

            //手动设置MIME Type,或者设置一个默认值， 以解决某些文件MIME Type文件识别不到，出现404错误
            staticfile.ServeUnknownFileTypes = true;
            staticfile.DefaultContentType = "application/octet-stream"; //设置默认MIME Type
            staticfile.OnPrepareResponse = (c) =>
            {
                c.Context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            };
            app.UseStaticFiles(staticfile);*/

            /*app.UseCors("cors").UseFileServer(new FileServerOptions()
            {
                FileProvider = new PhysicalFileProvider
                    (Path.Combine(Directory.GetCurrentDirectory(), Common.RecordPath)), //实际目录地址
                RequestPath = new PathString("/CustomizedRecord"), //用户访问地址
                EnableDirectoryBrowsing = true //开启目录浏览
                
            });*/


            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider =
                    new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), Common.CutOrMergePath)),
                OnPrepareResponse = (c) => { c.Context.Response.Headers.Add("Access-Control-Allow-Origin", "*"); },
                RequestPath = new PathString("/CutMergeFile")
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider =
                    new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), Common.RecordPath)),
                OnPrepareResponse = (c) => { c.Context.Response.Headers.Add("Access-Control-Allow-Origin", "*"); },
                RequestPath = new PathString("/CustomizedRecord")
            });


            /*app.UseCors("cors").UseFileServer(new FileServerOptions()
            {
                FileProvider = new PhysicalFileProvider
                    (Path.Combine(Directory.GetCurrentDirectory(), Common.CutOrMergePath))
              ,
                 //实际目录地址
                RequestPath = new PathString("/CutMergeFile"), //用户访问地址
                EnableDirectoryBrowsing = true //开启目录浏览
            });*/


            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllers();
                    //跨域需添加RequireCors方法，cors是在ConfigureServices方法中配置的跨域策略名称
                    endpoints.MapControllers().RequireCors("cors");
                });
        }
    }
}