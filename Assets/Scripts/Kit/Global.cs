using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
有些功能比较单一且需要用到脚本生命周期方法的类，比较适合使用这种单例脚本
 */
public class Global : MonoBehaviour
{
    public static Global instance;

    // 静态构造函数只会被执行一次
    // 在创建第一个实例或引用任何静态成员之前，由.NET调用
    static Global(){
        GameObject go = new GameObject("#Global#");
        DontDestroyOnLoad(go);  // 保证自己不被主动卸载掉
        instance = go.AddComponent<Global>();
    }

    public void DoSomeThings(){

    }

    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log("Start Global");
    }
}
