using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using System.IO;

namespace MyWebApi
{
    /// <summary>
    /// 载入配置,并提供配置项
    /// </summary>
    internal static class CustomSetting
    {
        // 监听地址
        internal static string[] Urls { get; private set; }
        // 默认文档配置项
        internal static DefaultFilesOptions DefaultDocOptions { get; private set; }
        // 虚拟目录
        internal static StaticFileOptions[] VirtualDirsOptions { get; private set; }


        /// <summary>
        /// 加载配置
        /// </summary>
        internal static void Load()
        {
            // 读取自定义配置json
            using StreamReader sr = new("customsetting.json");
            string jsonStr = sr.ReadToEnd();
            dynamic setjson = JsonConvert.DeserializeObject(jsonStr);

            // 默认文档配置项
            DefaultDocOptionsSet((string)setjson.defaultDoc);

            // urls监听地址
            UrlsSet((string)setjson.urls);

            // 虚拟目录
            VirtualDirsOptionsSet(setjson.virtualDir);
        }

        /// <summary>
        /// 默认文档设定
        /// </summary>
        /// <param name="docNames"></param>
        private static void DefaultDocOptionsSet(string docNames)
        {
            DefaultDocOptions = new DefaultFilesOptions();
            string[] docs = docNames.Split(';');
            foreach (var item in docs)
            {
                DefaultDocOptions.DefaultFileNames.Add(item);
            }
        }

        /// <summary>
        /// urls监听地址设定
        /// </summary>
        /// <param name="urlsstr"></param>
        private static void UrlsSet(string urlsstr)
        {
            Urls = urlsstr.Split(';');
        }

        // 静态文件配置项
        // 文档: https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/static-files?view=aspnetcore-7.0
        /// <summary>
        /// 虚拟目录设定
        /// </summary>
        /// <param name="virtualDir"></param>
        private static void VirtualDirsOptionsSet(dynamic virtualDir)
        {
            // virtualDir: [{fdir:"物理路径(相对web根目录)",refdir:"url映射路径,斜杠/打头"}]
            if (virtualDir == null || virtualDir.Count == 0)
                return;
            VirtualDirsOptions = new StaticFileOptions[virtualDir.Count];
            for (int i = 0; i < VirtualDirsOptions.Length; i++)
            {
                dynamic item = virtualDir[i];
                VirtualDirsOptions[i] = new StaticFileOptions()
                {
                    // 这里配置物理目录
                    FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), (string)item.fdir)),
                    // 配置对应虚拟目录,就是url请求上的目录
                    RequestPath = (string)item.refdir
                };
            }
        }

        /// <summary>
        /// 跨域策略 选项
        /// https://learn.microsoft.com/zh-cn/aspnet/core/security/cors?view=aspnetcore-6.0#uc1
        /// </summary>
        /// <param name="option"></param>
        internal static void CorsConfigBuild(CorsPolicyBuilder option)
        {
            option.AllowAnyHeader();
            option.AllowAnyMethod();
            option.AllowAnyOrigin();
            //cfg.AllowCredentials();
        }
    }
}
