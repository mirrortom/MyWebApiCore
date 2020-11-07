using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MyWebApi
{
    /// <summary>
    /// webapi接口类基类
    /// 本类提供了请求上下文对象和一些便利方法处理参数和返回值
    /// 用来提供数据的接口类(类似webapi的Controller),需要继承这个类.
    /// </summary>
    internal class ApiBase
    {
        #region 请求上下文对象及其它工具属性

        /// <summary>
        /// http请求上下文对象传入,此方法由handler调用
        /// </summary>
        /// <param name="context"></param>
        internal void SetHttpContext(HttpContext context)
        {
            if (this.HttpContext == null)
                this.HttpContext = context;
        }

        /// <summary>
        /// 获取有关单个 HTTP 请求的 HTTP 特定的信息。
        /// </summary>
        protected HttpContext HttpContext { get; private set; }
        /// <summary>
        /// 为当前 HTTP 请求获取 HttpRequestBase 对象。
        /// </summary>
        protected HttpRequest Request
        {
            get { return this.HttpContext.Request; }
        }
        /// <summary>
        /// 为当前 HTTP 响应获取 HttpResponseBase 对象。
        /// </summary>
        protected HttpResponse Response
        {
            get { return this.HttpContext.Response; }
        }


        #endregion

        #region 便利方法,将请求参数转为对象

        /// <summary>
        /// 获取GET参数,并且转为动态类型
        /// 无参数时返回空对象
        /// </summary>
        /// <returns></returns>
        protected virtual dynamic ParaGET()
        {
            dynamic obj = new System.Dynamic.ExpandoObject();
            foreach (string key in this.Request.Query.Keys)
            {
                var values = this.Request.Query[key];
                if (values.Count > 1)
                    ((IDictionary<string, object>)obj).Add(key, values);
                else
                    ((IDictionary<string, object>)obj).Add(key, values.FirstOrDefault());
            }
            return obj;
        }
        /// <summary>
        /// 获取GET参数,并且转为字典类型
        /// 无参数时返回空字典
        /// </summary>
        /// <returns></returns>
        protected virtual Dictionary<string, object> ParaDictGET()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (string key in this.Request.Query.Keys)
            {
                var values = this.Request.Query[key];
                if (values.Count > 1)
                    dict.Add(key, values);
                else
                    dict.Add(key, values.FirstOrDefault());
            }
            return dict;
        }
        /// <summary>
        /// 获取GET参数,并且转为指定类型
        /// 无参数时返回T的实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected virtual T ParaGET<T>()
        {
            dynamic obj = ParaGET();
            string json = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject<T>(json);
        }
        /// <summary>
        /// 获取form参数,并且转为动态类型
        /// 无参数时返回空对象
        /// </summary>
        /// <returns></returns>
        protected virtual dynamic ParaForm()
        {
            dynamic obj = new System.Dynamic.ExpandoObject();
            foreach (string key in this.Request.Form.Keys)
            {
                var values = this.Request.Form[key];
                if (values.Count > 1)
                    ((IDictionary<string, object>)obj).Add(key, values);
                else
                    ((IDictionary<string, object>)obj).Add(key, values.FirstOrDefault());
            }
            return obj;
        }
        /// <summary>
        /// 获取Form参数,并且转为字典类型
        /// 无参数时返回空字典
        /// </summary>
        /// <returns></returns>
        protected virtual Dictionary<string, object> ParaDictForm()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (string key in this.Request.Form.Keys)
            {
                var values = this.Request.Form[key];
                if (values.Count > 1)
                    dict.Add(key, values);
                else
                    dict.Add(key, values.FirstOrDefault());
            }
            return dict;
        }
        /// <summary>
        /// 获取form参数,并且转为指定类型
        /// 无参数时返回T的实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected virtual T ParaForm<T>()
        {
            dynamic obj = ParaForm();
            string json = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject<T>(json);
        }
        /// <summary>
        /// 从InputStream中获取参数.然后返回UTF8编码的字符串.如果是个JSON字符串,可再做转换
        /// 如果get,form都没参数,可尝试这个方法获取.例如Content-Type: application/json类型的参数
        /// 没有取到时返回null
        /// </summary>
        protected virtual string ParaStream()
        {
            byte[] byts = new byte[this.Request.ContentLength.Value];
            Request.Body.Read(byts, 0, byts.Length);
            string json = Encoding.UTF8.GetString(byts);
            return json.Trim();
        }

        //#endregion

        //#region response返回各种结果形式

        /// <summary>
        /// 返回JSON格式数据.obj如果是字符串,则视为json格式字符串直接返回.
        /// </summary>
        /// <param name="obj"></param>
        protected async Task Json(object obj)
        {
            this.Response.ContentType = "application/json;charset=utf-8";
            string jsonstr = obj.GetType() == typeof(string)
                ? obj.ToString() : JsonConvert.SerializeObject(obj);
            await this.Response.WriteAsync(jsonstr);
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
            this.Response.ContentType = "text/html;charset=utf-8";
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
            this.Response.Headers.Add("Content-disposition", $"attachment;filename={HttpUtility.UrlEncode(fileDownloadName)}");
            await this.Response.SendFileAsync(fileName);
        }
        #endregion
    }
}