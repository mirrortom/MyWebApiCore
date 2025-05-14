using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using MyWebApi.test;
using System.Reflection;

namespace MyWebApi.core;

/*
 * 自定义路由,映射url和处理方法
 */

internal class RouteMap
{
    /// <summary>
    /// httpGet字典
    /// </summary>
    public static Dictionary<string, RouteAction> HttpGetMap { get; private set; }

    /// <summary>
    /// httpPost方法字典
    /// </summary>
    public static Dictionary<string, RouteAction> HttpPostMap { get; private set; }

    public static void Init()
    {
        HttpGetMap = new(StringComparer.OrdinalIgnoreCase);
        HttpPostMap = new(StringComparer.OrdinalIgnoreCase);

        // 1.查找所有继承了ApiBase的类
        // 2.找出类里所有贴有httpget或者httppost或者两者都有的方法
        // 3.找出方法上贴有的验证特性
        var classList = Assembly.GetExecutingAssembly()
                   .GetTypes()
                   .Where(t => t.BaseType == typeof(ApiBase));

        foreach (var t in classList)
        {
            var methods = t.GetMethods();
            foreach (var m in t.GetMethods())
            {
                // 方法的返回类型必须是Task
                if (m.ReturnType != typeof(Task))
                    continue;

                // 数据类
                RouteAction r = NewRouteAction(t, m);

                // 2. 生成地址
                string routeName = CreateRouterName(t, m, r);

                // 3. 按http方法类型加入字典,键是地址,值是RouteAction对象
                if (r.IsGet)
                {
                    if (HttpGetMap.ContainsKey(routeName))
                    {
                        throw new Exception($"router键重复(GET方式)!请检查类和方法: 类名[ {t.FullName} ],方法名[ {m.Name} ]");
                    }
                    HttpGetMap.Add(routeName, r);
                }
                if (r.IsPost)
                {
                    if (HttpPostMap.ContainsKey(routeName))
                    {
                        throw new Exception($"router键重复(POST方式)!请检查类和方法: 类名[ {t.FullName} ],方法名[ {m.Name} ]");
                    }
                    HttpPostMap.Add(routeName, r);
                }
            }
        }
    }

    private static RouteAction NewRouteAction(Type t, MethodInfo m)
    {
        return new()
        {
            InstanceType = t,
            ActionInfo = m,
            IsGet = Attribute.IsDefined(m, typeof(HTTPGETAttribute)),
            IsPost = Attribute.IsDefined(m, typeof(HTTPPOSTAttribute)),
            AuthAttr = t.GetCustomAttribute<AUTHBaseAttribute>(false)
                            ?? m.GetCustomAttribute<AUTHBaseAttribute>(false),
            RouteAttr = m.GetCustomAttribute<ROUTEAttribute>()
        };
    }


    /// <summary>
    /// 生成路由地址,(将作为字典的键)
    /// </summary>
    /// <param name="t"></param>
    /// <param name="m"></param>
    /// <param name="r"></param>
    /// <returns></returns>
    private static string CreateRouterName(Type t, MethodInfo m, RouteAction r)
    {
        string defNsName = Assembly.GetExecutingAssembly().GetName().Name ?? string.Empty;
        // 默认地址规则:[part2NamespaceName/*]className/methodName
        // 如果类全名以默认命名空间开头.去掉默认部分,例如:MyWebApi.Api.DempApi
        // 去掉默认命名空间 MyWebApi,取用 Api.DemoApi
        string classPart = t.FullName[(defNsName.Length + 1)..].Replace('.', '/');
        if (classPart.EndsWith("Api", StringComparison.OrdinalIgnoreCase))
            classPart = classPart[..^3];
        var routeName = $"{classPart}/{m.Name}";

        // 如果使用了自定义路由地址,覆盖默认的
        if (r.RouteAttr != null && !string.IsNullOrEmpty(r.RouteAttr.Url))
        {
            routeName = r.RouteAttr.Url;
        }

        return routeName;
    }

    /// <summary>
    /// 路由请求处理的类和方法信息
    /// </summary>
    internal class RouteAction
    {
        /// <summary>
        /// api对象类型
        /// </summary>
        public Type InstanceType { get; set; }

        /// <summary>
        /// api方法信息
        /// </summary>
        public MethodInfo ActionInfo { get; internal set; }

        /// <summary>
        /// true:支持httpget方法
        /// </summary>
        public bool IsGet { get; set; }

        /// <summary>
        /// true:支持httppost方法
        /// </summary>
        public bool IsPost { get; set; }

        /// <summary>
        /// 验证特性
        /// </summary>
        public AUTHBaseAttribute? AuthAttr { get; set; }

        /// <summary>
        /// 路由特性
        /// </summary>
        public ROUTEAttribute? RouteAttr { get; set; }

        /// <summary>
        /// 检查方法参数,返回参数类型的默认值的对象数组
        /// </summary>
        /// <returns></returns>
        public object[] GetParasForInvoke()
        {
            object[] paras = null;
            var pArr = this.ActionInfo.GetParameters();
            if (pArr.Length > 0)
            {
                paras = new object[pArr.Length];
                for (int i = 0; i < pArr.Length; i++)
                {
                    var pinfo = pArr[i];
                    // 参数如有默认值,不再赋值
                    if (pinfo.HasDefaultValue)
                        continue;
                    paras[i] = pinfo.ParameterType.IsValueType ?
                    Activator.CreateInstance(pinfo.ParameterType) : null;
                }
            }
            return paras;
        }
    }
}