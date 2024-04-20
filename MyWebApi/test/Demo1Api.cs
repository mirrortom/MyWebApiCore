using System.Threading.Tasks;
using MyWebApi.core;

namespace MyWebApi.api;

internal class Demo1Api : ApiBase
{
  [HTTPGET]
  [HTTPPOST]
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
