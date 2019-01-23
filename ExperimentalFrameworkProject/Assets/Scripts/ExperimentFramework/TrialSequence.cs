using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class TrialSequence : MonoBehaviour {
    public Config configFile;
    DataTable trialTable;
    List<Trial> trials = new List<Trial>();

    Trial currentTrial;
    Coroutine currentTrialRunning;
    //int totaltrialCounter = 0;

    //int currentBlock = 0;
    //int currentTrialInBlock = 0;


    void Start() {
        trialTable = configFile.TrialTable;
        int i = 1;
        Debug.Log($"number of rows in trial table = {trialTable.Rows.Count}");
        foreach (DataRow row in trialTable.Rows) {
            row[Config.IndexColumnName] = i;
            Trial newTrial = new TestTrial(row, configFile);
            Debug.Log("Adding Trial to list");
            trials.Add(newTrial);
            i++;
        }
        Debug.Log("Starting to run experiment");
        StartRunningTrial(trials[0]);

    }

    void OnEnable() {
        ExperimentEventManager.OnTrialInterrupt += InterruptedTrial;
        ExperimentEventManager.OnTrialStart += TrialStarted;
        ExperimentEventManager.OnTrialCompleted += TrialDone;
        ExperimentEventManager.OnSkipToNextTrial += SkipTrial;
        ExperimentEventManager.OnGoBackOneTrial += BackOneTrial;
    }

    void OnDisable() {
        ExperimentEventManager.OnTrialInterrupt -= InterruptedTrial;
        ExperimentEventManager.OnTrialStart -= TrialStarted;
        ExperimentEventManager.OnTrialCompleted -= TrialDone;
        ExperimentEventManager.OnSkipToNextTrial -= SkipTrial;
        ExperimentEventManager.OnGoBackOneTrial -= BackOneTrial;
    }

    void TrialStarted(Trial trial) {
        int trialNum = TrialIndex(trial);
        Debug.Log($"*****\n\n");
        Debug.Log($"Starting trial {trialNum+1}/{trials.Count}");
    }

    void StartRunningTrial(Trial trial) {
        currentTrial = trial;
        currentTrialRunning = StartCoroutine(trial.Run());
    }

    

    void TrialDone(Trial currentTrial) {
        FinishTrial(currentTrial);
        GoToNextTrial(currentTrial);
    }

    void FinishTrial(Trial currentTrial) {
        int trialNum = TrialIndex(currentTrial);
        Debug.Log($"Done Trial {trialNum + 1}, success = {currentTrial.Success}");
        Debug.Log(currentTrial.Data.AsString());
    }

    void GoToNextTrial(Trial currentTrial) {
        int newIndex = TrialIndex(currentTrial) + 1;
        if (newIndex >= trials.Count) {
            DoneTrials();
        }
        else {
            Trial nextTrial = trials[newIndex];
            StartRunningTrial(nextTrial);
        }
        
    }

    void SkipTrial(Trial currentTrial) {
        Debug.Log("Got Skip event");
        ExperimentEventManager.InterruptTrial(currentTrial);
        GoToNextTrial(currentTrial);
    }

    void DoneTrials() {
        Debug.Log("---------------");
        Debug.Log("Done all trials");
    }

    void InterruptedTrial(Trial trial) {
        Debug.LogWarning("Got interrupt event from trial");
        FinishTrial(trial);
    }

    void BackOneTrial(Trial currentTrial) {
        Debug.Log("Got Back event");
        ExperimentEventManager.InterruptTrial(currentTrial);
        int newIndex = TrialIndex(currentTrial) - 1;
        if (newIndex < 0) {
            Debug.LogWarning("Was already at first trial, restarting trial");
            StartRunningTrial(currentTrial);
        }
        else {
            Debug.Log("Going back one trial"); 
            Trial prevTrial = trials[newIndex];
            StartRunningTrial(prevTrial);
        }
    }

    int TrialIndex(Trial trial) {
        return trials.IndexOf(trial);
    }

}



public interface Observer {
    void OnNotify(Subject subject, CustomEvent customEvent);
}

public enum CustomEvent {
    TrialComplete,
    BackOneTrial
}

public class Subject {
    List<Observer> observers = new List<Observer>();

    List<Observer> toRemove = new List<Observer>();

    public void AddObserver(Observer observer) {
        observers.Add(observer);
    }

    public void RemoveObserver(Observer observer) {
        toRemove.Remove(observer);
    }

    protected void Notify(CustomEvent customEvent) {
        RemoveExpiredObservers();
        foreach (var observer in observers) {
            observer.OnNotify(this, customEvent);
        }
        RemoveExpiredObservers();
        
    }

    void RemoveExpiredObservers() {
        foreach (var observer in toRemove) {
            observers.Remove(observer);
        }
        toRemove.Clear();
    }
}

public class TrialEvent {

}

public class TrialDefinition {
    public bool successfullyCompleted;
    public Trial trial;
    public int TrialInBlock;
    public int Block;
}

public class TestTrial : Trial {
    

    public TestTrial(DataRow data, Config config) : base(data, config) {
        
    }


}



