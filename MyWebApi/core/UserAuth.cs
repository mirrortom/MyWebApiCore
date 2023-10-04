namespace MyWebApi;

/// <summary>
/// 登录者信息,可继承此类
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
    /// 过期时间
    /// </summary>
    public long Expire { get; set; }
    /// <summary>
    /// 签名
    /// </summary>
    public string Sign { get; set; }
}