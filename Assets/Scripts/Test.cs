using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Output;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int[] a = {1, 2, 3};
        Debug.Log("debug.bb" + " : " + a);
        Dump(a, "bb");
        // Debug.Log(a);
        // Debug.Log();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
