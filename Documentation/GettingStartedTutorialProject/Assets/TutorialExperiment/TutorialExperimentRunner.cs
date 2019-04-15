using System;
using System.Collections;
using System.Collections.Generic;
using BML_ExperimentToolkit.Scripts.ExperimentParts;
using UnityEngine;

public class TutorialExperimentRunner : ExperimentRunner
{
    public override Type TrialType => typeof(TutorialTrial);

    public GameObject Stimulus;
    public GameObject Model;

    public Material Red;
    public Material Blue;
    public Material Green;

}
