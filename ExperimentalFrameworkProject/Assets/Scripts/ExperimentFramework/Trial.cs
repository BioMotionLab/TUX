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

    public int Index => (int)data[Config.IndexColumnName];

    protected Trial(DataRow data, Config configFile) {
        this.data = data;
        this.configFile = configFile;
    }


    public IEnumerator Run() {
        //Skip a frame to allow any previous things to end
        yield return null;

        ExperimentEventManager.StartingTrial(this);

        Debug.Log($"Trail {Index} Waiting for return key");
        bool waiting = true;
        bool successful = false;
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
                successful = true;
                ReturnPressed();
            }

            
            
        }

        if (successful) {
            Debug.Log($"Trail {Index} Trial completed successfully");
            ExperimentEventManager.EndTrial(this);
        }
        else {
            Debug.LogWarning($"Trail {Index} Trial completed unsuccessfully");
        }
        
        yield return null;

    }

    void ReturnPressed() {
        Debug.Log($"Trail {Index} Return key pressed");
    }

}