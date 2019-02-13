using System.Collections;
using System.Data;
using UnityEngine;

public class SimpleTrial : Trial {


    public SimpleTrial(DataRow data) : base(data) {

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