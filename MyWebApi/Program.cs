using MyWebApi.core;
using MyWebApi.test;
using cfg = MyWebApi.core.Config;

// 加载配置选项
cfg.Load();
// 路由初始化
RouteMap.Init();

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
    if (cfg.EnableStatic)
    {
        if (cfg.DefaultDocOptions != null)
            app.UseDefaultFiles(cfg.DefaultDocOptions);
        app.UseStaticFiles();
    }

    // 提供wwwroot以外的其它虚拟目录(静态文件的) (系统中间件)
    if (cfg.VirtualDirsOptions != null)
    {
        foreach (var item in cfg.VirtualDirsOptions)
            app.UseStaticFiles(item);
    }

    // 跨域策略 (系统中间件)
    app.UseCors(Config.CorsConfigBuild);

    // 查找并执行处理方法
    app.Run(ApiHandler.ProcReqMW);
};

// 通用主机:(https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-7.0)
// web主机:(https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/host/web-host?view=aspnetcore-7.0)

// 建立通用主机->载入配置->运行
var hostBuilder = new HostBuilder();
hostBuilder.ConfigureServices((IServiceCollection services) =>
{
    // 跨域服务(https://learn.microsoft.com/zh-cn/aspnet/core/security/cors?view=aspnetcore-7.0)
    services.AddCors();

    // 获取服务器信息,F5测试时会在控制台打印.主要用于测试
#if DEBUG
    // addhostedservice添加的服务,不能通过HttpContext.RequestServices.GetService()方式获取,
    // 可以获取AddSingleton方式添加的服务.
    // 另外,hostedservice方式添加的服务会主动执行,而sing却不会,要自己调用一次才行.
    // 所以,hosted添加是为了立即执行在控制台打印信息.
    // 而sing方式是为了演示在webapi中可以调用该服务.示例见demoapi.getinfo()
    services.AddHostedService<TestInfoService>()
            .AddSingleton<TestInfoService>();
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
//hostBuilder.UseWindowsService();
// 启动
IHost host = hostBuilder.Build();
// Run执行后,程序会在这里监听阻塞,run()后面的语句不会执行,直到监听程序结束后才执行.
// host.Run();
await host.RunAsync();

//Console.WriteLine("你按了ctrl + c, kestrel服务结束!程序结束.");