using Microsoft.AspNetCore.Http;

namespace MyWebApi.test
{
    internal class User : IUser
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
    /// <summary>
    /// 验证侵权者
    /// </summary>
    internal class AuthDemoAttribute : AUTHAttribute
    {
        internal override bool Authenticate(HttpContext context, ApiBase apiInstance)
        {
            string token = context.Request.Headers["Auth"];
            if (token == "0123456789")
            {
                // 设定请求者信息
                apiInstance.SetUser(new User() { Id = "0", Name = "Mirror" });
                return true;
            }
            return false;
        }
    }

}
