namespace MyWebApi.test;

/// <summary>
/// 返回结果包装
/// </summary>
public class ResultContext
{
    /// <summary>
    /// 错误提示信息
    /// </summary>
    public string ErrorMsg;
    /// <summary>
    /// 错误提示代码 约定200=成功 
    /// </summary>
    public int ErrorCode;
}
