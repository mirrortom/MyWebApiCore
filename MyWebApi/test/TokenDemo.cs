using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using System;
using System.ServiceProcess;

namespace MyWebApi.test;
/// <summary>
/// 用户管理方案:1.登入后发放一个GUID标志,用户信息放到内存缓存,2小时过期时间.
/// 2.浏览器请求时带上GUID,服务器检查GUID,判断用户,执行任务.
/// </summary>
public class TokenDemo
{
    private static IMemoryCache Cache { get; } = WrapContext.MemoryCache;

    /// <summary>
    /// 新的登录用户与GUID关系映射
    /// maxTimeSecs:有效时长,单位是秒.默认6000(100分钟)
    /// </summary>
    /// <returns></returns>
    public static string Create(string uid, string uname, int rid, string rname, string powid, int maxTimeSecs = 6000)
    {
        // token规则:uid,uname,rid,rname,exprie,sign这6个常用信息组成(这些信息用参数传来)
        var u = new UserAuth
        {
            Uuid = Guid.NewGuid().ToString("N"),
            UId = uid,
            UName = uname,
            RId = rid,
            RName = rname,
            PowId = powid,
            // 有效期为生成时刻起,加上100分钟
            ExpireDT = DateTimeOffset.Now.AddMinutes(maxTimeSecs),
        };

        // 加入缓存
        Cache.Set<UserAuth>(u.Uuid, u, u.ExpireDT);
        return u.Uuid;
    }

    /// <summary>
    /// 验证api调用权限
    /// </summary>
    /// <param name="token"></param>
    /// <param name="apiId">apiId</param>
    /// <returns></returns>
    public static bool Check(string token, int apiId)
    {
        // 检查用户凭证
        if (string.IsNullOrEmpty(token)) return false;
        // 用uuid为健查询缓存
        if (false == Cache.TryGetValue<UserAuth>(token, out var u))
            return false;
        if (u == null) return false;

        // 过期时间检查(这一步可以不必,因为缓存也设置了过期时间,过期后u是null了)
        if (u.ExpireDT < DateTimeOffset.Now)
            return false;

        // 是否有权限执行该方法.特别情况,apiId==-1不需要权限.powid=0为用户最高权限
        if (apiId == -1 || u.PowId == "0")
        {
            return true;
        }
        // 用户权限字符串包含该方法ID,就有权执行
        string[] powidArr = u.PowId.Split('|');
        return powidArr.IndexOf(apiId.ToString()) != -1;
    }
}
