using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Output
{
    public static void Dump(object obj, string msg){
        
        if (obj is Array){
            string output = "{";
            Array arr = (Array)obj;
            foreach (object a in arr)
            {
                output += (a + ",");
            }
            output += "}";
            Debug.Log(msg + " : " + output);
        }else{
            Debug.Log(msg + " : " + obj);
        }
    }
}



