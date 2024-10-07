using Microsoft.AspNetCore.Http;
using MyWebApi.core;

namespace MyWebApi.test;

/// <summary>
/// 验证授权,这个一般实现就是检查token,如果有效,生成一个对象保留请求者信息.
/// </summary>
internal class AuthDemoAttribute : AUTHBaseAttribute
{
    internal override bool Authenticate(HttpContext context)
    {
        // 取得token
        string token = context.Request.Headers["Auth"];
        return TokenDemo.Check(TokenDemo.TokenToUser(token));
    }
}
