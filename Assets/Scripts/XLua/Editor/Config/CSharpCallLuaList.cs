using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public static class CSharpCallLuaList
{
    [CSharpCallLua]
    public static List<Type> cs_call_lua_list = new List<Type>()
    {
        typeof( System.Collections.IEnumerator)
    };
}
