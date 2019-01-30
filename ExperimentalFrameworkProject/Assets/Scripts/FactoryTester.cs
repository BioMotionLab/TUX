using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryTester : MonoBehaviour {

    public Config theConfig;

    // Start is called before the first frame update
    void Start() {
        Debug.Log("Printing table");
        theConfig.Factory.ToTable(theConfig.ShuffleTrialOrder, theConfig.NumberOfTimesToRepeatTrials).PrintToConsole();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
