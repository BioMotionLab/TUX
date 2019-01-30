using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperimentEventManager : MonoBehaviour {

    public delegate void StartTrialEvent(Trial trial);
    public static event StartTrialEvent OnTrialStart;

    public delegate void TrialStartedEvent(Trial trial);
    public static event TrialStartedEvent OnTrialStarted;

    public delegate void InterruptTrialEvent(Trial trial);
    public static event InterruptTrialEvent OnTrialInterrupt;

    public delegate void CompletedTrialEvent(Trial trial);
    public static event CompletedTrialEvent OnTrialCompleted;

    public delegate void GoBackOneTrialEvent(Trial trial);
    public static event GoBackOneTrialEvent OnGoBackOneTrial;

    public delegate void SkipToNextTrialEvent(Trial currentTrial);
    public static event SkipToNextTrialEvent OnToNextTrial;

    public delegate void JumpToTrialEvent(int indexToJumpTo);
    public static event JumpToTrialEvent OnJumpToTrial;

    public delegate void OnTrialListUpdatedEvent(List<Trial> trials);
    public static event OnTrialListUpdatedEvent OnTrialListUpdated;

    public delegate void OnExperimentStartEvent(List<Trial> trials, string header);
    public static event OnExperimentStartEvent OnExperimentStart;

    public delegate void OnExperimentEndEvent();
    public static event OnExperimentEndEvent OnExperimentEnd;


    public static void StartingTrial(Trial trial) {
        OnTrialStart?.Invoke(trial);
    }

    public static void TrialStarted(Trial trial) {
        OnTrialStarted?.Invoke(trial);
    }

    public static void UpdateTrialList(List<Trial> trials, int index) {
        OnTrialListUpdated?.Invoke(trials);
    }

    public static void ExperimentStart(List<Trial> trials, string header) {
        OnExperimentStart?.Invoke(trials, header);

    }
    public static void ExperimentEnd() {
        OnExperimentEnd?.Invoke();

    }


    public static void InterruptTrial(Trial trial) {
        OnTrialInterrupt?.Invoke(trial);
    }

    public static void EndTrial(Trial trial) {
        OnTrialCompleted?.Invoke(trial);
    }

    public static void GoBackOneTrial(Trial trial) {
        OnGoBackOneTrial?.Invoke(trial);
    }

    public static void SkipToNextTrial(Trial trial) {
        OnToNextTrial?.Invoke(trial);
    }

    public static void JumpToTrial(int indexToJumpTo) {
        OnJumpToTrial?.Invoke(indexToJumpTo);
    }
}
