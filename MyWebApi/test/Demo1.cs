using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyWebApi.api
{
    class Demo1Api:ApiBase
    {
        [HTTPGET]
        public async Task index()
        {
            var res = new
            {
                name = "url地址:api/demo1/index",
                info = "api是命名空间,后面是类名和方法名"
            };
            await this.Json(res);
        }
    }
}
