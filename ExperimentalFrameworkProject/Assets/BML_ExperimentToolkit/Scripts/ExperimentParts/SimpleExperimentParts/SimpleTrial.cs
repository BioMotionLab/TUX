using System.Collections;
using System.Data;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts.SimpleExperimentParts {


    /// <inheritdoc />
    /// <summary>
    /// This is the simplest trial possible. It is used when no custom trial is specified.
    /// This trial overwrites only the MainCoroutine() method, making the program wait for the return key to be pressed.
    /// The trial ends when the key is pressed.
    /// </summary>
    public class SimpleTrial : Trial {

        SimpleExperiment simpleExperiment;

        /// <summary>
        /// Constructor just calls the base class Trail's constructor.
        /// </summary>
        /// <param name="experiment">The experiment being run</param>
        /// <param name="data">The row of BlockData for this trial from a table</param>
        public SimpleTrial(Experiment experiment, DataRow data) : base(experiment, data) {}

        protected override void PreMethod() {
           simpleExperiment  = (SimpleExperiment) Experiment;
        }

        /// <inheritdoc />
        /// <summary>
        /// Overwrites the MainCoroutine method to provide the trial's functionality
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator MainCoroutine() {
            bool running = true;
            Debug.Log("...Waiting for you to press return key! (in SimpleTrial MainCoroutine() method)");
            while (running) {
                if (Input.GetKeyDown(KeyCode.Return)) {
                    //Debug.Log($"{TrialText} Return key pressed!");
                    running = false;
                }

                yield return null;
            }
        }
    }
}