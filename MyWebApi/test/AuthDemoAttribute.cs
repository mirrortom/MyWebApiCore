using Microsoft.AspNetCore.Http;

namespace MyWebApi.test
{
    public class AuthDemoAttribute : AUTHAttribute
    {
        public override bool Authenticate(HttpContext content)
        {
            return base.Authenticate(content);
        }
    }
}
