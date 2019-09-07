using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Kit.Utils.Output;
using System;
using System.Threading;
using Kit.Utils;

enum Coll
{
    DSF, DSa, DStt
}

struct Tvvv{
    public string cc;
}

interface SLL{
    void aa();
}

public class Publisher{
    public delegate void NumManipulationHandler();
    public NumManipulationHandler ChangeNum;
    public NumManipulationHandler zz;

    public void OnNumChanged(){
        ChangeNum();
    }
}
public class Subscribe{
    public void printf(){
        Dump("jjj2");
        DumpTraceback();
    }
}

public class TestGeneric<S, T2>{
    public void Swap<T>(T a){

    }
}


public class Test : MonoBehaviour
{
    
    public static void CallToChild(){
        try
        {
            Dump("start ........");
            Thread.Sleep(5000);
            Dump("end.........");
        }
        catch (System.Exception e)
        {
            Dump(e);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        // ThreadStart child = new ThreadStart(CallToChild);
        // Thread childThread = new Thread(child);
        // Subscribe s = new Subscribe();
        // // s.printf();
        // #region
        // int a = 3;
        // int b = 3;
        // #endregion
        // ArrayList a;
        // childThread.Start();
        // Thread.Sleep(3000);
        // childThread.Abort("vvvvvvvvvv");
        
	}

    // Update is called once per frame
    void Update()
    {

    }

    void Tests()
    {
        int[] a = { 1, 2, 3 };

        // List<List<string>> b = new List<List<List>();
        // b.Add(a);
        // b.Add(a);
        // b.Add(a);
        int[,,] b = new int[2, 2, 4]
{
  {{1,2,3, 6},{4,5,6, 8}},
  {{7,8,9, 6},{2,3,4, 7}}
};


        int[] c = new int[4] { 1, 2, 3, 4 };


        int[,,,,] d = new int[1, 3, 2, 2, 1]{
  {
    {{{1}, {4}}, {{8}, {9}}},
    {{{2}, {4}}, {{1}, {2}}},
    {{{3}, {5}}, {{4}, {9}}},
  }
};
        // test(b);

        // Debug.Log("debug.bb" + " : " + a);
        // Dump(a, "bb");
        // Debug.Log(a.GetType().FilterName);
        // Dump("cccc", "fff");
        // Dump(a);
        // Debug.Log(a);
        // Debug.Log();
        // sbyte b = 3;
        ArrayList ab = new ArrayList() { 4, 7, "fff", "8" };

        Dictionary<string, ArrayList> dic = new Dictionary<string, ArrayList>();
        dic.Add("ss", ab);
        dic.Add("ss2", new ArrayList());
        dic.Add("ss3", new ArrayList() { 1 });

        Dictionary<string, string> dic2 = new Dictionary<string, string>();
        dic2.Add("ss", "ab");
        dic2.Add("ss2", "new ArrayList()");
        dic2.Add("ss3", "new ArrayList(){1}");

        Dictionary<string, int[]> dic3 = new Dictionary<string, int[]>();
        dic3.Add("ss", new int[3] { 1, 2, 3 });
        dic3.Add("ss2", new int[2] { 4, 4 });
        dic3.Add("ss3", new int[1] { 5 });

        Dictionary<string, int[,,,,]> dic4 = new Dictionary<string, int[,,,,]>();
        dic4.Add("ss", d);
        dic4.Add("ssa", d);

        // Dump(dic.GetType().Name, "jjj");
        // Debug.Log(dic.GetType().Name);

        Dump(DateTime.MinValue);
        Dump(Coll.DSF);
        Dump(new ArrayList() { new Version(), new Version() });
        Dump(dic4);
        Dump(dic2);
        Dump(dic);
        Dump(c);
        Dump(b, this.gameObject);
        Dump(this.gameObject, "rrrrrrrrr");
        Debug.Log("sllss", this.gameObject);
        Dump(this.gameObject.ToString(), "mmm");
        Dump("rrr", "ggg");
        Dump(this.gameObject, this.gameObject);
        Dump("zzz", this.gameObject);

        Debug.Log("this.gameObject", this.gameObject);
        Debug.Log("this.gameObject");
        Debug.Log("this.gameObject");
        Dump("this.game");
        // Debug.Log(this.gameObject, this.gameObject);
        // Debug.Log(b.GetLength(0));
        // Debug.Log(b.get);
        // int [][] b = new int[2][]{
        //   new int[]{},
        //   new int[]{},
        // };
        // Debug.Log(b.Rank);
        // Debug.Log(b.GetLength(0));
        // Debug.Log(b.GetLength(1));
        // Debug.Log("slflsf\nsllsf\nslfls\nsfslf\n");
    }
}
