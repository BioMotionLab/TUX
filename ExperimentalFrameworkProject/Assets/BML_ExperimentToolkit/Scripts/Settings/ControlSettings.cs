using System.Collections;
using System.Collections.Generic;
using BML_ExperimentToolkit.Scripts.Managers;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.Settings {

    [CreateAssetMenu]
    public class ControlSettings : ScriptableObject {
        public List<KeyCode> InterruptKeys;
        public List<KeyCode> BackKeys;
        public List<KeyCode> NextKeys;

        /// <summary>
        /// Allows experimenter to control the experiments and jump between trials.
        /// </summary>
        /// <returns></returns>
        public IEnumerator Run() {
            const bool running = true;

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            while (running) {
                ListenForInterrupt();
                ListenForGoBackTrial();
                ListenForGoToNextTrial();
                yield return null;
            }
            // ReSharper disable once IteratorNeverReturns
        }

        void ListenForGoToNextTrial() {
            foreach (KeyCode nextKey in NextKeys) {
                if (Input.GetKeyDown(nextKey)) {
                    ExperimentEvents.SkipToNextTrial();
                }
            }
        }

        void ListenForGoBackTrial() {
            foreach (KeyCode backKey in BackKeys) {
                if (Input.GetKeyDown(backKey)) {
                    ExperimentEvents.GoBackOneTrial();
                }
            }
        }

        void ListenForInterrupt() {
            foreach (KeyCode interruptKey in InterruptKeys) {
                if (Input.GetKeyDown(interruptKey)) {
                    ExperimentEvents.InterruptTrial();
                }
            }
        }
    }
}