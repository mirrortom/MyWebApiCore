using System.Text;
using cfg = MyWebApi.core.Config;
namespace MyWebApi.test;

public class TestInfoService : IHostedService
{
    private readonly IWebHostEnvironment WebHostEnv;
    private readonly IConfiguration Configuration;

    public TestInfoService(IWebHostEnvironment webHostEnv, IConfiguration configuration)
    {
        WebHostEnv = webHostEnv;
        Configuration = configuration;
    }

    public Dictionary<string, string> GetInfo()
    {
        string urls = Configuration.GetValue<string>(WebHostDefaults.ServerUrlsKey);
        StringBuilder virDirsInfo = new();
        if (cfg.VirtualDirsOptions != null && cfg.VirtualDirsOptions.Count > 0)
        {
            foreach (var item in cfg.VirtualDirsOptions)
            {
                string fsDir = ((Microsoft.Extensions.FileProviders.PhysicalFileProvider)item.FileProvider).Root;
                string str = $"虚拟路径{item.RequestPath}=>本地路径:{fsDir}{Environment.NewLine}";
                virDirsInfo.Append(str);
            }
        }
        return new Dictionary<string, string> {
            { "名字 AppName", WebHostEnv.ApplicationName },
            { "程序运行目录 AppContext.BaseDirectory", AppContext.BaseDirectory },
            {"web服务器监听 server listen",urls },
            {"内容根目录 ContentRootPath",WebHostEnv.ContentRootPath },
            {"web服务器 server","Kestrel" },
            {"web静态文件",cfg.EnableStatic?"启用":"禁用" },
            {"web根目录 WebRootPath",WebHostEnv.WebRootPath },
            {"web默认文档 DefaultFileNames",string.Join(',',cfg.DefaultDocOptions?.DefaultFileNames??[])},
            {"web虚拟目录 VirtualDirs",virDirsInfo.ToString() },
        };
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        string n = Environment.NewLine;
        Console.WriteLine($"--应用信息--");
        var dict = GetInfo();
        foreach (var k in dict.Keys)
        {
            Console.WriteLine($"#{k} {n}{dict[k]}{n}");
        }
        Console.WriteLine($"#{n}打开浏览器进行测试. ctrl + c 退出.");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}