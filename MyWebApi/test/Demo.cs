using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace MyWebApi
{
    class testEntity
    {
        public string name;
        public string title;
    }
    class DemoApi : ApiBase
    {
        [HTTPGET]
        public async Task getpara()
        {
            dynamic query = this.ParaGET();
            await this.Json(query);
        }
        [HTTPGET]
        public async Task getParaType()
        {
            dynamic query = this.ParaGET<testEntity>();
            await this.Json(query);
        }
        [HTTPPOST]
        public async Task formpara()
        {
            dynamic query = this.ParaForm();
            await this.Json(query);
        }
        [HTTPPOST]
        public async Task formParaType()
        {
            dynamic query = this.ParaForm<testEntity>();
            await this.Json(query);
        }
        [HTTPPOST]
        public async Task parajson()
        {
            dynamic para = await this.ParaStream();
            await this.Json(para);
        }
        [HTTPGET]
        [AUTH]
        public async Task token()
        {
            await this.Html("<p>需要权限,贴上[AUTH]特性.实现ApiHandler.PowerCheck()方法.</p>");
        }
        [HTTPGET]
        public async Task throwcatch()
        {
            throw new Exception("自定义异常处理方法,在ApiHandler.CustomExceptionHandlerOptions()方法里实现.这是一个中间件,在Program.cs启动方法里要开起它.");
            await this.Json(new { });
        }
        [HTTPPOST]
        public async Task getfile()
        {
            string file = AppContext.BaseDirectory+ "/wwwroot/readme.html";
            await this.File(file, "application/octet-stream", "说明readme.html");
        }
        [HTTPPOST]
        public async Task uploadfile()
        {
            if (this.Request.Form.Files.Count == 0)
            {
                await this.Json(new { info = "没有收到上传文件" });
                return;
            }
            IFormFile file = this.Request.Form.Files[0];

            await this.Json(new
            {
                info = $"文件名:{file.FileName},大小:{file.Length}"
            });
        }
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

    }
}
