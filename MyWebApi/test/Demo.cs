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
        public void getpara()
        {
            dynamic query = this.ParaGET();
            this.Json(query);
        }
        [HTTPGET]
        public void getParaType()
        {
            dynamic query = this.ParaGET<testEntity>();
            this.Json(query);
        }
        [HTTPPOST]
        public void formpara()
        {
            dynamic query = this.ParaForm();
            this.Json(query);
        }
        [HTTPPOST]
        public void formParaType()
        {
            dynamic query = this.ParaForm<testEntity>();
            this.Json(query);
        }
        [HTTPPOST]
        public void parajson()
        {
            dynamic para = this.ParaStream();
            this.Json(para);
        }
        [HTTPGET]
        [AUTH]
        public void token()
        {
            this.Html("<p>需要权限,贴上[AUTH]特性.实现ApiHandler.PowerCheck()方法.</p>");
        }
        [HTTPGET]
        public void throwcatch()
        {
            throw new Exception("");
        }
        [HTTPPOST]
        public void getfile()
        {
            string file = AppContext.BaseDirectory + "/wwwroot/index.html";
            this.File(file, "application/octet-stream", "index.html");
        }
        [HTTPPOST]
        public void uploadfile()
        {
            if (this.Request.Form.Files.Count == 0)
            {
                this.Json(new { info = "没有收到上传文件" });
                return;
            }
            IFormFile file = this.Request.Form.Files[0];

            // save file
            string filePath = AppContext.BaseDirectory + $"/{Guid.NewGuid():N}" + file.FileName;
            using var stream = System.IO.File.Create(filePath);
            file.CopyToAsync(stream);

            // return result
            this.Json(new
            {
                info = $"文件名:{file.FileName},大小:{file.Length}"
            });
        }
        [HTTPGET]
        public void gethtml()
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
            this.Html(html);
        }

    }
}
