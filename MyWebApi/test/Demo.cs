using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

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
        [HTTPPOST]
        public async Task formpara()
        {
            dynamic query = this.ParaForm();
            await this.Json(query);
        }
        [HTTPPOST]
        public async Task parajson()
        {
            dynamic para = this.ParaStream();
            await this.Json(para);
        }
        [HTTPPOST]
        public async Task getfile()
        {
            await this.File("readme.html", "application/octet-stream", "说明readme.html");
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
