using System.Collections;
using System.Data;
using BML_ExperimentToolkit.Scripts.ExperimentParts;
using UnityEngine;

namespace BML_TUX.Data.ScriptingTemplates {
    /// <summary>
    /// Classes that inherit from Trial define custom behaviour for your experiment.
    ///
    /// This template shows how to set up a custom trial script using the toolkit's built-in functions.
    ///
    /// You can delete any unused methods and unwanted comments. The only required parts are the constructor and the MainCoroutine.
    /// </summary>
    public class ___TrialClassName___ : Trial {
    
    
        // // You usually want to store a reference to your experiment runner
        // YourCustomExperimentRunner myRunner;
    
    
        // Required Constructor. Good place to set up references to objects in the unity scene
        public ___TrialClassName___(ExperimentRunner runner, DataRow data) : base(runner, data) {
            // myRunner = (YourCustomExperimentRunner)runner;  //cast the generic runner to your custom type.
            // GameObject myGameObject = myRunner.MyGameObject;  // get reference to gameObject stored in your custom runner
        
        }


        // Optional Pre-Trial code. Useful for setting up trials (one frame only)
        protected override void PreMethod() {
        
            // float thisTrialsDistanceValue = (float)Data["MyDistanceFloatVariableName"]; // Read values of independent variables
            // myGameObject.transform.position = new Vector3(thisTrialsDistanceValue, 0, 0); // set up scene based on value
        }

    
        // Optional Pre-Trial code. Useful for waiting for the participant to do something before each trial (multiple frames)
        protected override IEnumerator PreCoroutine() {
            yield return null; //required for coroutine
        }

    
        // Main Trial Execution Code.
        protected override IEnumerator RunMainCoroutine() {
        
            // You might want to do a while-loop to wait for participant response: 
            bool waitingForParticipantResponse = true;
            while (waitingForParticipantResponse) {   // keep check each frame until waitingForParticipantResponse set to false.

                if (Input.GetKeyDown(KeyCode.Return)) { // check return key pressed
                    waitingForParticipantResponse = false;  // escape from while loop
                }
            
                yield return null; // wait for next frame while allowing rest of program to run (without this the program will hang in an infinite loop)
            }
        
        }

    
        // Optional Post-Trial code. Useful for waiting for the participant to do something after each trial (multiple frames)
        protected override IEnumerator PostCoroutine() {
            yield return null;
        }

    
        // Optional Post-Trial code. Useful for resetting everything, and writing data to dependent variables. (One frame only)
        protected override void PostMethod() {
            // Good place to write results to dependent variables. 
            // Data["MyDependentFloatVariable"] = someFloatVariable;
        }
    }
}
