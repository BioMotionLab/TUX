using System.Collections;
using System.Collections.Generic;
using BML_ExperimentToolkit.Scripts.VariableSystem;
using UnityEngine;
using UnityEditor;

public class DatumTester : MonoBehaviour {


 // public IndependentVariable<int> intDatumGeneric = new IndependentVariable<int>();

    //[SerializeField]
    //public IndependentVariableInt intDatum = new IndependentVariableInt();

    
    //public IndependentVariableInt unserializedIntDatum = new IndependentVariableInt();

    //[SerializeField]
    //public DatumGameObject gameObjectDatum= new DatumGameObject();

    //[SerializeField]
    //public DatumVector3 vector3Datum = new DatumVector3();

    [SerializeField] public VariableFactory Factory = new VariableFactory();



    // Use this for initialization
    void Start () {

        



        //IDatum dat = new IndependentVariable<int>(intvariablename, () => somevariable);

        
        //data.Add(dat);

        

	    
        //data.Add(IndependentVariable.Add(intvariablename, () => somevariable));

	    

    }
	
	// Update is called once per frame
	void Update () {


    }
}
