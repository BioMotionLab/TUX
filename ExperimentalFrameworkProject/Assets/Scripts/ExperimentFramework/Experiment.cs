using System.Collections.Generic;
using UnityEngine;

public class Experiment : MonoBehaviour{

    public ExperimentTable table;
    public Config Config;
    public bool Running = false;
    public bool Ended = false;

    void Start() {
        table = Config.ExperimentTable;

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
        BlockSequenceRunner blockRunner = new BlockSequenceRunner(this, table.blocks);
        blockRunner.Start();
    }

    void EndExperiment() {
        Running = false;
        Ended = true;
    }
}