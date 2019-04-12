using System.Collections;
using BML_ExperimentToolkit.Scripts.Managers;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {
    public static class ExperimentControls {

        /// <summary>
        /// Allows experimenter to control the experiments and jump between trials.
        /// </summary>
        /// <returns></returns>
        public static IEnumerator RunExperimentControls() {
            //TODO let user select keycodes in inspector using scriptable object settings
            const bool running = true;
            
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            while (running) {

                if (Input.GetKeyDown(KeyCode.Backspace)) {
                    ExperimentEvents.InterruptTrial();
                }

                if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow)) {
                    ExperimentEvents.GoBackOneTrial();
                }

                if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow)) {
                    ExperimentEvents.SkipToNextTrial();
                }

                yield return null;
                
            }
            // ReSharper disable once IteratorNeverReturns
        }
    }
}
