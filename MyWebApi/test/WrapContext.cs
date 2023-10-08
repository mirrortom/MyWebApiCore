using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;

namespace MyWebApi.test;

/// <summary>
/// 包装各种上下文对象,用于传递
/// </summary>
public class WrapContext
{
    public static IMemoryCache MemoryCache { get; set; }

    protected readonly IMemoryCache cache = WrapContext.MemoryCache;
    protected UserAuth user;
    protected ResultContext result;

    public void SetContext(UserAuth userauth, ResultContext resultcontext)
    {
        this.user = userauth;
        this.result = resultcontext;
    }
}
