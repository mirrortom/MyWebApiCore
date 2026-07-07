using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;

namespace MyWebApi.test;

public class SecurityHelp
{
    #region 摘要

    /// <summary>
    /// MD5摘要,返回32位长度小写16进制字符串
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string Hex32_Md5(string str)
    {
        return PlainToDigest(str, 0);
    }

    /// <summary>
    /// Sha1摘要,返回40位长度小写16进制字符串
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string Hex40_SHA1(string str)
    {
        return PlainToDigest(str, 1);
    }

    /// <summary>
    /// Sha256摘要,返回64位长度小写16进制字符串
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string Hex64_SHA256(string str)
    {
        return PlainToDigest(str, 2);
    }
    /// <summary>
    /// Sha384摘要,返回96位长度小写16进制字符串
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string Hex96_SHA384(string str)
    {
        return PlainToDigest(str, 3);
    }
    /// <summary>
    /// Sha512摘要,返回128位长度小写16进制字符串
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string Hex128_SHA512(string str)
    {
        return PlainToDigest(str, 4);
    }

    /// <summary>
    /// 生产明文的摘要,空字符串或者null时不生成并返回null
    /// </summary>
    /// <returns></returns>
    private static string PlainToDigest(string plain, int type)
    {
        if (string.IsNullOrEmpty(plain))
            return null;
        byte[] buffer = Encoding.UTF8.GetBytes(plain);
        byte[] data;
        if (type == 0)
        {
            data = MD5.HashData(buffer);
        }
        else if (type == 1)
        {
            data = SHA1.HashData(buffer);
        }
        else if (type == 2)
        {
            data = SHA256.HashData(buffer);
        }
        else if (type == 3)
        {
            data = SHA384.HashData(buffer);
        }
        else if (type == 4)
        {
            data = SHA512.HashData(buffer);
        }
        else
            return null;
        // Console.WriteLine(data.Length);
        return Convert.ToHexString(data).ToLower();
    }
    #endregion
}
