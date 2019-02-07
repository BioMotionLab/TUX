using System.Collections.Generic;
using UnityEngine;

public class Experiment : MonoBehaviour{

    public ExperimentDesign Design;
    public Config Config;
    public bool Running = false;
    public bool Ended = false;

    void Start() {
        Design = Config.ExperimentDesign;
        ExperimentEvents.InitExperiment(this);
    }

    void OnEnable() {
        ExperimentEvents.OnStartExperiment += StartExperiment;
        ExperimentEvents.OnEndExperiment += EndExperiment;
    }

    void OnDisable() {
        ExperimentEvents.OnStartExperiment -= StartExperiment;
        ExperimentEvents.OnEndExperiment -= EndExperiment;
    }

    

    void StartExperiment() {
        Running = true;
        BlockSequenceRunner blockRunner = new BlockSequenceRunner(this, Design.Blocks);
        blockRunner.Start();
    }

    void EndExperiment() {
        Running = false;
        Ended = true;
    }
}