using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrialSequence : MonoBehaviour {
    //List<TrialDefinition> trialsInBlock = new List<TrialDefinition>();
    //List<List<TrialDefinition>> blocks = new List<List<TrialDefinition>>();
    List<Trial> trials;

    Trial currentTrial;
    Coroutine currentTrialRunning;
    //int totaltrialCounter = 0;

    //int currentBlock = 0;
    //int currentTrialInBlock = 0;


    void Start() {
        trials = new List<Trial>();
        for (int i = 0; i < 5; i++) {
            trials.Add(new TestTrial());
        }
        StartRunningTrial(trials[0]);
    }

    void OnEnable() {
        ExperimentEventManager.OnTrialInterrupt += InterruptedTrial;
        ExperimentEventManager.OnTrialStart += TrialStarted;
        ExperimentEventManager.OnTrialCompleted += TrialDone;
        ExperimentEventManager.OnSkipToNextTrial += GoToNextTrial;
        ExperimentEventManager.OnGoBackOneTrial += BackOneTrial;
    }

    void OnDisable() {
        ExperimentEventManager.OnTrialInterrupt -= InterruptedTrial;
        ExperimentEventManager.OnTrialStart -= TrialStarted;
        ExperimentEventManager.OnTrialCompleted -= TrialDone;
        ExperimentEventManager.OnSkipToNextTrial -= GoToNextTrial;
    }

    void TrialStarted(Trial trial) {
        int trialNum = TrialIndex(trial);
        Debug.Log($"*****\n\n");
        Debug.Log($"Starting trial {trialNum}");
    }

    void StartRunningTrial(Trial trial) {
        currentTrial = trial;
        currentTrialRunning = StartCoroutine(trial.Run(TrialIndex(trial)));
    }

    void TrialDone(Trial currentTrial) {
        int trialNum = TrialIndex(currentTrial);
        Debug.Log($"Done Trial {trialNum}");
        GoToNextTrial(currentTrial);
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

    void DoneTrials() {
        Debug.Log("---------------");
        Debug.Log("Done all trials");
    }

    void InterruptedTrial(Trial trial) {
        Debug.LogWarning("Got interrupt event from trial");
    }

    void BackOneTrial(Trial currentTrial) {
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

public static class IntExtensions {
    public static bool IsWithin(this int value, int minimum, int maximum) {
        return value >= minimum && value <= maximum;
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

}



