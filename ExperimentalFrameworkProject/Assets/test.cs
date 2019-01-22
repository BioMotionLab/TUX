using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour {

	public List<foo> vars = new List<foo>();
}

[Serializable]
public class foo {
    public int bar;

    public Poop pooIntObject = new PooInt(5);

}

[Serializable]

public class Poop { }

public class Poo<T> : Poop {
    public T genericValue;

    public Poo(T val) {
        genericValue = val;
    }
}

[Serializable]
public class PooInt : Poo<int> {
    public PooInt(int val) : base(val) { }
}