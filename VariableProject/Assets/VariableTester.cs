using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class VariableTester : MonoBehaviour {

    //[SerializeField]
    public Variable<int> distances = new Variable<int> { 5, 10, 15};

    [SerializeField]                                         
    //public Variable<float> times = new Variable<float> {0.5f, 0.1f, 0.15f};

    void Start() {

        Debug.Log(distances[0]);

        foreach (var value in distances) {
            Debug.Log(value);
        }

        foreach (var value in distances) {
            Debug.Log(value);
        }

        Debug.Log(String.Join(", ", distances));

        distances.HelloWorld();
    }

    
}
