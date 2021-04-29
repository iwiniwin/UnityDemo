using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using XLua;

public class XLuaComponentBinder : MonoBehaviour
{
    public string componentFactoryPath = "ComponentFactory";
    public List<string> luaComponents = new List<string>();
    private List<string> realLuaComponents = new List<string>();
    private event Action LuaStart;
    private event Action LuaUpdate;
    private event Action LuaOnDestroy;

    private bool startCalled = false;

    void Awake()
    {
        foreach(string scriptPath in luaComponents)
        {
            AddComponent(scriptPath);
        }
    }

    void Start()
    {
        if(LuaStart != null)
        {
            LuaStart();
        }
        startCalled = true;
    }

    void Update()
    {
        if(LuaUpdate != null)
        {
            LuaUpdate();
        }
    }

    public object[] AddComponent(string scriptPath) 
    {
        realLuaComponents.Add(scriptPath);
        object[] ret = XLuaManager.Instance.DoString(string.Format("return require('{0}')('{1}')", componentFactoryPath, scriptPath));
        if (ret != null && ret.Length >= 1)
        {
            LuaTable luaComponent = ret[0] as LuaTable;
            BindComponent(luaComponent);
        }
        return ret;
    }

    private void BindComponent(LuaTable luaComponent)
    {
        if(luaComponent == null) return;
        luaComponent.Set("gameObject", this.gameObject);
        Action<LuaTable> scriptAwake;
        Action<LuaTable> scriptStart;
        Action<LuaTable> scriptUpdate;
        Action<LuaTable> scriptOnDestroy;
        luaComponent.Get("Awake", out scriptAwake);
        luaComponent.Get("Start", out scriptStart);
        luaComponent.Get("Update", out scriptUpdate);
        luaComponent.Get("OnDestroy", out scriptOnDestroy);
        if(scriptAwake != null)
        {
            scriptAwake(luaComponent);
        }
        if(scriptStart != null)
        {
            if(startCalled)
            {
                scriptStart(luaComponent);
            }
            else
            {
                LuaStart += () => {
                    scriptStart(luaComponent);
                };
            }
        }
        if(scriptUpdate != null)
        {
            LuaUpdate += () => {
                scriptUpdate(luaComponent);
            };
        }
        if(scriptOnDestroy != null)
        {
            LuaOnDestroy += () => {
                scriptOnDestroy(luaComponent);
            };
        }
    }

    void OnDestroy() {
        DestroyLuaComponents();
    }

    public void DestroyLuaComponents() 
    {
        luaComponents.Clear();
        realLuaComponents.Clear();
        if(LuaOnDestroy != null)
        {
            LuaOnDestroy();
        }
        LuaStart = null;
        LuaUpdate = null;
        LuaOnDestroy = null;
    }
}
