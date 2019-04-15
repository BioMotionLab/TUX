using System.Collections;
using System.Collections.Generic;
using System.Data;
using BML_ExperimentToolkit.Scripts.ExperimentParts;
using UnityEngine;

public class TutorialTrial : Trial
{
    public TutorialTrial(ExperimentRunner runner, DataRow data) : base(runner, data) {
    }

    TutorialExperimentRunner tutorialRunner;

    public override void PreMethod() {
        tutorialRunner = (TutorialExperimentRunner)runner;

        //Get this trial's value for the Color variable
        string colorString = (string) Data["Color"];

        //Set the material of the stimulus to the color
        if (colorString == "Red") {
            tutorialRunner.Stimulus.GetComponent<MeshRenderer>().material = tutorialRunner.Red;
        }

    }

    protected override IEnumerator RunMainCoroutine() {
        Debug.Log("Trial Running");
        yield return null;
    }
}
