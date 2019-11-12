using System.Collections;
using System.Data;
using UnityEngine;

namespace bmlTUX.Scripts.ExperimentParts.SimpleExperimentParts {


    /// <inheritdoc />
    /// <summary>
    /// This is the simplest trial possible. It is used when no custom trial is specified.
    /// This trial overwrites only the MainCoroutine() method, making the program wait for the return key to be pressed.
    /// The trial ends when the key is pressed.
    /// </summary>
    public class SimpleTrial : Trial {
        


        /// <summary>
        /// Constructor just calls the base class Trial's constructor.
        /// </summary>
        /// <param name="runner">The Runner being run</param>
        /// <param name="data">The row of BlockData for this trial from a table</param>
        public SimpleTrial(ExperimentRunner runner, DataRow data) : base(runner, data) {
        }


        /// <inheritdoc />
        /// <summary>
        /// Overwrites the MainCoroutine method to provide the trial's functionality
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator RunMainCoroutine() {
            bool running = true;
            Debug.Log("...Waiting for you to press return key! (This is from SimpleTrial MainCoroutine() method)");
       
            //Loop over several frames
            while (running) {
                
                //End Trial based on user input
                if (Input.GetKeyDown(KeyCode.Return)) {
                    Debug.Log($"Return key pressed!");
                    running = false;
                }
                
                yield return null; //IMPORTANT: This lets it wait for next frame, and not hang program.
            }
        }
    }
}