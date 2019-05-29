using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using BML_ExperimentToolkit.Scripts.ExperimentParts;
using UnityEngine;

public class TutorialBlock : Block
{
    public TutorialBlock(ExperimentRunner runner, DataTable trialTable, string identity, Type trialType, DataRow dataRow) 
        : base(runner, trialTable, identity, trialType, dataRow) {
    }

    protected override void PreMethod() {
        //Get reference to custom ExperimentRunner
        TutorialExperimentRunner tutorialRunner = (TutorialExperimentRunner) runner;

        //Get value of distance for this block.
        float distance = (float) Data["Distance"];

        //get position
        Vector3 position = tutorialRunner.Stimulus.transform.localPosition;

        //set z of position to the distance value.
        position.z = distance;

        //update stimulus position to new position
        tutorialRunner.Stimulus.transform.localPosition = position;

    }
}
