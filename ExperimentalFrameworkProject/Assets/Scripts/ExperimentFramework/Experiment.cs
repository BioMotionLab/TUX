using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public abstract class Experiment : MonoBehaviour, Outputtable {

    public abstract Type TrialType { get; }
    public abstract Type BlockType { get; }


    public ExperimentDesign Design;
    Session session;
    OutputManager outputManager;
    public Config Config;
    public bool Running = false;
    public bool Ended = false;
    
    void Start() {
        
        Design = Config.Factory.ToTable(this, Config.ShuffleTrialOrder, Config.NumberOfTimesToRepeatTrials);
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
        outputManager = new OutputManager(currentSession.OutputFullPath);
        StartCoroutine(RunPreExperiment());
    }

    IEnumerator RunPreExperiment() {
        yield return Pre();
        ExperimentEvents.ExperimentStarted();
        BlockSequenceRunner blockRunner = new BlockSequenceRunner(this, Design.Blocks);
        blockRunner.Start();
    }

    IEnumerator RunPostExperiment() {
        yield return Post();


        Running = false;
        Ended = true;
        Design.Disable();
        outputManager.Disable();
    }

    protected virtual IEnumerator Pre() {
        Debug.Log("Skipping pre experiment code");
        yield return null;
    }

    protected virtual IEnumerator Post() {
        Debug.Log("Skipping post experiment code");
        yield return null;
    }

    void EndExperiment() {
        StartCoroutine(RunPostExperiment());

        
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
