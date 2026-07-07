using Microsoft.Extensions.Caching.Memory;

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
    private IMemoryCache Cache { get; } = MemoryCache;
    /// <summary>
    /// 登陆者信息
    /// </summary>
    public UserAuth User { get; private set; } = new();
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
    /// 新建服务对象.
    /// 由webapi调用.
    /// </summary>
    /// <typeparam name="SRV"></typeparam>
    /// <param name="userauth"></param>
    /// <param name="resultcode"></param>
    /// <returns></returns>
    public static SRV NewSrv<SRV>(HttpContext httpContext)
        where SRV : WrapContext, new()
    {
        var srv = new SRV();
        // 来访者信息
        srv.UserSet(httpContext);

        // 继承类的重写Init方法
        srv.Init();

        return srv;
    }

    /// <summary>
    /// 获取请求者信息
    /// 未登录或者匿名用户,user含有ip,port,useragent等数据.
    /// 登录用户,user包含验证过的用户信息
    /// </summary>
    private void UserSet(HttpContext httpContext)
    {
        // 认证用户,从缓存中获取到.
        var auth = httpContext.Request.Headers["Auth"];
        // Headers里面的值是StringValues类型,是一个或者多个字符串,需要使用[index]
        string token = string.IsNullOrEmpty(auth) ? "" : (auth[0] ?? "");
        var u = Cache.Get<UserAuth>(token);
        // 否则是匿名用户
        u ??= new UserAuth();
        //
        this.User = u;

        // 其它数据
        this.User.Ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "";
        this.User.Port = httpContext.Connection.RemotePort.ToString();
        var userAgent = httpContext.Request.Headers.UserAgent;
        this.User.UserAgent = string.IsNullOrEmpty(userAgent) ? ""
        : (userAgent[0] ?? "");
    }
}
