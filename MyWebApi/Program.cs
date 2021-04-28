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
 
            // kestrel服务器功能选项加入配置
            static void kestrelAppConfBuild(IApplicationBuilder app)
            {
                // 跨域
                app.UseCors(CustomSetting.CorsConfigBuild);

                // 默认文档(注意调用顺序,要在"静态文件UseStaticFiles"之前调用)
                app.UseDefaultFiles(CustomSetting.DefaultDocOptions);

                // 静态文件功能(注意调用顺序,要在"自定义路由中间件"之前调用)
                app.UseStaticFiles();

                // 虚拟目录(静态文件的)
                if (CustomSetting.VirtualDirsOptions != null)
                {
                    foreach (var item in CustomSetting.VirtualDirsOptions)
                        app.UseStaticFiles(item);
                }

                // 系统提供的异常处理,能在请求页面上显示异常信息,信息很详细,用于开发环境调错.
                //app.UseDeveloperExceptionPage();

                // 自定义异常处理返回中间件.返回一个json
                app.UseExceptionHandler(ApiHandler.CustomExceptionHandlerOptions());

                // 自定义路由中间件.这个中间件安排在最后,所以没有调用next().
                app.Use(ApiHandler.UrlHandler);
            }


            // web服务器主机: 加入功能选项,选择kestrel服务器
            static void webHostBuild(IWebHostBuilder webBuilder)
            {
                webBuilder.Configure(kestrelAppConfBuild);
                webBuilder.UseUrls(CustomSetting.Urls);
                webBuilder.UseKestrel();
            }

            // 通用承载主机: 添加服务等
            static void servicesConfigure(IServiceCollection services)
            {
                // 跨域服务,(为什么不是在web主机上加入呢?)
                services.AddCors();
            }

            // 建立主机
            IHostBuilder hostBuild = Host.CreateDefaultBuilder();
            hostBuild.ConfigureServices(servicesConfigure);
            // 添加kestrelweb服务器
            hostBuild.ConfigureWebHostDefaults(webHostBuild);
            // 生成主机实例,开机运行
            IHost host = hostBuild.Build();
            host.Run();
        }
    }

}
