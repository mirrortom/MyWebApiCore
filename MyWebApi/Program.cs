using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MyWebApi
{
    class Program
    {
        static void Main(string[] args)
        {
            // 主机功能配置项
            void serverCfg(IServiceCollection service)
            {
                // 跨域功能
                service.AddCors();
            }

            // 跨域策略配置项
            void cors(CorsPolicyBuilder cfg)
            {
                cfg.AllowAnyHeader();
                cfg.AllowAnyMethod();
                cfg.AllowAnyOrigin();
                cfg.AllowCredentials();
            }

            // kestrel服务器配置文件载入
            IConfiguration kestrelCfg = new ConfigurationBuilder()
                .AddJsonFile("kestrel.json")
                .Build();

            // 开机运行,可选择其中一种方式运行,服务或者控制台
            // 实例化主机,载入配置项
            IWebHost webhost = new WebHostBuilder()
                .ConfigureServices(serverCfg)
                .UseConfiguration(kestrelCfg)
                .UseKestrel()
                .Configure(app => app
                    .UseCors(cors)// 跨域
                    .UseDeveloperExceptionPage()// 能在请求页面上显示异常信息
                    .Use(ApiHandler.UrlHandler)// 自定义路由中间件
                )
                .Build();
            if (args.Length > 0 && args[0] == "s")
            {
                // 以windows服务方式运行
                webhost.RunAsService();
            }
            else
            {
                webhost.Run();
            }
        }
    }
}
