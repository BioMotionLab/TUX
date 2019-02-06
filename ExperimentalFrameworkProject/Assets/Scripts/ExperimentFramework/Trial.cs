using System;
using System.Collections;
using System.Data;
using UnityEngine;


/// <summary>
/// One Trial of an experiment. The experiment calls Run on this trial,
/// and it is in charge of setting and cleaning itself up
/// </summary>
public abstract class Trial {
    readonly DataRow data;
    readonly Config configFile;

    public DataRow Data => data;

    public int Index => (int)data[Config.TrialIndexColumnName];

    public bool Success {
        get {
            return (bool)data[Config.SuccessColumnName];
        }
        set {
            data[Config.SuccessColumnName] = value;
        }
    }

    public int Attempts {
        get { return (int) data[Config.AttemptsColumnName]; }
        set { data[Config.AttemptsColumnName] = value; }
    }

    public bool Skipped {
        get {
            return (bool)data[Config.SkippedColumnName];
        }
        set {
            data[Config.SkippedColumnName] = value;
        }
    }

    protected Trial(DataRow data, Config configFile) {
        this.data = data;
        this.configFile = configFile;
    }


    bool interrupt = false;

    public IEnumerator Run() {
        
        Success = false;
        interrupt = false;

        //Skip a frame to allow any previous things to end
        yield return null;

        ExperimentEvents.TrialHasStarted(this);

        Debug.Log($"Trail {Index} Waiting for return key");
        bool waiting = true;
        while (waiting) {

            if (interrupt) break;

            if (CheckExperimenterControls()) break;

            yield return null;
            if (Input.GetKeyDown(KeyCode.Return)) {
                waiting = false;
                Success = true;
                ReturnPressed();
            }

            
            
        }

        if (Success) {
            Debug.Log($"Trial {Index} completed successfully");
            ExperimentEvents.TrialHasCompleted();
        }
        
        
        yield return null;

    }

    bool CheckExperimenterControls() {
        if (Input.GetKeyDown(KeyCode.Backspace)) {
            ExperimentEvents.InterruptTrial();
            return true;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            ExperimentEvents.GoBackOneTrial();
            return true;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            ExperimentEvents.SkipToNextTrial();
            return true;
        }

        return false;
    }

    void ReturnPressed() {
        Debug.Log($"Trail {Index} Return key pressed");
    }

    public void Interrupt() {
        interrupt = true;
    }

}