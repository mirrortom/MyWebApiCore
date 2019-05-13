using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyWebApi
{
    class Program
    {
        static void Main(string[] args)
        {
            // 主机功能配置
            void serverCfg(IServiceCollection service)
            {
                // 跨域功能
                service.AddCors();
            }

            // 跨域策略配置
            void cors(CorsPolicyBuilder cfg)
            {
                cfg.AllowAnyHeader();
                cfg.AllowAnyMethod();
                cfg.AllowAnyOrigin();
                cfg.AllowCredentials();
            }

            // kestrel服务器配置
            IConfiguration kestrelCfg = new ConfigurationBuilder()
                .AddJsonFile("kestrel.json")
                .Build();

            // 开机运行,可选择其中一种方式运行,服务或者控制台
            IWebHost webhost = new WebHostBuilder()
                .ConfigureServices(serverCfg)
                .UseConfiguration(kestrelCfg)
                .UseKestrel()
                .Configure(app => app
                    .UseCors(cors)
                    .UseDeveloperExceptionPage()
                    .Use(ApiHandler.UrlHandler)
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
