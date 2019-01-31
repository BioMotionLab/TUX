using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;

public class TrialSequence : MonoBehaviour {
    public Config ConfigFile;


    DataTable trialTable;
    readonly List<Trial> allTrials = new List<Trial>();
    List<Trial> currentTrialList = new List<Trial>();
    Trial currentTrial;

    void Start() {
        trialTable = ConfigFile.TrialTable;
        if (trialTable.Rows.Count <= 0) {
            throw new InvalidDataException("Trial Table Not created correctly");
        }
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
        ExperimentEventManager.ExperimentStart(allTrials, trialTable.HeaderAsString());
        StartRunningTrial(currentTrialList[0]);

    }

    void OnEnable() {
        ExperimentEventManager.OnTrialInterrupt += SkipTrial;
        ExperimentEventManager.OnTrialStart += TrialStarted;
        ExperimentEventManager.OnTrialCompleted += TrialDone;
        ExperimentEventManager.OnToNextTrial += ToNextTrial;
        ExperimentEventManager.OnGoBackOneTrial += BackOneTrial;
        ExperimentEventManager.OnJumpToTrial += JumpToTrial;
    }

    void OnDisable() {
        ExperimentEventManager.OnTrialInterrupt -= SkipTrial;
        ExperimentEventManager.OnTrialStart -= TrialStarted;
        ExperimentEventManager.OnTrialCompleted -= TrialDone;
        ExperimentEventManager.OnToNextTrial -= ToNextTrial;
        ExperimentEventManager.OnGoBackOneTrial -= BackOneTrial;
        ExperimentEventManager.OnJumpToTrial -= JumpToTrial;
    }

    void TrialStarted(Trial trial) {
        int trialNum = TrialIndex(trial);
        Debug.Log($"*****\n\n");
        Debug.Log($"Starting currentTrial {trialNum+1}/{currentTrialList.Count}");
    }

    void StartRunningTrial(Trial trial) {
        ExperimentEventManager.TrialStarted(trial);
        currentTrial = trial;
        StartCoroutine(trial.Run());
    }

    void TrialDone(Trial currentTrial) {
        FinishTrial(currentTrial);
        currentTrial.Attempts++;
        GoToNextTrial(currentTrial);
    }

    void FinishTrial(Trial currentTrial) {
        int trialNum = TrialIndex(currentTrial);
        
        Debug.Log($"Done Trial {trialNum + 1}, success = {currentTrial.Success}");
        Debug.Log(currentTrial.Data.AsStringWithHeader(trialTable));
        ExperimentEventManager.UpdateTrialList(allTrials, TrialIndex(currentTrial));
    }

    void GoToNextTrial(Trial currentTrial) {
        int newIndex = TrialIndex(currentTrial) + 1;
        
        bool searchingForUncompletedTrial = true;
        while (searchingForUncompletedTrial) {
            if (newIndex > currentTrialList.Count - 1) {
                searchingForUncompletedTrial = false;
                DoneAllTrials();
            }
            else if (!currentTrialList[newIndex].Success) {
                searchingForUncompletedTrial = false;
                Trial nextTrial = currentTrialList[newIndex];
                StartRunningTrial(nextTrial);
            }
            newIndex++;

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
        else {
            // finish up
            Debug.Log($"No more unsuccessful trials");
            var endTable = CreateTableFromTrials(allTrials);
            Debug.Log(endTable.AsString());
            ExperimentEventManager.ExperimentEnd();
        }

        
    }

    DataTable CreateTableFromTrials(List<Trial> trialList) {
        DataTable newTable = trialTable.Clone();
        foreach (Trial trial in trialList) {
            newTable.ImportRow(trial.Data);
        }
        return newTable;
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

    void JumpToTrial(int jumpToIndex) {
        Debug.Log("Got jump event");
        FinishTrial(currentTrial);
        currentTrial.Interrupt();
        currentTrialList = allTrials;
        Trial newTrial = currentTrialList[jumpToIndex];
        StartRunningTrial(newTrial);
    }


    int TrialIndex(Trial trial) {
        return currentTrialList.IndexOf(trial);
    }

}


public class TestTrial : Trial {
    

    public TestTrial(DataRow data, Config config) : base(data, config) {
        
    }


}



