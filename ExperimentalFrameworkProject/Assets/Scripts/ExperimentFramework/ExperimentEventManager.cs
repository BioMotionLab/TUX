using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperimentEventManager : MonoBehaviour {

    public delegate void StartTrialEvent(Trial trial);
    public static event StartTrialEvent OnTrialStart;

    public delegate void InterruptTrialEvent(Trial trial);
    public static event InterruptTrialEvent OnTrialInterrupt;

    public delegate void CompletedTrialEvent(Trial trial);
    public static event CompletedTrialEvent OnTrialCompleted;

    public delegate void GoBackOneTrialEvent(Trial trial);
    public static event GoBackOneTrialEvent OnGoBackOneTrial;

    public delegate void SkipToNextTrialEvent(Trial trial);
    public static event SkipToNextTrialEvent OnToNextTrial;

    public static void StartingTrial(Trial trial) {
        OnTrialStart?.Invoke(trial);
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
}
