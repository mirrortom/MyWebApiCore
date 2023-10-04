using System;

namespace MyWebApi.test;

public class TokenDemo
{
    /// <summary>
    /// 密钥16位
    /// </summary>
    private const string Key16 = "nq1#*9!3*1285v23";

    /// <summary>
    /// 建立新的token(自定义的token是一个合规的json字符串,不是jwt格式的)
    /// </summary>
    /// <returns></returns>
    public static string Create()
    {
        // token规则:uid,uname,rid,rname,exprie,sign这6个常用信息组成(这些信息用参数传来)
        UserAuth u = new()
        {
            UId = "0",
            UName = "mirror",
            RId = 0,
            RName = "admin",
            // 有效期为生成时刻起,加上2个小时
            Expire = DateTime.Now.AddHours(2).Ticks
        };

        // 签名规则:上面5个信息按顺序合并字符串做sha140摘要
        string signStr = $"{u.UId}{u.UName}{u.RId}{u.RName}{u.Expire}";

        u.Sign = SecurityHelp.Hex40_SHA1(signStr);

        // 生成json字符串,加密返回
        string jsonStr = SerializeHelp.ObjectToJSON(u);

        return SecurityHelp.PlainToCipherHex_Aes(jsonStr, Key16);
    }

    /// <summary>
    /// 检查客户端发来的token
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public static bool Check(string token, UserAuth user)
    {
        if (string.IsNullOrWhiteSpace(token))
            return false;

        // 解密得到json字符串
        string tokenStr = SecurityHelp.CipherHexToPlain_Aes(token, Key16);
        if (string.IsNullOrWhiteSpace(tokenStr))
            return false;

        // 对象化
        UserAuth u = SerializeHelp.JsonToObject<UserAuth>(tokenStr);
        if (u == null) return false;

        // check 过期时间检查
        long passedtime = DateTime.Now.Ticks - u.Expire;
        if (passedtime >= 0)
            return false;

        // 验证签名
        string signStr = $"{u.UId}{u.UName}{u.RId}{u.RName}{u.Expire}";
        string sign = SecurityHelp.Hex40_SHA1(signStr);
        if (sign != u.Sign)
            return false;

        // 登录者信息保存
        user.UId = u.UId;
        user.UName = u.UName;
        user.RId = u.RId;
        user.RName = u.RName;
        return true;
    }
}
