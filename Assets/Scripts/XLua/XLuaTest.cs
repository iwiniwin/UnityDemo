using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[XLua.Hotfix]
public class XLuaTest : MonoBehaviour
{

    void Awake()
    {
        Debug.Log(Time.frameCount + "   Awake");
    }

    private int tick = 0;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        Debug.Log(Time.frameCount + "   Start");
        yield break;
    }

    // Update is called once per frame
    void Update()
    {
        if (++tick % 50 == 0)
        {
            Debug.Log(">>>>>>>>Update in C#, tick = " + tick);
        }
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 300, 80), "Hotfix"))
        {
            XLuaManager.Instance.DoString(@"
            xlua.hotfix(CS.XLuaTest, 'Update', function(self)
                self.tick = self.tick + 1
                if (self.tick % 50) == 0 then
                    print('<<<<<<<<Update in lua, tick = ' .. self.tick)
                end
            end)
            ");

        }
        string hint = "heint.......................................";
        GUIStyle style = GUI.skin.textArea;
        style.normal.textColor = Color.red;
        style.fontSize = 16;
        GUI.TextArea(new Rect(10, 100, 500, 290), hint, style);
    }
}
