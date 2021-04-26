using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LuaComponentIndicator : MonoBehaviour
{
    public List<string> LuaComponents = new List<string>();

    public event Action<GameObject> LuaUpdate;
    public event Action<GameObject> LuaOnDestroy;

    void Update()
    {
        if(LuaUpdate != null)
        {
            LuaUpdate(this.gameObject);
        }
    }

    void OnDestroy() {
        if(LuaOnDestroy != null)
        {
            LuaOnDestroy(this.gameObject);
        }
        LuaUpdate = null;
        LuaOnDestroy = null;
    }

    public void AddLuaComponent(string name)
    {
        LuaComponents.Add(name);
    }
}
