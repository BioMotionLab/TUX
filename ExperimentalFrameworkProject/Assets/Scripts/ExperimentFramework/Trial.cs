using System;
using System.Collections;
using UnityEngine;


/// <summary>
/// One Trial of an experiment. The experiment calls Run on this trial,
/// and it is in charge of setting and cleaning itself up
/// </summary>
public abstract class Trial {


    public IEnumerator Run(int index) {
        //Skip a frame to allow any previous things to end
        yield return null;

        ExperimentEventManager.StartingTrial(this);

        Debug.Log($"Trail {index} Waiting for return key");
        bool waiting = true;
        bool successful = false;
        while (waiting) {


            if (Input.GetKeyDown(KeyCode.Backspace)) {
                ExperimentEventManager.InterruptTrial(this);
                Debug.Log($"Trail {index} finished sending interrupt event");
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
                ReturnPressed(index);
            }

            
            
        }

        if (successful) {
            Debug.Log($"Trail {index} Trial completed successfully");
            ExperimentEventManager.EndTrial(this);
        }
        else {
            Debug.LogWarning($"Trail {index} Trial completed unsuccessfully");
        }
        
        yield return null;

    }

    void ReturnPressed(int index) {
        Debug.Log($"Trail {index} Return key pressed");
    }

}