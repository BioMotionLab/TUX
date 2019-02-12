using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyFancyExperiment : Experiment {
    protected override IEnumerator Post() {
        //Your code here
        return null;
    }

    protected override IEnumerator Pre() {

        DisplayStartingInstructions(); //called right away

        //the rest of your program will run normally while this waits.
        yield return new WaitForSeconds(5);
        
        StopDisplayingInstructions(); //will only get called after 5 seconds
    }

    void StopDisplayingInstructions() {
        //your code for displaying
    }

    void DisplayStartingInstructions() {
        //your code for stopping to display
    }

}
