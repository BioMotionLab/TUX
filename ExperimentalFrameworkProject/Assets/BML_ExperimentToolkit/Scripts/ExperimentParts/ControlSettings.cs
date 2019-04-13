using System.Collections;
using BML_ExperimentToolkit.Scripts.Managers;
using BML_Utilities;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {

    [CreateAssetMenu(menuName = MenuNames.BmlAssetMenu + "Create Control Settings Asset")]
    public class ControlSettings : ScriptableObject {
        public KeyCode InterruptKey;
        public KeyCode BackKey;
        public KeyCode NextKey;

        /// <summary>
        /// Allows experimenter to control the experiments and jump between trials.
        /// </summary>
        /// <returns></returns>
        public IEnumerator Run() {
            //TODO let user select keycodes in inspector using scriptable object settings
            const bool running = true;

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            while (running) {

                if (Input.GetKeyDown(InterruptKey)) {
                    ExperimentEvents.InterruptTrial();
                }

                if (Input.GetKeyDown(BackKey)) {
                    ExperimentEvents.GoBackOneTrial();
                }

                if (Input.GetKeyDown(NextKey)) {
                    ExperimentEvents.SkipToNextTrial();
                }

                yield return null;

            }
            // ReSharper disable once IteratorNeverReturns
        }

    }
}