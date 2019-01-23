﻿using System;
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

    public int Index => (int)data[Config.IndexColumnName];

    public bool Success {
        get {
            return (bool)data[Config.SuccessColumnName];
        }
        set {
            data[Config.SuccessColumnName] = value;
        }
    }

    protected Trial(DataRow data, Config configFile) {
        this.data = data;
        this.configFile = configFile;
    }


    public IEnumerator Run() {
        //Skip a frame to allow any previous things to end
        Success = false;
        yield return null;

        ExperimentEventManager.StartingTrial(this);

        Debug.Log($"Trail {Index} Waiting for return key");
        bool waiting = true;
        while (waiting) {


            if (Input.GetKeyDown(KeyCode.Backspace)) {
                ExperimentEventManager.InterruptTrial(this);
                Debug.Log($"Trail {Index} finished sending interrupt event");
                break;
            }

            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                ExperimentEventManager.GoBackOneTrial(this);
                break;
            }

            if (Input.GetKeyDown(KeyCode.DownArrow)) {
                ExperimentEventManager.SkipToNextTrial(this);
                break;
            }

            yield return null;
            if (Input.GetKeyDown(KeyCode.Return)) {
                waiting = false;
                Success = true;
                ReturnPressed();
            }

            
            
        }

        if (Success) {
            Debug.Log($"Trial {Index} completed successfully");
            ExperimentEventManager.EndTrial(this);
        }
        
        
        yield return null;

    }

    void ReturnPressed() {
        Debug.Log($"Trail {Index} Return key pressed");
    }

}