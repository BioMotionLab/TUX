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

    public DataRow Data => data;
    MonoBehaviour runner;
    public int Index => (int)data[Config.TrialIndexColumnName];
    public int BlockIndex => (int)(data[Config.BlockIndexColumnName]);
    public string TrialText => $"Trial {Index} of Block {BlockIndex}";

    public bool CompletedSuccessfully {
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

    protected Trial(DataRow data) {
        this.data = data;
   
    }

    protected bool trialRunning = true;
    bool interrupt = false;

    public IEnumerator Run (MonoBehaviour theRunner) {
        this.runner = theRunner;
        InitializeTrial();
        //Skip a frame to allow any previous things to end
        yield return null;

        runner.StartCoroutine(RunExperimentControls());


        Debug.Log($"{TrialText} Running...");

        if (!interrupt) {
            Debug.Log($"{TrialText} Running pretrial");
            yield return RunPreTrial();
        }
        else {
            Debug.Log($"{TrialText} pretrial interrupted");
        }

        Debug.Log($"{TrialText} Running main trial");
        IEnumerator main = RunMainTrial();
        while (!interrupt && main.MoveNext()) {
            yield return main.Current;
            
        }
        Debug.Log("Done running main");
    
        if (!interrupt) {
            Debug.Log($"{TrialText} Running posttrial");
            yield return RunPostTrial();
        }
        else {
            Debug.Log($"{TrialText} posttrial interrupted");
        }

        Debug.Log($"finalizing from within run");
        FinalizeTrial();
        if (!interrupt) {
            ExperimentEvents.TrialHasCompleted();
        }

    }

    void InitializeTrial() {
        CompletedSuccessfully = false;
        interrupt = false;
        trialRunning = true;
    }

    public void FinalizeTrial() {
        trialRunning = false;
        if (!interrupt) {
            Debug.Log($"Finalizing {TrialText}");
            CompletedSuccessfully = true;
            Attempts++;
        }
        
    }

    public IEnumerator RunExperimentControls() {

        while (trialRunning) {

            //let things from last frame finish up
            yield return null;

            if (Input.GetKeyDown(KeyCode.Backspace)) {
                Debug.Log($"detected skip key");
                interrupt = true;
                trialRunning = false;
                Skipped = true;
                Debug.Log("Finalizing from within controls");
                FinalizeTrial();
                //Let notifications disperse through program for a frame
                yield return null;

                ExperimentEvents.InterruptTrial();
            }

            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow)) {
                Debug.Log($"detected last key");
                interrupt = true;
                trialRunning = false;
                Debug.Log("Finalizing from within controls");
                FinalizeTrial();
                //Let notifications disperse through program for a frame
                yield return null;

                ExperimentEvents.GoBackOneTrial();
                
            }

            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow)) {
                Debug.Log($"detected next key");
                interrupt = true;
                trialRunning = false;
                Debug.Log("Finalizing from within controls");
                FinalizeTrial();
                //Let notifications disperse through program for a frame
                yield return null;


                ExperimentEvents.SkipToNextTrial();
                
            }

            if (!trialRunning) {
                runner.StopCoroutine(this.Run(runner));
            }

        }
        //Let notifications disperse through program for a frame
        yield return null;
    }


    public void Interrupt() {
        interrupt = true;
    }

    protected virtual IEnumerator RunPreTrial() {
        Debug.Log($"{TrialText} Skipped Pretrial");
        yield return null;
    }

    protected abstract IEnumerator RunMainTrial(); 

    protected virtual IEnumerator RunPostTrial() {
        Debug.Log($"{TrialText} Skipped Post-trial code");
        yield return null;
    }

}


public class TestTrial : Trial {


    public TestTrial(DataRow data) : base(data) {

    }
    
    protected override IEnumerator RunMainTrial() {
        bool running = true;
        while (running) {
            if (Input.GetKeyDown(KeyCode.Return)) {
                Debug.Log($"{TrialText} Return key pressed");
                running = false;
            }

            yield return null;
        }
    }
}
