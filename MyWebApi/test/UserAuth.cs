namespace MyWebApi.test;

/// <summary>
/// 登录者信息
/// </summary>
public class UserAuth
{
    /// <summary>
    /// 前端标识.使用guid
    /// </summary>
    public string Uuid { get; set; }
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
    /// api权限列表,0=所有权限,其它情况每个权限id数字用|分开.1|2|3
    /// </summary>
    public string PowId{ get; set;  }
    /// <summary>
    /// 过期时间 unix时间戳(总秒数)
    /// </summary>
    public long Expire { get; set; }
    /// <summary>
    /// 过期时间
    /// </summary>
    public DateTimeOffset ExpireDT { get; set; }
    /// <summary>
    /// 远端IP地址
    /// </summary>
    public string Ip { get; set; }
    /// <summary>
    /// 远端口号
    /// </summary>
    public string Port { get; set; }
    /// <summary>
    /// 用户代理信息
    /// </summary>
    public string UserAgent{  get; set; }
}