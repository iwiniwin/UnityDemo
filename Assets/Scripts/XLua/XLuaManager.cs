using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using XLua;
using UDK;

public class XLuaManager : UnitySingleton<XLuaManager>
{
    public const string luaScriptsFolder = "LuaScripts";

    LuaEnv luaEnv = null;

    void Awake()
    {
        InitLuaEnv();
    }

    public void InitLuaEnv(LuaEnv.CustomLoader loader = null)
    {
        if(luaEnv != null) return;
        luaEnv = new LuaEnv();
        luaEnv.AddLoader(loader ?? DefaultCustomLoader);
    }

    float tickInterval = 10;
    float lastTickTime = 0;
    int fullGcFrameCount = 100;
    void Update()
    {
        if (luaEnv != null)
        {
            if (Time.time - lastTickTime > tickInterval)
            {
                luaEnv.Tick();
                lastTickTime = Time.time;
            }
            if (Time.frameCount % fullGcFrameCount == 0)
            {
                luaEnv.FullGc();
            }
        }
    }

    public object[] DoString(string scriptContent, string chunkName = "chunk", LuaTable env = null)
    {
        if(luaEnv == null)
        {
            InitLuaEnv();
        }
        try
        {
            return luaEnv.DoString(scriptContent, chunkName, env);
        }
        catch (System.Exception ex)
        {
            string msg = string.Format("XLua DoString exception : {0}\n{1}", ex.Message, ex.StackTrace);
            Debug.LogError(msg);
        }
        return null;
    }

    public object[] LoadScript(string scriptName, string chunkName = "chunk", LuaTable env = null)
    {
        return DoString(string.Format("return require('{0}')", scriptName), chunkName, env);
    }

    public object[] ReloadScript(string scriptName, string chunkName = "chunk", LuaTable env = null)
    {
        DoString(string.Format("package.loaded['{0}'] = nil", scriptName));
        return LoadScript(scriptName, chunkName, env);
    }

    public void DisposeLuaEnv()
    {
        if (luaEnv != null)
        {
            try
            {
                luaEnv.Dispose();
                luaEnv = null;
            }
            catch (System.Exception ex)
            {
                string msg = string.Format("XLua Dispose exception : {0}\n{1}", ex.Message, ex.StackTrace);
                Debug.LogError(msg);
            }
        }
    }

    public static byte[] DefaultCustomLoader(ref string luaPath)
    {
        string scriptPath = string.Empty;
        string filepath = string.Empty;
#if UNITY_EDITOR
        filepath = luaPath.Replace(".", "/") + ".lua";
        scriptPath = System.IO.Path.Combine(Application.dataPath, luaScriptsFolder);
        scriptPath = System.IO.Path.Combine(scriptPath, filepath);
        return FileUtil.ReadAllBytes(scriptPath);
#endif
        //AB加载或者其他加载
        // return LuaReaderHelper.Instance.LoadFromPackage(luaPath);  // todo
    }

    private void ClearComponentBinders()
    {
        var objects = FindObjectsOfType<XLuaComponentBinder>();
        foreach(var obj in objects)
        {
            obj.DestroyLuaComponents();  // Destroy(obj)实际的对象销毁操作始终延迟到当前更新循环结束，因此主动通过DestroyLuaComponents清空委托
            Destroy(obj);  
        }
    }

    void OnDestroy()
    {
        ClearComponentBinders();
        DisposeLuaEnv();
    }
}
