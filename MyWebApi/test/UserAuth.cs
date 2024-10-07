namespace MyWebApi.test;

/// <summary>
/// 登录者信息
/// </summary>
public class UserAuth
{
    /// <summary>
    /// 用户id
    /// </summary>
    public string UId { get; set; }
    /// <summary>
    /// 用户名字
    /// </summary>
    public string UName { get; set; }
    /// <summary>
    /// 角色id
    /// </summary>
    public int RId { get; set; }
    /// <summary>
    /// 角色名字
    /// </summary>
    public string RName { get; set; }
    /// <summary>
    /// 过期时间 生成时间加上有效期(分钟),转为unix时间戳(总秒数)
    /// </summary>
    public long Expire { get; set; }
    /// <summary>
    /// 签名
    /// </summary>
    public string Sign { get; set; }
    /// <summary>
    /// 远端IP地址
    /// </summary>
    public string Ip { get; set; }
    /// <summary>
    /// 远端口号
    /// </summary>
    public string Port { get; set; }
}