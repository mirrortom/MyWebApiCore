using System;
using System.Threading;
using System.Threading.Tasks;
using MyWebApi.core;

// 这个类加了一层命名空间(.api)
namespace MyWebApi.api;

internal class Demo1Api : ApiBase
{
    [HTTPGET]
    public async Task index()
    {
        var res = new
        {
            name = "url地址:api/demo1/index",
            info = "api是命名空间,后面是类名和方法名.这是GET请求返回是json数据."
        };
        await this.Json(res);
    }

    
    [HTTPPOST]
    public async Task index(int type)
    {
        var res = new
        {
            name = "url地址:api/demo1/index",
            info = "api是命名空间,后面是类名和方法名.这是用于POST请求方式的重载方法."
        };
        await this.Json(res);
    }

    [ROUTE("api/demo1/indexReLoad")]
    [HTTPGET]
    public async Task index(int type,string title)
    {
        var res = new
        {
            name = "url地址:api/demo1/index",
            info = "api是命名空间,后面是类名和方法名.这是用于GET请求方式的有参数重载方法.如果调用到,说明有bug!"
        };
        await this.Json(res);
    }
}
