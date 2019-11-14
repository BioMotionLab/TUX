using System;
using System.Collections;
using System.Collections.Generic;
using bmlTUX.Scripts.Managers;
using UnityEngine;

namespace bmlTUX.Scripts.Settings {

    [CreateAssetMenu(menuName = TUXMenuNames.AssetCreationMenu + "Control Settings File")]
    public class ControlSettings : ScriptableObject {
        public List<KeyCode> InterruptKeys;
        public List<KeyCode> BackKeys;
        public List<KeyCode> NextKeys;
        public List<KeyCode> QuitKeys;
        public bool listening { get; private set; }

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

        public IEnumerator ListenForQuit() {
            listening = true;
            while (listening) {
                
                foreach (KeyCode quitKey in QuitKeys) {
                    if (Input.GetKeyDown(quitKey)) {
                        Debug.LogWarning($"Quit Key Pressed ({quitKey}), quitting program.");
                        listening = true;
                        #if UNITY_EDITOR
                            UnityEditor.EditorApplication.isPlaying = false;
                        #else
                            Application.Quit();
                        #endif
                    }
                }
                yield return null;
            }
            
            // ReSharper disable once IteratorNeverReturns
        }
 
    }
}