using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MyWebApi
{
    class Program
    {
        static void Main(string[] args)
        {
            // 加载配置选项
            CustomSetting.Load();

            // webapp 设置
            var webapp = (IApplicationBuilder app) =>
            {
                // 生产环境异常处理
                app.UseExceptionHandler(ApiHandler.CustomExceptMW);
                // 开发环境异常处理 (系统中间件)
                //app.UseDeveloperExceptionPage();

                // 默认文档,静态文件 (系统中间件)
                app.UseDefaultFiles()
                   .UseStaticFiles();

                // 提供wwwroot以外的其它虚拟目录(静态文件的) (系统中间件)
                if (CustomSetting.VirtualDirsOptions != null)
                {
                    foreach (var item in CustomSetting.VirtualDirsOptions)
                        app.UseStaticFiles(item);
                }

                // 跨域策略 (系统中间件)
                app.UseCors(CustomSetting.CorsConfigBuild);

                // url映射到类的方法
                app.Run(ApiHandler.UrlMapMethodMW);

            };

            // webhost 设置
            var webHostBuild = (IWebHostBuilder webHost) =>
            {
                webHost.UseKestrel()// 使用kestrel服务器
                       .UseUrls(CustomSetting.Urls)// 监听端点
                       .Configure(webapp);// 承载应用
            };

            // host 服务设置
            var srvsOnHost = (IServiceCollection services) =>
            {
                // 跨域服务
                services.AddCors();
                services.AddMemoryCache();
            };

            // 建立主机->载入配置->运行
            // 用默认配置建立主机:(https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-7.0)
            Host.CreateDefaultBuilder()
                .ConfigureServices(srvsOnHost)// 按需配置服务
                .ConfigureWebHost(webHostBuild)// 配置web主机
                .Build()
                .Run();
        }
    }

}
