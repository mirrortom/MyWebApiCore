﻿using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MyWebApi.core;

/// <summary>
/// webapi接口类基类
/// 本类提供了请求上下文对象和一些便利方法处理参数和返回值
/// 用来提供数据的接口类(类似webapi的Controller),需要继承这个类.
/// </summary>
public class ApiBase
{
    #region 请求上下文对象及其它工具属性

    /// <summary>
    /// ApiHandler.UrlMapMethodMW(),URL中间件调用此方法设定请求上下文对象和其它功能
    /// </summary>
    /// <param name="context"></param>
    internal void SetHttpContext(HttpContext context)
    {
        this.HttpContext = context;
        this.Request = context.Request;
        this.Response = context.Response;
    }

    /// <summary>
    /// 为当前 HTTP 请求获取 HttpRequestBase 对象,来自HttpContext.Request
    /// </summary>
    protected HttpRequest Request { get; private set; }

    /// <summary>
    /// 为当前 HTTP 响应获取 HttpResponseBase 对象,来自HttpContext.Response
    /// </summary>
    protected HttpResponse Response { get; private set; }

    /// <summary>
    /// 获取有关单个 HTTP 请求的 HTTP 特定的信息.(可直接使用其它便利属性),由ApiHandler的URL中间件设定
    /// </summary>
    protected HttpContext HttpContext { get; private set; }

    #endregion 请求上下文对象及其它工具属性

    #region 便利方法,将请求参数转为对象
    /// <summary>
    /// 获取GET参数,并且转为字典类型
    /// 无参数时返回空字典
    /// </summary>
    /// <returns></returns>
    protected virtual Dictionary<string, string> ParaDictGET()
    {
        Dictionary<string, string> dict = [];
        foreach (string key in this.Request.Query.Keys)
        {
            var values = this.Request.Query[key];
            dict.Add(key, values.Count > 1 ? values : values.FirstOrDefault());
        }
        return dict;
    }

    /// <summary>
    /// 获取GET参数,并且转为动态类型
    /// 无参数时返回空对象(JObject)
    /// </summary>
    /// <returns></returns>
    protected virtual dynamic ParaGET()
    {
        var dict = ParaDictGET();
        dynamic obj = JObject.FromObject(dict);
        return obj;
    }

    /// <summary>
    /// 获取GET参数,并且转为T类型
    /// 无参数时返回T的实例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    protected virtual T ParaGET<T>()
    {
        var obj = this.ParaDictGET();
        string json = JsonConvert.SerializeObject(obj);
        return JsonConvert.DeserializeObject<T>(json);
    }

    // 关于表单参数读取的性能建议文档
    // https://learn.microsoft.com/zh-cn/aspnet/core/performance/performance-best-practices?view=aspnetcore-7.0#prefer-readformasync-over-requestform

    /// <summary>
    /// 获取Form参数,并且转为字典类型
    /// 无参数时返回空字典
    /// </summary>
    /// <returns></returns>
    protected virtual async Task<Dictionary<string, object>> ParaDictForm()
    {
        Dictionary<string, object> dict = [];
        var form = await this.Request.ReadFormAsync();
        foreach (var key in form.Keys)
        {
            var values = form[key];
            dict.Add(key, values.Count > 1 ? values : values.FirstOrDefault());
        }
        return dict;
    }

    /// <summary>
    /// 获取form参数,并且转为动态类型
    /// 无参数时返回空对象(JObject)
    /// </summary>
    /// <returns></returns>
    protected virtual async Task<dynamic> ParaForm()
    {
        var dict = await ParaDictForm();
        dynamic obj = JObject.FromObject(dict);
        return obj;
    }

    /// <summary>
    /// 获取form参数,并且转为指定类型
    /// 无参数时返回T的实例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    protected virtual async Task<T> ParaForm<T>()
    {
        dynamic obj = await this.ParaDictForm();
        string json = JsonConvert.SerializeObject(obj);
        return JsonConvert.DeserializeObject<T>(json);
    }

    // 关于对body参数异步读取的建议文档
    // https://learn.microsoft.com/zh-cn/aspnet/core/performance/performance-best-practices?view=aspnetcore-7.0#avoid-synchronous-read-or-write-on-httprequesthttpresponse-body

    /// <summary>
    /// <para>读取body参数,然后返回字符串.如果是个JSON字符串,可再做转换.</para>
    /// <para>如果get,form都没参数,可尝试这个方法获取.例如Content-Type: application/json类型的参数</para>
    /// <para>如果参数太大,会占用很多内存,方法是先读取到内存中的.</para>
    /// <para>没有取到时返回string.Empty</para>
    /// </summary>
    protected virtual async Task<string> ParaStream()
    {
        // 可能为string.Empty
        return await new StreamReader(this.Request.Body).ReadToEndAsync();
    }

    #endregion 便利方法,将请求参数转为对象

    #region response返回几种结果形式

    // 关于对返回结果的性能建议文档
    // 不建议用同步方式写入返回结果
    // https://learn.microsoft.com/zh-cn/aspnet/core/performance/performance-best-practices?view=aspnetcore-7.0#avoid-synchronous-read-or-write-on-httprequesthttpresponse-body

    // 误区
    // Response返回方法做成一个Task方法,在webapi中可以异步调用.

    /// <summary>
    /// 返回JSON格式数据.obj如果是字符串,则视为json格式字符串直接返回.
    /// </summary>
    /// <param name="obj"></param>
    protected async Task Json(object obj)
    {
        this.Response.ContentType = "application/json;charset=utf-8";
        if (!(obj is string s))
        {
            s = JsonConvert.SerializeObject(obj);
        }
        await this.Response.WriteAsync(s);
    }

    /// <summary>
    /// 返回一段HTML格式文本
    /// </summary>
    /// <param name="html"></param>
    protected async Task Html(string html)
    {
        this.Response.ContentType = "text/html;charset=utf-8";
        await this.Response.WriteAsync(html);
    }

    /// <summary>
    /// 返回纯文本格式字符串
    /// </summary>
    /// <param name="text"></param>
    protected async Task Text(string text)
    {
        this.Response.ContentType = "text/plain;charset=utf-8";
        await this.Response.WriteAsync(text);
    }

    /// <summary>
    /// 返回文件,需要指定文件内容头型
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="contentType"></param>
    protected async Task File(string fileName, string contentType)
    {
        this.Response.ContentType = $"{contentType};charset=utf-8";
        await this.Response.SendFileAsync(fileName);
    }

    /// <summary>
    /// 返回文件,指定文件内容头型,下载文件显示名
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="contentType"></param>
    /// <param name="fileDownloadName"></param>
    protected async Task File(string fileName, string contentType, string fileDownloadName)
    {
        this.Response.ContentType = $"{contentType};charset=utf-8";
        this.Response.Headers.Append("Content-disposition", $"attachment;filename={HttpUtility.UrlEncode(fileDownloadName)}");
        await this.Response.SendFileAsync(fileName);
    }

    #endregion response返回几种结果形式
}
