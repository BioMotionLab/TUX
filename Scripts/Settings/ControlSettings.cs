using System;
using System.Collections;
using System.Collections.Generic;
using bmlTUX.Scripts.Managers;
using UnityEngine;

namespace bmlTUX.Scripts.Settings {

    [Serializable]
    public class ControlSettings {
        
        public List<KeyCode> InterruptKeys = new List<KeyCode> {
            KeyCode.Slash
        };
        
        public List<KeyCode> BackKeys = new List<KeyCode> {
            KeyCode.Comma
        };
        
        public List<KeyCode> NextKeys = new List<KeyCode> {
            KeyCode.Period
        };
        
        public List<KeyCode> QuitKeys = new List<KeyCode> {
            KeyCode.Q,
            KeyCode.Escape
        };
        bool Listening { get; set; }

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
            Listening = true;
            while (Listening) {
                
                foreach (KeyCode quitKey in QuitKeys) {
                    if (Input.GetKeyDown(quitKey)) {
                        Debug.LogWarning($"{TuxLog.Prefix} Quit Key Pressed ({quitKey}), quitting program.");
                        Listening = true;
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