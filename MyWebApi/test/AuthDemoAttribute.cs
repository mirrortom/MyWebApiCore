using Microsoft.AspNetCore.Http;
using MyWebApi.core;

namespace MyWebApi.test;

/// <summary>
/// 验证授权
/// 1.继承AUTHBase特性,重写其Authenticate方法
/// 2.重写方法里,分析请求数据里带的token,返回是否通过
/// 3.使用时,将特性贴在API类或者方法上
/// </summary>
internal class AuthDemoAttribute : AUTHBaseAttribute
{
    internal override bool Authenticate(HttpContext context)
    {
        // 取得token
        string? token = context.Request.Headers["Auth"];
        // 验证token,验证用户API执行权限
        return !string.IsNullOrEmpty(token)
        && TokenDemo.Check(TokenDemo.TokenToUser(token))
        && this.Id == 1;
    }
}
