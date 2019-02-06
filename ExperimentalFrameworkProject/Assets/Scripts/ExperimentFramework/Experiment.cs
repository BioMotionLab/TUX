using System.Collections.Generic;
using UnityEngine;

public class Experiment : MonoBehaviour{

    public ExperimentTable table;
    public Config Config;

    void Start() {
        table = Config.ExperimentTable;

        ExperimentEvents.InitExperiment(this);
    }

    void OnEnable() {
        ExperimentEvents.OnStartExperiment += StartExperiment;
    }

    void OnDisable() {
        ExperimentEvents.OnStartExperiment -= StartExperiment;
    }

    void StartExperiment(ExperimentTable table) {
        BlockSequenceRunner blockRunner = new BlockSequenceRunner(this, table.blocks);
        blockRunner.Start();
    }
}