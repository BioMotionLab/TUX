using System.Collections.Generic;
using BML_ExperimentToolkit.Scripts.ExperimentParts;
using BML_Utilities.Extensions;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.Managers {


    public class TrialSequenceRunner {

        Trial currentlyRunningTrial;

        List<Trial> trialsInSequence;
        List<Trial> currentTrialList;


        public TrialSequenceRunner(ExperimentRunner runner, List<Trial> trialList) {
            OnEnable();
            trialsInSequence = trialList;
            currentTrialList = trialList;
        }

        public void Start() {
            //Debug.Log("Starting to run trial sequence");
            StartRunningTrial(currentTrialList[0]);
        }

        void OnEnable() {
            ExperimentEvents.OnTrialInterrupt += InterruptTrial;
            ExperimentEvents.OnTrialHasCompleted += TrialHasCompleted;
            ExperimentEvents.OnSkipToNextTrial += SkipToNextTrial;
            ExperimentEvents.OnGoBackOneTrial += BackOneTrial;
            ExperimentEvents.OnJumpToTrial += JumpToTrial;
        }

        void OnDisable() {
            ExperimentEvents.OnTrialInterrupt -= InterruptTrial;
            ExperimentEvents.OnTrialHasCompleted -= TrialHasCompleted;
            ExperimentEvents.OnSkipToNextTrial -= SkipToNextTrial;
            ExperimentEvents.OnGoBackOneTrial -= BackOneTrial;
            ExperimentEvents.OnJumpToTrial -= JumpToTrial;
        }


        void StartRunningTrial(Trial trial) {
            currentlyRunningTrial = trial;

            ExperimentEvents.StartPart(trial);
            ExperimentEvents.TrialHasStarted(trial, TrialIndex(trial));
        }

        void TrialHasCompleted() {
            Debug.Log("Trial has completed event received)");
            FinishTrial();
            GoToNextTrial();
        }

        void FinishTrial() {

            Debug.Log($"Finished {currentlyRunningTrial.TrialText} in Sequence, success = {currentlyRunningTrial.CompletedSuccessfully}\n" +
                      $"trialTable:\n" +
                      $"{currentlyRunningTrial.Data.AsString(header: true)}");

            ExperimentEvents.UpdateTrial(trialsInSequence, TrialIndex(currentlyRunningTrial));
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
            Debug.LogWarning("Got Next Trial event");
            FinishTrial();

            int newIndex = TrialCurrentIndex(currentlyRunningTrial) + 1;
            if (newIndex > currentTrialList.Count - 1) {
                Debug.LogWarning("Already at final trial, restarting current Trial");
                StartRunningTrial(currentlyRunningTrial);
            }
            else {
                Debug.Log("Going to next trial");
                Trial next = currentTrialList[newIndex];
                StartRunningTrial(next);
            }

        }

        void DoneTrialSequence() {
            Debug.Log("---------------");
            Debug.Log("Done all trials in sequence");

            List<Trial> unsuccessfulTrials = new List<Trial>();
            foreach (Trial trial in currentTrialList) {
                if (!trial.CompletedSuccessfully && !trial.Skipped) {
                    unsuccessfulTrials.Add(trial);
                }
            }


            if (unsuccessfulTrials.Count > 0) {
                //if any trials not complete, run them
                Debug.Log($"Running {unsuccessfulTrials.Count} unsuccessful trials");
                currentTrialList = unsuccessfulTrials;
                StartRunningTrial(unsuccessfulTrials[0]);
            }
            else {
                // finish up
                Debug.Log($"No more trials");
                ExperimentEvents.TrialSequenceHasCompleted(trialsInSequence);
                OnDisable();
            }
        }


        void InterruptTrial() {
            Debug.LogWarning("Got SkipTrial event from currentTrial");
            FinishTrial();
            GoToNextTrial();
        }

        void BackOneTrial() {
            Debug.LogWarning("Got Back event");
            FinishTrial();
            int newIndex = TrialCurrentIndex(currentlyRunningTrial) - 1;
            if (newIndex < 0) {
                Debug.LogWarning("Was already at first currentTrial, restarting currentTrial");
                StartRunningTrial(currentlyRunningTrial);
            }
            else {
                Debug.Log("Going back one currentTrial");
                Trial prevTrial = currentTrialList[newIndex];
                StartRunningTrial(prevTrial);
            }
        }

        void JumpToTrial(int jumpToIndex) {
            Debug.Log("Got jump event");
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


