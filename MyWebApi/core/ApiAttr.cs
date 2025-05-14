using Microsoft.AspNetCore.Http;
using System;

namespace MyWebApi.core;

/// <summary>
/// 指定接受httpPOST方式请求
/// </summary>
internal class HTTPPOSTAttribute : Attribute
{
}

/// <summary>
/// 指定接受httpGET方式请求
/// </summary>
internal class HTTPGETAttribute : Attribute
{
}

/// <summary>
/// 自定义路由时,设置地址.如果有重复的地址,在服务启动时会报错.
/// </summary>
internal class ROUTEAttribute(string url) : Attribute
{
    /// <summary>
    /// 路由地址
    /// </summary>
    public string Url { get; private set; } = url;
}

/// <summary>
/// 验证,用于API类或方法
/// </summary>
internal class AUTHBaseAttribute : Attribute
{
    /// <summary>
    /// <para>判断解析token,检查登录者信息,按需重写本方法</para>
    /// </summary>
    /// <returns></returns>
    internal virtual bool Authenticate(HttpContext context) => true;
    /// <summary>
    /// 方法唯一标识
    /// </summary>
    public int Id { get; set; }
}
