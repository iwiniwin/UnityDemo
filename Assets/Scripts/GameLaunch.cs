using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UDK.Output;

public class GameLaunch : MonoBehaviour
{
    // Start is called before the first frame update
    IEnumerator Start()
    {
        XLuaManager.Instance.InitLuaEnv();
        XLuaManager.Instance.LoadScript("Main");
        yield break;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
