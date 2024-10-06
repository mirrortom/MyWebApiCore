namespace MyWebApi.core;

/*
 * 单独建立这个返回值类,可以从API类传递给业务处理类,便于由业务处理类设置返回消息和代码.
 * 一般情况下简单的返回消息在API层可以完成.业务处理类提供更详细的消息.
 */

/// <summary>
/// 返回值代码和消息
/// </summary>
public class ReturnCode
{
    public string ErrMsg;
    public int ErrCode;
}
