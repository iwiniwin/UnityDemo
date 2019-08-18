using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Kit.Utils.Output;
using Example.Coroutine;

public class TestCoroutine : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Dump("jjjj");
        // MyCoroutine.Instance.StartCoroutine(new MyWaitForSeconds(3.0f));
        StartCoroutine(Test());
        MyCoroutine.Instance.MyStartCoroutine(MyTest());
        // Dump("hhhh");
    }

    // IEnumerator Start(){
    //     new WaitForSeconds(3);
    //     Dump("start..........");
    //     yield return StartCoroutine(MyWaitFunction(3.0f));
    //     Dump("end ..........");
    //     WWW www = new WWW("");
    //     yield return www;
    // }

    // Update is called once per frame
    void Update()
    {
        // Dump("update...........");
    }

    IEnumerator Test(){
        Dump(Time.time, "hhhhhhhhhhh");
        yield return new WaitForSeconds(3);
        Dump(Time.time, "zzz");
        yield return null;
        Dump(Time.time, "vvvvv");
        yield return null;
        Dump(Time.time, "jjj");
        yield return new WaitForSeconds(2);
        // yield return StartCoroutine(Test2());
        Dump(Time.time, "bbb");
    }

    IEnumerator MyTest(){
        Dump(Time.time, "my hhhhhhhhhhh");
        yield return new MyWaitForSeconds(3);
        Dump(Time.time, "my zzz");
        yield return null;
        Dump(Time.time, "my vvvvv");
        yield return null;
        Dump(Time.time, "my jjj");
        yield return new MyWaitForSeconds(2);
        // yield return StartCoroutine(Test2());
        // yield return MyCoroutine.Instance.MyStartCoroutine(Test2());
        Dump(Time.time, "my bbb");
    }

    IEnumerator Test2(){
        yield return new WaitForSeconds(3);
        Dump("ssss", "fff");
    }
}
