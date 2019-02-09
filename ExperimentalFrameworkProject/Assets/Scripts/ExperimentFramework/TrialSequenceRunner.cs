using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class TrialSequenceRunner {

    Experiment experiment;

    Trial currentlyRunningTrial;

    List<Trial> trialsInSequence;
    List<Trial> currentTrialList;

    public TrialSequenceRunner(Experiment experiment, List<Trial> trialList) {
        OnEnable();
        this.experiment = experiment;
        this.trialsInSequence = trialList;
        this.currentTrialList = trialList;
    }

    public void Start() {
        Debug.Log("Starting to run trial sequence");
        StartRunningTrial(currentTrialList[0]);
    }

    void OnEnable() {
        ExperimentEvents.OnTrialInterrupt += InterruptTrial;
        ExperimentEvents.OnTrialHasCompleted += TrialHasCompleted;
        ExperimentEvents.OnSkipToNextTrial += SkipToNextTrial;
        ExperimentEvents.OnGoBackOneTrial += BackOneTrial;
        ExperimentEvents.OnJumpToTrial += JumpToTrial;
    }

    void OnDisable() {
        ExperimentEvents.OnTrialInterrupt -= InterruptTrial;
        ExperimentEvents.OnTrialHasCompleted -= TrialHasCompleted;
        ExperimentEvents.OnSkipToNextTrial -= SkipToNextTrial;
        ExperimentEvents.OnGoBackOneTrial -= BackOneTrial;
        ExperimentEvents.OnJumpToTrial -= JumpToTrial;
    }


    void StartRunningTrial(Trial trial) {
        currentlyRunningTrial = trial;
        ExperimentEvents.TrialHasStarted(trial, TrialIndex(trial));
        experiment.StartCoroutine(trial.Run());
    }

    void TrialHasCompleted() {
        FinishTrial();
        GoToNextTrial();
    }

    void FinishTrial() {
        int trialNum = TrialCurrentIndex(currentlyRunningTrial);
        
        Debug.Log($"Done Trial {trialNum + 1}, success = {currentlyRunningTrial.CompletedSuccesssfully}\n" +
                  $"trialTable:\n" +
                  $"{currentlyRunningTrial.Data.AsString(header:true)}");
        ExperimentEvents.UpdateTrial(trialsInSequence, TrialIndex(currentlyRunningTrial));
    }

    void GoToNextTrial() {
        int newIndex = TrialCurrentIndex(currentlyRunningTrial) + 1;
        
        bool searchingForUncompletedTrial = true;
        while (searchingForUncompletedTrial) {
            if (newIndex > currentTrialList.Count - 1) {
                searchingForUncompletedTrial = false;
                DoneTrialSequence();
            }
            else if (!currentTrialList[newIndex].CompletedSuccesssfully) {
                searchingForUncompletedTrial = false;
                Trial nextTrial = currentTrialList[newIndex];
                StartRunningTrial(nextTrial);
            }
            newIndex++;

        }
    }

    void SkipToNextTrial() {
        Debug.Log("Got Next Trial event");
        FinishTrial();
        GoToNextTrial();
    }

    void DoneTrialSequence() {
        Debug.Log("---------------");
        Debug.Log("Done all trials in sequence");

        List<Trial> unsuccessfulTrials = new List<Trial>();
        foreach (Trial trial in currentTrialList) {
            if (!trial.CompletedSuccesssfully && !trial.Skipped) {
                unsuccessfulTrials.Add(trial);
            }
        }


        if (unsuccessfulTrials.Count > 0) { //if any trials not complete, run them
            Debug.Log($"Running {unsuccessfulTrials.Count} unsuccessful trials");
            currentTrialList = unsuccessfulTrials;
            StartRunningTrial(unsuccessfulTrials[0]);
        }
        else {
            // finish up
            Debug.Log($"No more trials");
            ExperimentEvents.TrialSequenceHasCompleted(trialsInSequence);
            OnDisable();
        }
    }


    void InterruptTrial() {
        Debug.LogWarning("Got Skip event from currentTrial");
        currentlyRunningTrial.Skipped = true;
        FinishTrial();
        GoToNextTrial();
    }

    void BackOneTrial() {
        Debug.Log("Got Back event");
        FinishTrial();
        int newIndex = TrialCurrentIndex(currentlyRunningTrial) - 1;
        if (newIndex < 0) {
            Debug.LogWarning("Was already at first currentTrial, restarting currentTrial");
            StartRunningTrial(currentlyRunningTrial);
        }
        else {
            Debug.Log("Going back one currentTrial"); 
            Trial prevTrial = currentTrialList[newIndex];
            StartRunningTrial(prevTrial);
        }
    }

    void JumpToTrial(int jumpToIndex) {
        Debug.Log("Got jump event");
        FinishTrial();
        currentlyRunningTrial.Interrupt();
        currentTrialList = trialsInSequence;
        Trial newTrial = currentTrialList[jumpToIndex];
        StartRunningTrial(newTrial);
    }


    int TrialCurrentIndex(Trial trial) {
        return currentTrialList.IndexOf(trial);
    }

    int TrialIndex(Trial trial) {
        return trialsInSequence.IndexOf(trial);
    }

}


public class TestTrial : Trial {
    

    public TestTrial(DataRow data) : base(data) {
        
    }


}



