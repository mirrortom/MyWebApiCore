using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;
using Microsoft.Extensions.Caching.Memory;

namespace MyWebApi
{

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
            dynamic query = this.ParaGET<DemoEntity>();
            await this.Json(query);
        }
        [HTTPPOST]
        public async Task formpara()
        {
            dynamic query = await this.ParaForm();
            await this.Json(query);
        }
        [HTTPPOST]
        public async Task formParaType()
        {
            dynamic query = await this.ParaForm<DemoEntity>();
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
            throw new Exception("server was throw a exception!");
            await Task.CompletedTask;
        }
        [HTTPPOST]
        public async Task getfile()
        {
            string file = AppContext.BaseDirectory + "/wwwroot/index.html";
            await this.File(file, "application/octet-stream", "index.html");
        }
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

        // 读取或者写入缓存
        [HTTPGET]
        public async Task cache()
        {
            string id = this.MemoryCache.Get<string>("id");
            await this.Json(new { id });
        }
    }
}
