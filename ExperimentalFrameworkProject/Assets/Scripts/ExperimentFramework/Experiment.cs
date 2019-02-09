using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Experiment : MonoBehaviour, Outputtable{

    public ExperimentDesign Design;
    Session session;
    OutputManager outputManager;
    public Config Config;
    public bool Running = false;
    public bool Ended = false;
    
    void Start() {
        Design = Config.ExperimentDesign;
        ExperimentEvents.InitExperiment(this);
    }

    void OnEnable() {
        ExperimentEvents.OnStartExperiment += StartExperiment;
        ExperimentEvents.OnTrialUpdated += TrialUpdated;
        ExperimentEvents.OnEndExperiment += EndExperiment;
    }

    void OnDisable() {
        ExperimentEvents.OnStartExperiment -= StartExperiment;
        ExperimentEvents.OnTrialUpdated -= TrialUpdated;
        ExperimentEvents.OnEndExperiment -= EndExperiment;


    }

    void TrialUpdated(List<Trial> trials, int index) {
        OutputUpdated();
    }

    void OutputUpdated() {
        ExperimentEvents.OutputUpdated(this);
    }

    void StartExperiment(Session currentSession) {
        this.session = currentSession;
        Running = true;
        outputManager = new OutputManager(currentSession.OutputPath, session.DebugMode);
        ExperimentEvents.ExperimentStarted();
        BlockSequenceRunner blockRunner = new BlockSequenceRunner(this, Design.Blocks);
        blockRunner.Start();
    }

    void EndExperiment() {
        Running = false;
        Ended = true;
        outputManager.Disable();
    }

    public string AsString {
        get {
            StringBuilder sb = new StringBuilder();

            string header = Design.TrialTableHeader;
            sb.AppendLine(header);
            foreach (Block block in Design.Blocks) {
                foreach (Trial trial in block.Trials) {
                    sb.AppendLine(trial.Data.AsString(separator: Delimiter.Comma, truncate: -1));
                }
            }

            return sb.ToString();
        }
    }
}