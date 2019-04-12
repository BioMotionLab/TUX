using System.Collections;
using BML_ExperimentToolkit.Scripts.Managers;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {
    public class ExperimentControls {
        readonly ControlSettings controlSettings = new ControlSettings();

        /// <summary>
        /// Allows experimenter to control the experiments and jump between trials.
        /// </summary>
        /// <returns></returns>
        public IEnumerator Run() {
            //TODO let user select keycodes in inspector using scriptable object settings
            const bool running = true;
            
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            while (running) {

                if (Input.GetKeyDown(controlSettings.InterruptKey)) {
                    ExperimentEvents.InterruptTrial();
                }

                if (Input.GetKeyDown(controlSettings.BackKey)) {
                    ExperimentEvents.GoBackOneTrial();
                }

                if (Input.GetKeyDown(controlSettings.NextKey)) {
                    ExperimentEvents.SkipToNextTrial();
                }

                yield return null;
                
            }
            // ReSharper disable once IteratorNeverReturns
        }
    }
}
