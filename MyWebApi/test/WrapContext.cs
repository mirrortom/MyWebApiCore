﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using MyWebApi.core;

namespace MyWebApi.test;

/// <summary>
/// 包装各种上下文对象,用于传递
/// </summary>
public class WrapContext
{
    public static IMemoryCache MemoryCache { get; set; }

    //protected readonly DBMO db = DbContext.GetDB();
    protected readonly IMemoryCache cache = WrapContext.MemoryCache;
    protected UserAuth user;
    protected ReturnCode result;


    public void SetContext(UserAuth userauth, ReturnCode resultcode)
    {
        this.user = userauth;
        this.result = resultcode;
    }
}
