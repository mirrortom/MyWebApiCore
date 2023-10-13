using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using MyWebApi.core;

namespace MyWebApi.test;

/// <summary>
/// 建立服务对象.包装上下文对象,用于服务类的基类
/// </summary>
public class WrapContext
{
    public static IMemoryCache MemoryCache { get; set; }

    //protected DBMO db = DbContext.GetDB();
    protected IMemoryCache cache;
    protected UserAuth user;
    protected ReturnCode result;

    /// <summary>
    /// 初始化服务对象,由子类重写,实现自定义功能
    /// </summary>
    public virtual void Init()
    {

    }

    /// <summary>
    /// 新建服务对象,并设置一些上下文
    /// </summary>
    /// <typeparam name="SRV"></typeparam>
    /// <param name="userauth"></param>
    /// <param name="resultcode"></param>
    /// <returns></returns>
    public static SRV NewSrv<SRV>(UserAuth userauth, ReturnCode resultcode)
        where SRV : WrapContext, new()
    {
        var srv = new SRV
        {
            // 上下文对象
            user = userauth,
            result = resultcode,
            cache = WrapContext.MemoryCache,
            //db = DbContext.GetDBM()
        };
        // 初始化
        srv.Init();
        return srv;
    }
}
