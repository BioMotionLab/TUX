using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


[Serializable]
public class Variable<T> : List<T> {

    [SerializeField]
    public List<T> AsList = new List<T>();


    public void HelloWorld() {
        Debug.Log("hello world");
    }


}






