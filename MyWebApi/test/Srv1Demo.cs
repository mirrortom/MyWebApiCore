using System;

namespace MyWebApi.test;

public class Srv1Demo : WrapContext
{
    public string info()
    {
        return $"来自{nameof(Srv1Demo)}服务的消息new guid: {Guid.NewGuid()} 时间: {DateTime.Now}";
    }
}
