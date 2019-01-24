using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class TrialSequence : MonoBehaviour {
    public Config ConfigFile;


    DataTable trialTable;
    readonly List<Trial> allTrials = new List<Trial>();
    List<Trial> currentTrialList = new List<Trial>();


    Trial currentTrial;
    Coroutine currentTrialRunning;
    //int totaltrialCounter = 0;    

    //int currentBlock = 0;
    //int currentTrialInBlock = 0;


    void Start() {
        trialTable = ConfigFile.TrialTable;
        int i = 1;
        Debug.Log($"number of rows in currentTrial table = {trialTable.Rows.Count}");
        foreach (DataRow row in trialTable.Rows) {
            row[Config.IndexColumnName] = i;
            Trial newTrial = new TestTrial(row, ConfigFile);
            Debug.Log("Adding Trial to list");
            allTrials.Add(newTrial);
            i++;
        }
        Debug.Log("Starting to run experiment");
        currentTrialList = allTrials;
        StartRunningTrial(currentTrialList[0]);

    }

    void OnEnable() {
        ExperimentEventManager.OnTrialInterrupt += SkipTrial;
        ExperimentEventManager.OnTrialStart += TrialStarted;
        ExperimentEventManager.OnTrialCompleted += TrialDone;
        ExperimentEventManager.OnToNextTrial += ToNextTrial;
        ExperimentEventManager.OnGoBackOneTrial += BackOneTrial;
    }

    void OnDisable() {
        ExperimentEventManager.OnTrialInterrupt -= SkipTrial;
        ExperimentEventManager.OnTrialStart -= TrialStarted;
        ExperimentEventManager.OnTrialCompleted -= TrialDone;
        ExperimentEventManager.OnToNextTrial -= ToNextTrial;
        ExperimentEventManager.OnGoBackOneTrial -= BackOneTrial;
    }

    void TrialStarted(Trial trial) {
        int trialNum = TrialIndex(trial);
        Debug.Log($"*****\n\n");
        Debug.Log($"Starting currentTrial {trialNum+1}/{currentTrialList.Count}");
    }

    void StartRunningTrial(Trial trial) {
        currentTrial = trial;
        currentTrialRunning = StartCoroutine(trial.Run());
        trial.Attempts++;
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
        if (newIndex >= currentTrialList.Count) {
            DoneAllTrials();
        }
        else {
            Trial nextTrial = currentTrialList[newIndex];
            StartRunningTrial(nextTrial);
        }
    }

    void ToNextTrial(Trial currentTrial) {
        Debug.Log("Got Next Trial event");
        FinishTrial(currentTrial);
        GoToNextTrial(currentTrial);
    }

    void DoneAllTrials() {
        Debug.Log("---------------");
        Debug.Log("Done all currentTrialList");

        List<Trial> unsuccessfulTrials = new List<Trial>();
        foreach (Trial trial in currentTrialList) {
            if (!trial.Success && !trial.Skipped) {
                unsuccessfulTrials.Add(trial);
            }
        }


        if (unsuccessfulTrials.Count > 0) { //if any trials not complete, run them
            Debug.Log($"Running {unsuccessfulTrials.Count} unsuccessful trials");
            currentTrialList = unsuccessfulTrials;
            StartRunningTrial(unsuccessfulTrials[0]);
        }
        else { // finish up
            Debug.Log($"No more unsuccessful trials");
            DataTable endTable = trialTable.Clone();
            foreach (Trial trial in allTrials) {
                endTable.ImportRow(trial.Data);
            }
            Debug.Log(endTable.AsString());
        }
        
    }

    void SkipTrial(Trial currentTrial) {
        Debug.LogWarning("Got Skip event from currentTrial");
        currentTrial.Skipped = true;
        FinishTrial(currentTrial);
        GoToNextTrial(currentTrial);
    }

    void BackOneTrial(Trial currentTrial) {
        Debug.Log("Got Back event");
        FinishTrial(currentTrial);
        int newIndex = TrialIndex(currentTrial) - 1;
        if (newIndex < 0) {
            Debug.LogWarning("Was already at first currentTrial, restarting currentTrial");
            StartRunningTrial(currentTrial);
        }
        else {
            Debug.Log("Going back one currentTrial"); 
            Trial prevTrial = currentTrialList[newIndex];
            StartRunningTrial(prevTrial);
        }
    }

    int TrialIndex(Trial trial) {
        return currentTrialList.IndexOf(trial);
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


public class TestTrial : Trial {
    

    public TestTrial(DataRow data, Config config) : base(data, config) {
        
    }


}



