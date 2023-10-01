namespace MyWebApi;

/// <summary>
/// 登录者信息,可继承此类
/// </summary>
public class UserAuth
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int RId { get; set; }
    public string RName { get; set; }
    public long Expire { get; set; }
    public string Sign { get; set; }
}