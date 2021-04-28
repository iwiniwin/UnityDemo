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
        ClearLuaComponent();
    }

    public void AddLuaComponent(string name)
    {
        LuaComponents.Add(name);
    }

    public void ClearLuaComponent() 
    {
        LuaComponents.Clear();
        if(LuaOnDestroy != null)
        {
            LuaOnDestroy(this.gameObject);
        }
        LuaUpdate = null;
        LuaOnDestroy = null;
    }
}
