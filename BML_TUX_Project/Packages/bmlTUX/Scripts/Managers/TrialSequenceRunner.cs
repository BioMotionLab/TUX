using System.Collections.Generic;
using bmlTUX.Scripts.ExperimentParts;
using bmlTUX.Scripts.Utilities;
using bmlTUX.Scripts.Utilities.Extensions;
using UnityEngine;

namespace bmlTUX.Scripts.Managers {


    public class TrialSequenceRunner {

        Trial currentlyRunningTrial;
        public bool Running = false;

        readonly List<Trial> trialsInSequence;
        List<Trial> currentTrialList;


        public TrialSequenceRunner(List<Trial> trialList) {
            OnEnable();
            trialsInSequence = trialList;
            currentTrialList = trialList;
        }

        
        public void Start() {
            Running = true;
            if (currentTrialList.Count == 0) {
                Debug.LogError(TuxLog.Error("No Trials Defined! You probably didn't set up a design file."));
                return;
            }
            StartRunningTrial(currentTrialList[0]);
        }

        void OnEnable() {
            ExperimentEvents.OnTrialInterrupt += InterruptTrial;
            ExperimentEvents.OnTrialHasCompleted += TrialHasCompleted;
            ExperimentEvents.OnSkipToNextTrial += SkipToNextTrial;
            ExperimentEvents.OnGoBackOneTrial += BackOneTrial;
            ExperimentEvents.OnJumpToTrial += JumpToTrial;
            ExperimentEvents.OnOutputSuccessfullyUpdated += LogTrial;
        }

        void OnDisable() {
            ExperimentEvents.OnTrialInterrupt -= InterruptTrial;
            ExperimentEvents.OnTrialHasCompleted -= TrialHasCompleted;
            ExperimentEvents.OnSkipToNextTrial -= SkipToNextTrial;
            ExperimentEvents.OnGoBackOneTrial -= BackOneTrial;
            ExperimentEvents.OnJumpToTrial -= JumpToTrial;
            ExperimentEvents.OnOutputSuccessfullyUpdated -= LogTrial;
        }


        void StartRunningTrial(Trial trial) {
            currentlyRunningTrial = trial;

            ExperimentEvents.StartPart(trial);
            ExperimentEvents.TrialHasStarted(trial);
        }

        void TrialHasCompleted() {
            FinishTrial();
            GoToNextTrial();
        }

        void FinishTrial() {
            ExperimentEvents.UpdateTrial(trialsInSequence, TrialIndex(currentlyRunningTrial));
        }

        void LogTrial(string filePath) {
            string successText = currentlyRunningTrial.CompletedSuccessfully ? "successfully" : "<color=red><b>unsuccessful</b></color>";
            Debug.Log($"{TuxLog.Prefix} <color=green><b>Finished</b></color> {currentlyRunningTrial.TrialText} {successText}. \tOutput Updated: {filePath} \n" +
                      $"Output Table for this trial:\n" +
                      $"{currentlyRunningTrial.Data.AsString(header: true)}");
        }

        void GoToNextTrial() {
            int newIndex = TrialCurrentIndex(currentlyRunningTrial) + 1;

            bool searchingForUncompletedTrial = true;
            while (searchingForUncompletedTrial) {
                if (newIndex > currentTrialList.Count - 1) {
                    searchingForUncompletedTrial = false;
                    DoneTrialSequence();
                }
                else if (!currentTrialList[newIndex].CompletedSuccessfully) {
                    searchingForUncompletedTrial = false;
                    Trial nextTrial = currentTrialList[newIndex];
                    StartRunningTrial(nextTrial);
                }

                newIndex++;

            }
        }

        void SkipToNextTrial() {
            currentlyRunningTrial.InterruptTrial();
            FinishTrial();

            int newIndex = TrialCurrentIndex(currentlyRunningTrial) + 1;
            if (newIndex > currentTrialList.Count - 1) {
                Debug.LogWarning($"{TuxLog.Prefix} Already at final trial, restarting current Trial");
                StartRunningTrial(currentlyRunningTrial);
            }
            else {
                Debug.Log($"{TuxLog.Prefix} Going to next trial");
                Trial next = currentTrialList[newIndex];
                StartRunningTrial(next);
            }

        }

        void DoneTrialSequence() {
            List<Trial> unsuccessfulTrials = new List<Trial>();
            foreach (Trial trial in currentTrialList) {
                if (!trial.CompletedSuccessfully && !trial.Skipped) {
                    unsuccessfulTrials.Add(trial);
                }
            }


            if (unsuccessfulTrials.Count > 0) {
                //if any trials not complete, run them
                Debug.Log($"{TuxLog.Prefix} Running {unsuccessfulTrials.Count} unsuccessful trials");
                currentTrialList = unsuccessfulTrials;
                StartRunningTrial(unsuccessfulTrials[0]);
            }
            else {
                // finish up
                ExperimentEvents.TrialSequenceHasCompleted(trialsInSequence);
                Running = false;
                OnDisable();
            }
        }


        void InterruptTrial() {
            Debug.LogWarning($"{TuxLog.Prefix} Got SkipCompletely event from currentTrial");
            currentlyRunningTrial.SkipCompletely();
            FinishTrial();
            GoToNextTrial();
        }

        void BackOneTrial() {
            Debug.LogWarning($"{TuxLog.Prefix} Got Back event");
            currentlyRunningTrial.InterruptTrial();
            FinishTrial();
            int newIndex = TrialCurrentIndex(currentlyRunningTrial) - 1;
            if (newIndex < 0) {
                Debug.LogWarning($"{TuxLog.Prefix} Was already at first Trial, restarting current Trial");
                StartRunningTrial(currentlyRunningTrial);
            }
            else {
                Debug.Log($"{TuxLog.Prefix} Going back one Trial");
                Trial prevTrial = currentTrialList[newIndex];
                StartRunningTrial(prevTrial);
            }
        }

        void JumpToTrial(int jumpToIndex) {
            Debug.Log($"{TuxLog.Prefix} Got jump event");
            FinishTrial();
            currentlyRunningTrial.InterruptTrial();
            currentTrialList = trialsInSequence;
            Trial newTrial = currentTrialList[jumpToIndex];
            StartRunningTrial(newTrial);
        }


        int TrialCurrentIndex(Trial trial) {
            return currentTrialList.IndexOf(trial);
        }

        int TrialIndex(Trial trial) {
            return trialsInSequence.IndexOf(trial);
        }

    }

}


