using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public static class LuaCallCSharpList
{
    [LuaCallCSharp]
    public static List<Type> unity_lua_call_cs_list = new List<Type>()
    {
        typeof(GameObject),
    };

    [LuaCallCSharp]
    public static List<Type> custom_lua_call_cs_list = new List<Type>()
    {

    };
}
