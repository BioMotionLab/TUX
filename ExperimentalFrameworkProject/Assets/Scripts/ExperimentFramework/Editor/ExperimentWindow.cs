using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public class ExperimentWindow : EditorWindow {
    static readonly GUILayoutOption CompleteIndicatorWidth = GUILayout.Width(20);
    static readonly GUILayoutOption JumpToButtonWidth = GUILayout.Width(40);
    static readonly GUILayoutOption RunningTrialIndicatorWidth = GUILayout.Width(60);
    int index = -1;
    List<Trial> trials = new List<Trial>();
    bool running = false;
    string header;

    bool initialized = false;
    Experiment experiment;

    // Add menu item named "My Window" to the Window menu
    [MenuItem("Experiment/My Window")]
    public static void ShowWindow() {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(ExperimentWindow));
    }

    void OnEnable() {
        ExperimentEvents.OnInitExperiment += InitWindow;
    }

    void InitWindow(Experiment experiment) {
        this.experiment = experiment;
        initialized = true;
    }


    void OnDisable() {
        ExperimentEvents.OnInitExperiment -= InitWindow;
    }

    void Update() {
        if (!Application.isPlaying) initialized = false;
    }

    void OnGUI() {
        EditorGUILayout.BeginVertical();

        GUILayout.Label("Experiment:", EditorStyles.boldLabel);

        if (!initialized) {
            EditorGUILayout.HelpBox("Not in PlayMode", MessageType.Error);
            return;
        }

        

        EditorGUILayout.TextArea(experiment.table.blockTable.AsString());
        //BLOCK DISPLAY
        EditorGUILayout.BeginHorizontal();
        string runningText = running ? "Running" : "Ended";
        EditorGUILayout.TextArea(runningText, RunningTrialIndicatorWidth);
        EditorGUILayout.TextArea("Jump to", JumpToButtonWidth);
        Color doneColor = running == true ? Color.blue : Color.green;
        EditorGUILayout.ColorField(GUIContent.none, doneColor, false, false, false, CompleteIndicatorWidth);
        EditorGUILayout.TextArea(header);
        EditorGUILayout.EndHorizontal();

        foreach (Trial trial in trials) {
            EditorGUILayout.BeginHorizontal();

            bool success = (bool)trial.Data[Config.SuccessColumnName];
            bool skip = (bool)trial.Data[Config.SkippedColumnName];
            Color color = success == true ? Color.green : Color.red;
            if (skip) color = Color.yellow;

            string trialRunningText = index == trials.IndexOf(trial) ? "Running" : "";

            EditorGUILayout.TextArea(trialRunningText, RunningTrialIndicatorWidth);
            if (GUILayout.Button("Go", JumpToButtonWidth)) {
                ExperimentEvents.JumpToTrial(trials.IndexOf(trial));
            }
            EditorGUILayout.ColorField(GUIContent.none, color, false, false, false, CompleteIndicatorWidth);
            EditorGUILayout.TextArea(trial.Data.AsString());
            EditorGUILayout.EndHorizontal();
        }


    }


    //void OnGUI() {

    //    GUILayout.Label("Trials:", EditorStyles.boldLabel);
    //    EditorGUILayout.BeginVertical();

    //    EditorGUILayout.BeginHorizontal();
    //    string runningText = running ? "Running" : "Ended";
    //    EditorGUILayout.TextArea(runningText,RunningTrialIndicatorWidth);
    //    EditorGUILayout.TextArea("Jump to", JumpToButtonWidth);
    //    Color doneColor = running == true ? Color.blue : Color.green;
    //    EditorGUILayout.ColorField(GUIContent.none, doneColor, false, false, false, CompleteIndicatorWidth);
    //    EditorGUILayout.TextArea(header);
    //    EditorGUILayout.EndHorizontal();

    //    foreach (Trial trial in trials) {
    //        EditorGUILayout.BeginHorizontal();

    //        bool success = (bool) trial.Data[Config.SuccessColumnName] ;
    //        bool skip = (bool) trial.Data[Config.SkippedColumnName];
    //        Color color = success == true ? Color.green : Color.red;
    //        if (skip) color = Color.yellow;

    //        string trialRunningText = index == trials.IndexOf(trial) ? "Running" : "";

    //        EditorGUILayout.TextArea(trialRunningText, RunningTrialIndicatorWidth);
    //        if (GUILayout.Button("Go", JumpToButtonWidth)) {
    //            ExperimentEvents.JumpToTrial(trials.IndexOf(trial));
    //        }
    //        EditorGUILayout.ColorField(GUIContent.none, color, false, false, false, CompleteIndicatorWidth);
    //        EditorGUILayout.TextArea(trial.Data.AsString());
    //        EditorGUILayout.EndHorizontal();
    //    }

    //    if(!running) EditorGUILayout.TextArea("\nExperiment Complete!");
    //    EditorGUILayout.EndVertical();


    //}



}
