using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyWebApi.test;

public class SerializeHelp
{
    /// <summary>
    /// 用指定字段(或者属性),建立新对象.返回对象的json字符串
    /// </summary>
    /// <typeparam name="T">源对象类型</typeparam>
    /// <param name="obj">源对象</param>
    /// <param name="fieldName">源对象字段(属性)</param>
    /// <returns></returns>
    public static string ObjectToJSON<T>(T obj, params string[] fieldNames)
    {
        // 没有指定字段,返回所有字段
        if (fieldNames.Length == 0)
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        //
        var dict = ObjectToDict(obj, fieldNames);
        return Newtonsoft.Json.JsonConvert.SerializeObject(dict);
    }

    /// <summary>
    /// 将对象列表转化为字典列表.可以指定部分字段(属性)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="objlist"></param>
    /// <param name="fieldNames"></param>
    /// <returns></returns>
    public static IEnumerable<Dictionary<string, object>> ObjectsToDicts<T>(IEnumerable<T> objlist, params string[] fieldNames)
    {
        List<Dictionary<string, object>> dictlist = new List<Dictionary<string, object>>();
        foreach (T item in objlist)
        {
            dictlist.Add(ObjectToDict(item, fieldNames));
        }
        return dictlist;
    }

    /// <summary>
    /// 将对象转化为字典.可以指定部分字段(属性)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="fieldNames"></param>
    /// <returns></returns>
    public static Dictionary<string, object> ObjectToDict<T>(T obj, params string[] fieldNames)
    {
        List<string> pnames = new List<string>();
        if (fieldNames != null && fieldNames.Length > 0)
            pnames.AddRange(fieldNames);
        else
        {
            System.Reflection.FieldInfo[] fields = obj.GetType().GetFields();
            System.Reflection.PropertyInfo[] props = obj.GetType().GetProperties();
            pnames.AddRange(fields.Select(o => o.Name));
            pnames.AddRange(props.Select(o => o.Name));
        }
        Dictionary<string, object> dict = new Dictionary<string, object>();
        for (int i = 0, len = pnames.Count; i < len; i++)
        {
            string pname = pnames[i];
            System.Reflection.FieldInfo field = obj.GetType().GetField(pname,
                                  System.Reflection.BindingFlags.Public |
                                  System.Reflection.BindingFlags.Instance |
                                  System.Reflection.BindingFlags.IgnoreCase);
            if (field != null)
            {
                dict.Add(field.Name, field.GetValue(obj));
                continue;
            }
            // 使用反射找到出该字段名称相同的对象的属性
            System.Reflection.PropertyInfo prop = obj.GetType().GetProperty(pname,
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.IgnoreCase);
            if (prop != null)
            {
                dict.Add(prop.Name, prop.GetValue(obj));
            }
        }
        return dict;
    }

    /// <summary>
    /// 转化为指定类型对象
    /// 如果JSON数值不能转为T的对应类型值,则会异常Mirror更新于2018年7月1日 18:42:01
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="json"></param>
    /// <returns></returns>
    public static T JsonToObject<T>(string json)
    {
        try
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        catch (Exception)
        {
            return default(T);
        }
    }
}