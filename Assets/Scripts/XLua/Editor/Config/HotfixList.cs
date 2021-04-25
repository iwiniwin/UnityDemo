using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
using System.Reflection;
using System.Linq;

public static class HotfixList
{
    [Hotfix]
    public static List<Type> by_property
    {
        get
        {
            return (from type in Assembly.Load("Assembly-CSharp").GetTypes()
                where type.Namespace == "XXXX"
                select type).ToList();
        }
    }
}
