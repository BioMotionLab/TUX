using System.Collections;
using System.Data;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts.SimpleExperimentParts {

    public class SimpleTrial : Trial {


        public SimpleTrial(DataRow data) : base(data) {

        }

        protected override IEnumerator Main() {
            bool running = true;
            Debug.Log("...Waiting for you to press return key! (in SimpleTrial Main() method)");
            while (running) {
                if (Input.GetKeyDown(KeyCode.Return)) {
                    Debug.Log($"{TrialText} Return key pressed!");
                    running = false;
                }

                yield return null;
            }
        }
    }
}