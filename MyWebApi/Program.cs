using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using MyWebApi;
using MyWebApi.test;
using System;
using cfg = MyWebApi.Config;

// 加载配置选项
cfg.Load();

// webapp 设置
var webapp = (IApplicationBuilder app) =>
{
    // 处理 ASP.NET Core 中的错误
    // 开发人员异常页运行在中间件管道的前面部分,以便它能够捕获随后中间件中抛出的未经处理的异常
    // https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/error-handling?view=aspnetcore-7.0
#if DEBUG
    // 开发环境异常处理 (系统中间件)
    app.UseDeveloperExceptionPage();
#else
    // 生产环境异常处理
    app.UseExceptionHandler(ApiHandler.CustomExceptMW);
#endif

    // 默认文档,静态文件 (系统中间件)
    app.UseDefaultFiles()
       .UseStaticFiles();

    // 提供wwwroot以外的其它虚拟目录(静态文件的) (系统中间件)
    if (cfg.VirtualDirsOptions != null)
    {
        foreach (var item in cfg.VirtualDirsOptions)
            app.UseStaticFiles(item);
    }

    // 跨域策略 (系统中间件)
    app.UseCors(cfg.CorsConfigBuild);

    // url映射到类的方法
    app.Run(ApiHandler.UrlMapMethodMW);
};

// 通用主机:(https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-7.0)
// web主机:(https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/host/web-host?view=aspnetcore-7.0)

// 建立通用主机->载入配置->运行
var host = new HostBuilder();
host.ConfigureServices((IServiceCollection services) =>
{
    // 跨域服务(https://learn.microsoft.com/zh-cn/aspnet/core/security/cors?view=aspnetcore-7.0)
    services.AddCors();
    // MemoryCache内存缓存工具.在ApiBase.SetHttpContext里获取并复制到属性上
    services.AddMemoryCache();

#if DEBUG
    // 在控制台打印服务器信息,用于测试时
    services.AddSingleton<TestInfoService>()
            .AddHostedService<TestInfoService>();
#endif
})
.ConfigureWebHost((IWebHostBuilder webHostBuild) =>
{
    // 设置web服务主机
    // 使用kestrel服务器
    webHostBuild.UseKestrel()
    // 配置监听端点
    .UseUrls(cfg.Urls)
    // 加入应用程序
    .Configure(webapp);
});
// 部署成windows服务
//host.UseWindowsService();
// 启动
host.Build()
    .Run();


