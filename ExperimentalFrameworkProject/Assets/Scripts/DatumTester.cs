using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DatumTester : MonoBehaviour {


 // public Datum<int> intDatumGeneric = new Datum<int>();

    //[SerializeField]
    //public DatumInt intDatum = new DatumInt();

    
    //public DatumInt unserializedIntDatum = new DatumInt();

    //[SerializeField]
    //public DatumGameObject gameObjectDatum= new DatumGameObject();

    //[SerializeField]
    //public DatumVector3 vector3Datum = new DatumVector3();

    [SerializeField] public DatumFactory Factory = new DatumFactory();



    // Use this for initialization
    void Start () {

        



        //IDatum dat = new Datum<int>(intvariablename, () => somevariable);

        
        //data.Add(dat);

        

	    
        //data.Add(Datum.New(intvariablename, () => somevariable));

	    

    }
	
	// Update is called once per frame
	void Update () {


    }
}
