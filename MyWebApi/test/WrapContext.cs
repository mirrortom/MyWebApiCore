using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace MyWebApi.test;

/// <summary>
/// 建立服务对象.包装上下文对象,用于服务类的基类
/// </summary>
public class WrapContext
{
    /// <summary>
    /// 内存缓存
    /// </summary>
    public static IMemoryCache MemoryCache { get; } = new MemoryCache(new MemoryCacheOptions());

    //protected DBMO db = DbContext.GetDB();
    /// <summary>
    /// 内存取缓存
    /// </summary>
    public IMemoryCache Cache { get; } = MemoryCache;
    /// <summary>
    /// 登陆者信息
    /// </summary>
    public UserAuth User { get; } = new();
    /// <summary>
    /// 返回错误码
    /// </summary>
    public ReturnCode Result { get; } = new();

    /// <summary>
    /// 初始化服务对象,由子类重写,实现自定义功能
    /// </summary>
    public virtual void Init()
    {

    }

    /// <summary>
    /// 新建服务对象
    /// </summary>
    /// <typeparam name="SRV"></typeparam>
    /// <param name="userauth"></param>
    /// <param name="resultcode"></param>
    /// <returns></returns>
    public static SRV NewSrv<SRV>(HttpContext httpContext)
        where SRV : WrapContext, new()
    {
        var srv = new SRV();
        UserSet(httpContext, srv.User);

        // 继承类的重写Init方法
        srv.Init();

        return srv;
    }

    /// <summary>
    /// 请求者信息设置
    /// </summary>
    private static void UserSet(HttpContext httpContext, UserAuth user)
    {
        // 请求者可能是匿名的(没有token).包含来源IP,等常用数据
        // 从token信息得到的user实例.在添加了[AUTH]的API类或者方法上,token是验证过的.
        var u = TokenDemo.TokenToUser(httpContext.Request.Headers["Auth"]);
        // 是有token的用户,但token可能无效
        if (u != null)
        {
            user.UId = u.UId;
            user.UName = u.UName;
            user.RId = u.RId;
            user.RName = u.RName;
        }
        // 其它数据
        user.Ip = httpContext.Connection.RemoteIpAddress.ToString();
        user.Port = httpContext.Connection.RemotePort.ToString();
    }
}
