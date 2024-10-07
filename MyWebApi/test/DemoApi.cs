using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using MyWebApi.core;
using MyWebApi.test;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MyWebApi;

internal class DemoApi : ApiBase
{
    [HTTPPOST]
    [HTTPGET]
    public async Task getinfo()
    {
        var testinfo = this.HttpContext.RequestServices.GetService(typeof(TestInfoService)) as TestInfoService;
        if (testinfo != null)
        {
            var dict = testinfo.GetInfo();
            await this.Json(dict);
            return;
        }
        var srv = this.HttpContext.Connection;
        var result = new { ip = srv.LocalIpAddress.ToString(), port = srv.LocalPort };
        await this.Json(result);
    }

    // get参数
    [HTTPPOST]
    [HTTPGET]
    public async Task getpara()
    {
        dynamic query = this.ParaGET();
        await this.Json(query);
    }

    // get类型参数
    [HTTPPOST]
    [HTTPGET]
    public async Task getParaType()
    {
        var query = this.ParaGET<DemoEntity>();
        await this.Json(query);
    }

    // 表单参数
    [HTTPPOST]
    public async Task formpara()
    {
        dynamic query = await this.ParaForm();
        await this.Json(query);
    }

    // 表单类型参数
    [HTTPPOST]
    public async Task formParaType(int type)
    {
        var query = await this.ParaForm<DemoEntity>();
        await this.Json(query);
    }

    // body参数
    [HTTPPOST]
    public async Task parabody()
    {
        var para = await this.ParaStream();
        await this.Text(para);
    }

    // 需要权限
    [HTTPPOST]
    [AuthDemo]
    public async Task auth()
    {
        var srv = WrapContext.NewSrv<Srv1Demo>(this.HttpContext);
        await this.Json(srv.User);
    }

    // 获取token
    [HTTPPOST]
    public async Task gettoken()
    {
        string token = TokenDemo.Create();
        await this.Text(token);
    }

    // 丢出异常
    [HTTPGET]
    [HTTPPOST]
    public async Task throwcatch()
    {
        throw new Exception("server was throw a exception!");
        await Task.CompletedTask;
    }

    // 提供下载文件
    [HTTPPOST]
    public async Task getfile()
    {
        string file = AppContext.BaseDirectory + "/wwwroot/index.html";
        await this.File(file, "application/octet-stream", "index.html");
    }

    // 接收上传文件
    [HTTPPOST]
    public async Task uploadfile()
    {
        if (this.Request.Form.Files.Count == 0)
        {
            await Task.Delay(1000);
            await this.Json(new { info = "没有收到上传文件" });
            return;
        }
        IFormFile file = this.Request.Form.Files[0];

        // save file
        string filePath = AppContext.BaseDirectory + Path.GetRandomFileName() + Path.GetExtension(file.FileName);
        using (var stream = System.IO.File.Create(filePath))
        {
            await file.CopyToAsync(stream);
        }

        // return result
        await this.Json(new
        {
            info = $"文件名:{file.FileName},大小:{file.Length}"
        });
    }

    // 返回html文本
    [HTTPPOST]
    [HTTPGET]
    public async Task gethtml()
    {
        string html = @"
<!DOCTYPE>
<html>
<head></head>
<body>
<h1>asp.net core MyWebApi</h1>
<p>返回一个HTML文本</p>
</body>
</html>
";
        await this.Html(html);
    }

    // 返回纯文本
    [HTTPPOST]
    [HTTPGET]
    public async Task gettext()
    {
        string txt = "hello world. I trying return a plain!";
        await this.Text(txt);
    }

    // 读取或者写入缓存
    [HTTPPOST]
    [HTTPGET]
    public async Task cache()
    {
        string key = "last-request-time";
        // 取出上次请求时间
        string lastRequestTime = WrapContext.MemoryCache.Get<string>(key);
        // 缓存本次请求时间
        DateTime requestTime = DateTime.Now;
        WrapContext.MemoryCache.Set<string>(key, requestTime.ToString());
        await this.Json(new
        {
            lastRequestTime =
            string.IsNullOrWhiteSpace(lastRequestTime) ? "---" : lastRequestTime
        });
    }

    // 使用服务类
    [HTTPPOST]
    [HTTPGET]
    public async Task srv()
    {
        // WrapContext,请求处理服务类.
        // 请求到达后,为了处理请求,设计了一个上下文类WrapContext,里面包含常用的数据.
        // 比如:请求者信息,错误代码,缓存,数据操作接口等.
        // 除了这些公用的数据,每个api处理时也会用到特定数据.所以使用时,继承WrapContext类.
        // 然后重写init方法,实现特定数据.
        var srv = WrapContext.NewSrv<Srv1Demo>(this.HttpContext);
        await this.Json(new { errcode = 200, ermsg = srv.info() });
    }
}