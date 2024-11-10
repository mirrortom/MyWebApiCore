using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using System;

namespace MyWebApi.test;

public class TokenDemo
{
    /// <summary>
    /// 密钥16位,用于实际项目时修改
    /// </summary>
    private const string Key16 = "nq1#*9!3*1285v23";

    /// <summary>
    /// 建立新的token(自定义的token是一个合规的json字符串,不是jwt格式的)
    /// maxTimeSecs:有效时长,单位是秒.默认6000(100分钟)
    /// </summary>
    /// <returns></returns>
    public static string Create(string uid = "0", string uname = "mirror", string rid = "0", string rname = "admin", int maxTimeSecs = 6000)
    {
        // token规则:uid,uname,rid,rname,exprie,sign这6个常用信息组成(这些信息用参数传来)
        dynamic u = new JObject();
        u.UId = uid;
        u.UName = uname;
        u.RId = rid;
        u.RName = rname;
        // 有效期为生成时刻起,加上100分钟
        u.Expire = DateTimeOffset.Now.AddMinutes(maxTimeSecs).ToUnixTimeSeconds();

        // 签名规则:上面5个信息按顺序合并字符串,做sha140摘要
        string signStr = $"{u.UId}{u.UName}{u.RId}{u.RName}{u.Expire}";

        u.Sign = SecurityHelp.Hex40_SHA1(signStr);

        // 生成json字符串,加密返回
        string jsonStr = SerializeHelp.ObjectToJSON(u);

        return SecurityHelp.PlainToCipherHex_Aes(jsonStr, Key16);
    }

    /// <summary>
    /// 验证token签名
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public static bool Check(UserAuth u)
    {
        // 对象化
        if (u == null) return false;

        // check 过期时间检查
        if (u.Expire < DateTimeOffset.Now.ToUnixTimeSeconds())
            return false;

        // 验证签名
        string signStr = $"{u.UId}{u.UName}{u.RId}{u.RName}{u.Expire}";
        string sign = SecurityHelp.Hex40_SHA1(signStr);
        if (sign != u.Sign)
            return false;
        return true;
    }

    /// <summary>
    /// 将token转为用户对象(不验证签名).失败返回null.
    /// </summary>
    /// <returns></returns>
    public static UserAuth TokenToUser(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        // 解密得到json字符串
        string tokenStr = SecurityHelp.CipherHexToPlain_Aes(token, Key16);
        if (string.IsNullOrWhiteSpace(tokenStr))
            return null;

        // 对象化
        var u = SerializeHelp.JsonToObject<UserAuth>(tokenStr);
        if (u == null) return null;

        return u;
    }
}
