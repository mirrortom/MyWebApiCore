using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.AccessControl;
using System.Threading.Tasks;

using static MyWebApi.core.RouteMap;

namespace MyWebApi.core;

/*
 *---中间件说明---
 *  中间件是一个RequestDelegate类型,它是一个委托,含有一个参数(类型HttpContext)和一个返回值
 *(类型Task).
 *  ProcReqMW中间件的任务是查找处理方法,并执行这个方法,然后方法会返回一个Task对象,框架将会处
 *理这个Task.
 *  曾经遇到的麻烦:如果将处理方法再包装到一个Task对象里,然后返回,结果异常:
 *               涉嫌在请求结束后,使用HttpContent
 *               涉嫌多个线程操作httpContext,而它线程不安全...
 *               这些情况是由于对Task/await/async异步程序不熟悉.
 *  具体看文档:
 *  https://learn.microsoft.com/zh-cn/aspnet/core/performance/performance-best-practices?view=aspnetcore-7.0#do-not-access-httpcontext-from-multiple-threads
 *  另外,如果用同步的webapiMethod,则没这么多问题,但官方文档推荐异步读写请求...具体看文档
 *  https://learn.microsoft.com/zh-cn/aspnet/core/performance/performance-best-practices?view=aspnetcore-7.0#avoid-synchronous-read-or-write-on-httprequesthttpresponse-body
*/

/*   
 *---性能问题---
 *  由于每次都要反射查询类和方法,所以效率很低,反射查询类比直接使用类要慢上5倍左右.密集请求的情况下,
 *效率不高.
 *  在程序启动时,将所有符合要求的处理类和它的方法的信息对象,反射查询出来并且缓存在字典,避免在请求处
 *理时临时查询,而通过查字典找到方法信息.这样效率比直接写慢2倍左右,可以接受了.
 *  效率最高的方法是直接new和调用方法,可以手动写到字典里,但这样很麻烦啊,即时用生成器也麻烦.
 *
 *  尝试通过委托调用遇到的问题:
 *  问题: 预先准备方法的委托(无实例对象时的开放式委托)以提高执行效率,但是做过很多尝试都没成功!
 *  尝试: 建立有实例对象的委托不行,对象必须在请求过程中建立,请求结束时释放,不能缓存.
 *       由于类型不能确定,实例开放式委托无法建立,虽然可以建立为Delegate类型,但是在调用时又无
 *       法转换为实际类型,而开放式委托要传入确定类型参数,传入基类WebApiBase不行的.
 *       虽然可以用DynamicInvoke调用,但性能还不如直接Invoke.
 *  最后: 还是直接使用Invoke.检查参数信息,传入默认类型参数.
*/

/// <summary>
/// url解析中间件,异常处理中间件
/// </summary>
internal class ApiHandler
{
    /// <summary>
    /// 匹配地址,找到处理类,执行处理逻辑以响应请求.是最后一个中间件.
    /// </summary>
    /// <returns></returns>
    internal static Task? ProcReqMW(HttpContext context)
    {
        // 1. 分析地址:Path.Value的值不含协议名字(http),主机名字(www.xx.com),?后面的参数(?a=1)
        // 例如: /api/emp/getlist
        var routeName = context.Request.Path.Value?[1..];
        // 地址为空?
        if (string.IsNullOrEmpty(routeName))
        {
            return ApiHandler.HtmlErrorEnd(context, 404, "URL无效!");
        }
        var isMap = RouteMap.HttpGetMap.TryGetValue(routeName, out RouteAction routeAction)
        || RouteMap.HttpPostMap.TryGetValue(routeName, out routeAction);

        // 没有找到资源时
        if (isMap == false || routeAction == null)
        {
            return ApiHandler.HtmlErrorEnd(context, 404, "页面没找到!");
        }

        // 2. 如果有验证,执行验证方法
        // api类没有实例化,但上面的AUTH特性可以执行
        if (routeAction.AuthAttr != null)
        {
            if (routeAction.AuthAttr.Authenticate(context) == false)
            {
                return ApiHandler.HtmlErrorEnd(context, 403, "拒绝访问!");
            }
        }

        // 3. 实例化类
        if (Activator.CreateInstance(routeAction.InstanceType) is not ApiBase api)
        {
            return ApiHandler.HtmlErrorEnd(context, 500, "服务器错误,请求处理失败!");
        }
        api.SetHttpContext(context);
        // 4.调用方法

        // 执行方法
        return routeAction.ActionInfo.Invoke(api, routeAction.GetParasForInvoke()) as Task;
    }

    /// <summary>
    /// 自定义处理异常,用于生产环境,返回一般异常信息提示(中间件)
    /// </summary>
    /// <returns></returns>
    internal static void CustomExceptMW(IApplicationBuilder exBuild)
    {
        static async Task exHandler1(HttpContext context)
        {
            // 从上下文对象中获取异常对象.
            //var exh = context.Features.Get<IExceptionHandlerPathFeature>();
            await ApiHandler.HtmlErrorEnd(context, 500, "服务器发生异常!");
        }
        // Run是最后一个处理方法,如果还要传导,用Use()
        exBuild.Run(exHandler1);
    }

    /// <summary>
    /// 返回一个html格式的错误页面
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected static async Task HtmlErrorEnd(HttpContext context, int statusCode, string errmsg)
    {
        var html = $"<html><head></head><body><h1>{statusCode}</h1><p>{errmsg}</p></body></html>";
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "text/html;charset=utf-8";
        await context.Response.WriteAsync(html);
    }
}