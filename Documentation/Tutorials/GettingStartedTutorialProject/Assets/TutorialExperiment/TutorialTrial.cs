using System;
using System.Collections;
using System.Data;
using BML_ExperimentToolkit.Scripts.ExperimentParts;
using UnityEngine;

public class TutorialTrial : Trial
{
    
    TutorialExperimentRunner tutorialRunner;
    
    //constructor. you don't need to worry about this.
    public TutorialTrial(ExperimentRunner runner, DataRow data) : base(runner, data) {
    }
    
    protected override void PreMethod() {
        
        // convert the generic ExperimentRunner to your custom type of ExperimentRunner.
        tutorialRunner = (TutorialExperimentRunner)Runner;

        //Get this trial's value for the Color variable
        string colorString = (string) Data["Color"];

        //Set the material of the stimulus to the color
        MeshRenderer stimulusMesh = tutorialRunner.Stimulus.GetComponent<MeshRenderer>();
        switch (colorString) {
            case "Red":
                stimulusMesh.material = tutorialRunner.Red;
                break;
            case "Blue":
                stimulusMesh.material = tutorialRunner.Blue;
                break;
            case "Green":
                stimulusMesh.material = tutorialRunner.Green;
                break;
            default:
                throw new ArgumentOutOfRangeException($"No material defined for {colorString}");
        }

        //Get this trial's value for the Size variable
        float size = (float) Data["Size"];

        //Set the size of Model 
        Transform modelTransform = tutorialRunner.Model.transform;
        modelTransform.localScale = new Vector3(size, size, size);


    }

    protected override IEnumerator RunMainCoroutine() {
    
        bool trialComplete = false;
        while (!trialComplete) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                trialComplete = true;
            }

            //Define moveSpeed (this should probably be at top of file)
            float moveSpeed = 1;

            //Store the stimulus' transform so we reduce typing (could also be at top)
            Transform stimulusTransform = tutorialRunner.Stimulus.transform;

            //calculate how much to change based on time since last frame and moveSpeed
            float sizeChange = Time.deltaTime * moveSpeed;
            Vector3 changeVector = new Vector3(sizeChange, sizeChange, sizeChange);

            //find the new value for the localScale of the stimulus.
            //Add if W key, subtract if S key
            Vector3 newScale = stimulusTransform.localScale;
            if (Input.GetKey(KeyCode.W)) {
                newScale += changeVector;
            }
            else if (Input.GetKey(KeyCode.S)) {
                newScale -= changeVector;
            }

            //Update the localScale to the new value
            stimulusTransform.localScale = newScale;

            yield return null;
        }
    }

    protected override void PostMethod() {
        float selectedSize = tutorialRunner.Stimulus.transform.localScale.x;
        Data["SelectedSize"] = selectedSize;
    }
}
