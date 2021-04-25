using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
using UDK;

public class XLuaManager : UnitySingleton<XLuaManager>
{
    public const string luaScriptsFolder = "LuaScripts";
    
    LuaEnv luaEnv = null;

    public void InitLuaEnv(LuaEnv.CustomLoader loader = null)
    {
        luaEnv = new LuaEnv();
        luaEnv.AddLoader(loader ?? DefaultCustomLoader);
    }

    float tickInterval = 10;
    float lastTickTime = 0;
    int fullGcFrameCount = 100;
    void Update()
    {
        if(luaEnv != null) {
            if(Time.time - lastTickTime > tickInterval) {
                luaEnv.Tick();
                lastTickTime = Time.time;
            }
            if(Time.frameCount % fullGcFrameCount == 0) {
                luaEnv.FullGc();
            }
        }
    }

    public void DoString(string scriptContent) {
        if(luaEnv != null) {
            try{
                luaEnv.DoString(scriptContent);
            }catch(System.Exception ex) {
                string msg = string.Format("XLua DoString exception : {0}\n{1}", ex.Message, ex.StackTrace);
                Debug.LogError(msg);
            }
        }
    }

    public void LoadScript(string scriptName) {
        DoString(string.Format("require('{0}')", scriptName));
    }

    public void ReloadScript(string scriptName) {
        DoString(string.Format("package.loaded['{0}'] = nil", scriptName));
        LoadScript(scriptName);
    }

    public void DisposeLuaEnv() {
        if(luaEnv != null) {
            try{
                luaEnv.Dispose();
                luaEnv = null;
            }catch(System.Exception ex) {
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

    void OnDestroy() {
        DisposeLuaEnv();
    }
}
