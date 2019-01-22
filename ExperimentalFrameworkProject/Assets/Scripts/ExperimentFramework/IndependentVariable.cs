using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using UnityEngine.Experimental.UIElements;


[Serializable]
public abstract class IndependentVariable :Variable {

    public string Name;


    public bool CustomProportions;
    public RepeatType Repeat;
    public BlockingBehaviour Blocking;
    public int NumberOfRepeats;
    public bool RandomizeOrder;

    public enum RepeatType {
        None,
        RepeatWithin,
        RepeatAsBlock,
    }

    protected enum Type {
        Int,
        Float,
        String,
        GameObject
    }

    public enum BlockingBehaviour {
        NoBlock,
        SuperBlock,
        MidBlock,
        LowBlock,
    }

    
}

[Serializable]
public abstract class Variable {

}


[Serializable]
public class IndependentVariableInt : IndependentVariable<int> { }

[Serializable]
public class IndependentVariable<T> : IndependentVariable {
    public List<T> Values = new List<T>();
}

[Serializable]
public class IndependentVariableFloat : IndependentVariable<float> { }

[Serializable]
public class IndependentVariableString : IndependentVariable<string> { }

[Serializable]
public class IndependentVariableGameObject : IndependentVariable<GameObject> { }

[Serializable]
public class IndependentVariableFactory {

    [SerializeField]
    public List<IndependentVariable> IVs = new List<IndependentVariable>();


    public Type SelectType;

    public enum Type {
        Int,
        Float,
        String,
        GameObject
    }

    public IndependentVariableInt Int() {
        return new IndependentVariableInt();
    }
    public IndependentVariableFloat Float() {
        return new IndependentVariableFloat();
    }
    public IndependentVariableString String() {
        return new IndependentVariableString();
    }
    public IndependentVariableGameObject GameObject() {
        return new IndependentVariableGameObject();
    }

    public void CreateNew() {

        IndependentVariable newVar;
        switch (SelectType) {
            case Type.Int:
                newVar = Int();
                break;
            case Type.Float:
                newVar = Float();
                break;
            case Type.String:
                newVar = String();
                break;
            case Type.GameObject:
                newVar = GameObject();
                break;
            default:
                throw new ArgumentOutOfRangeException();

        }

        IVs.Add(newVar);
        Debug.Log(IVs.Count);
    }
}