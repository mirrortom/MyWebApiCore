using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MyWebApi
{
    // 自定义中间件
    internal class ApiHandler
    {
        /// <summary>
        /// 分析url地址然后执行对应的类和方法.(中间件)
        /// </summary>
        /// <param name="next"></param>
        /// <returns></returns>
        internal static async Task UrlMapMethodMW(HttpContext context)
        {
            // 程序集名(当前运行此代码的程序集)
            string assbName = Assembly.GetExecutingAssembly().GetName().Name;

            // 分解URL路径用于找类名和方法名
            string[] urlparts = context.Request.Path.Value.Split('/');

            // 如果路径不合规则,引发异常.合法路径 /api/user/info , /user/name
            // 如果开启了静态文件,url所在文件没找到时,也会进入这里
            if (urlparts.Length < 3)
                throw new Exception($"Url is invalid! [{urlparts}]");

            // 类名和方法名:类名全称是- 程序集名.[命名空间].类名Api(约定后缀)
            // 命名空间: url拆分后,0位是命名空间,如果只有两段,则没有命名空间(只有当前程序集的默认命名空间)
            // 后缀约定: 为了统一书写,一个成为api的类,结尾为Api.例如: UserApi
            string apiClass = "", apiMethod = "";
            if (urlparts.Length < 4)
            {
                apiClass = $"{assbName}.{urlparts[1]}Api";
                apiMethod = urlparts[2];
            }
            else
            {
                apiClass = $"{assbName}.{urlparts[1]}.{urlparts[2]}Api";
                apiMethod = urlparts[3];
            }

            // 到当前程序集中寻找这个类
            Type webapiT = Assembly.GetExecutingAssembly().GetType(apiClass, false, true);

            // 未找到该类,引发异常
            if (webapiT == null)
                throw new Exception($"Class not found! [{apiClass}]");

            // 建立实例,再传入HttpContext对象
            // 继承约定: API类需要继承ApiBase这个基类
            ApiBase workapi = (ApiBase)Activator.CreateInstance(webapiT, true);
            workapi.SetHttpContext(context);

            // 到类中寻找方法
            MethodInfo webapiMethod = webapiT.GetMethods().FirstOrDefault(o => string.Compare(o.Name, apiMethod, true) == 0);

            // 未找到方法,引发异常
            if (webapiMethod == null)
                throw new Exception($"Method not found! [{apiClass}.{apiMethod}]");

            // 检查方法是否贴有特性HTTPPOST/HTTPGET/HTTPALL. 例如[HTTPPOST],只响应POST请求.
            // 特性约定:做为API的方法需要贴上三个特性中的一个
            if (!AttributeCheck(context, webapiMethod))
                throw new Exception($"Method not WebApi! [{apiClass}.{apiMethod}]");

            // 检查类或者方法上是否贴有AUTH特性,有则执行权限判断
            if (!AuthCheck(context, webapiMethod, webapiT))
                throw new Exception($"Method Access Denied! [{apiClass}.{apiMethod}]");

            // 
            await Task.Factory.StartNew(() =>
            {
                webapiMethod.Invoke(workapi, null);
            });
        }

        /// <summary>
        /// 自定义处理异常,返回简要异常信息.(中间件)
        /// 可以设置多个处理方法
        /// </summary>
        /// <returns></returns>
        internal static void CustomExceptMW(IApplicationBuilder exBuild)
        {
            async Task exHandler1(HttpContext context)
            {
                // 从上下文对象中获取异常对象.
                //var exh = context.Features.Get<IExceptionHandlerPathFeature>();
                // 返回异常信息
                context.Response.ContentType = "application/json;charset=utf-8";
                context.Response.StatusCode = 200;
                string errJsonStr = JsonConvert.SerializeObject(
                    new
                    {
                        //errmsg = exh.Error.Message,
                        errmsg = "server faild!",
                        errcode = 501
                    });
                await context.Response.WriteAsync(errJsonStr);
            }
            // Run是最后一个处理方法,如果还要传导,用Use()
            exBuild.Run(exHandler1);
        }

        /// <summary>
        /// 接口权限判断.当webapi类或其方法贴有AUTH特性时.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="webapiMethod"></param>
        /// <param name="webapiType"></param>
        /// <returns></returns>
        private static bool AuthCheck(HttpContext context, MethodInfo webapiMethod, Type webapiType)
        {
            if (Attribute.IsDefined(webapiType, typeof(AUTHAttribute)))
            {
                var auth = webapiType.GetCustomAttribute<AUTHAttribute>();
                return auth.Authenticate(context);
            }
            if (Attribute.IsDefined(webapiMethod, typeof(AUTHAttribute)))
            {
                var auth = webapiMethod.GetCustomAttribute<AUTHAttribute>();
                return auth.Authenticate(context);
            }
            return true;
        }

        /// <summary>
        /// 方法特性检查.是否贴有作为接口的三个特性
        /// 并非必要,只是为了加一个功能,让贴了特性的方法才能被访问.没贴的当内部方法
        /// </summary>
        /// <param name="webapiMethod">要检查的接口方法</param>
        /// <returns></returns>
        private static bool AttributeCheck(HttpContext context, MethodInfo webapiMethod)
        {
            string httpMethod = context.Request.Method.ToUpper();
            if (httpMethod == "POST" && Attribute.IsDefined(webapiMethod, typeof(HTTPPOSTAttribute)))
            {
                return true;
            }
            else if (httpMethod == "GET" && Attribute.IsDefined(webapiMethod, typeof(HTTPGETAttribute)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    #region 功能特性
    // 特性贴在webapi的方法上,AUTH特性也可以贴在类上
    public class WebApiBaseAttribute : Attribute
    {
        /// <summary>
        /// 接口功能描述
        /// </summary>
        public string Desc { get; set; }
        /// <summary>
        /// 接口id(为每一个接口分配一个整数ID,用于权限判断)
        /// </summary>
        public int Id { get; set; }
    }
    public class AUTHAttribute : WebApiBaseAttribute
    {
        // 判断解析token,检查登录者信息
        // string token = context.Request.Headers["Auth"].ToString();
        public virtual bool Authenticate(HttpContext content) => true;
    }
    public class HTTPPOSTAttribute : WebApiBaseAttribute
    {

    }
    public class HTTPGETAttribute : WebApiBaseAttribute
    {

    }
    #endregion
}