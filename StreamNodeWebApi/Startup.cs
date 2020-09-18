using System.IO;
using System.Text.Json.Serialization;
using CommonFunctions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;


namespace StreamNodeWebApi
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
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "StreamNodeApi", Version = "v1"});
                c.IncludeXmlComments(Path.Combine(Common.WorkPath, "Swagger.xml"));
            });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            /*services.AddControllers()
                .AddNewtonsoftJson(
                    options =>
                    {
                        options.SerializerSettings.Converters.Add(new IsoDateTimeConverter()
                        {
                            DateTimeFormat = "yyyy-MM-dd HH:mm:ss",
                        });
                    }
                );*/

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
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "StreamNodeApi"); });
            app.UseRouting();
            //注意：UseCors必须放在UseRouting和UseEndpoints之间
            app.UseCors("cors");
            app.UseMiddleware<ExceptionMiddleware>(); //ExceptionMiddleware 加入管道
            app.UseAuthorization();
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