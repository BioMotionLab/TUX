using System.Collections;
using System.Collections.Generic;
using System.Data;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public class ExperimentWindow : EditorWindow {
    static readonly GUILayoutOption CompleteIndicatorWidth = GUILayout.Width(20);
    static readonly GUILayoutOption JumpToButtonWidth = GUILayout.Width(40);
    static readonly GUILayoutOption RunningTrialIndicatorWidth = GUILayout.Width(60);
    static readonly GUILayoutOption IndentWidth = GUILayout.Width(40);

    int currentBlockIndex = -1;
    int currentTrialIndex = -1;

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
        ExperimentEvents.OnBlockUpdated += BlockCompleted;
        ExperimentEvents.OnTrialUpdated += TrialCompleted;
        ExperimentEvents.OnStartExperiment += ExperimentStarted;
        ExperimentEvents.OnTrialHasStarted += TrialStarted;
    }

    

    void OnDisable() {
        ExperimentEvents.OnInitExperiment -= InitWindow;
        ExperimentEvents.OnBlockUpdated -= BlockCompleted;
        ExperimentEvents.OnTrialUpdated -= TrialCompleted;
        ExperimentEvents.OnStartExperiment -= ExperimentStarted;
        ExperimentEvents.OnTrialHasStarted -= TrialStarted;
    }

    void TrialStarted(Trial trial, int index) {
        currentTrialIndex = index;
        RepaintWindow();
    }

    void TrialCompleted(List<Trial> trials, int index) {
        RepaintWindow();
    }

    void ExperimentStarted() {
        currentBlockIndex = 0;
        currentTrialIndex = 0;
        RepaintWindow();
    }

    void InitWindow(Experiment experiment) {
        currentBlockIndex = -1;
        currentTrialIndex = -1;
        this.experiment = experiment;
        initialized = true;
        RepaintWindow();
    }

    void RepaintWindow() {
        Debug.Log("repainting window");
        Repaint();
    }
    void BlockCompleted(List<Block> blocks, int index) {
        currentBlockIndex = index+1;
        RepaintWindow();
    }

    void Update() {
        if (!Application.isPlaying) initialized = false;
    }

    void OnGUI() {
        EditorGUILayout.BeginVertical();

        
        if (!Application.isPlaying) {
            EditorGUILayout.HelpBox("Not in PlayMode", MessageType.Error);
            return;
        }

        if (!initialized) {
            EditorGUILayout.HelpBox($"Experiment and Config File not properly initialized.", MessageType.Error);
            return;
        }

        if (!experiment.Running) {
            if (GUILayout.Button("Start Experiment")) {
                ExperimentEvents.StartExperiment();
            }
        }
        else {
            string runningText = experiment.Running ? "Running" : "Not Running";
            EditorGUILayout.TextArea($"Experiment is {runningText}.");
        }

        string EndedText = !experiment.Ended ? "Not Finished" : "Ended";
        EditorGUILayout.TextArea($"Experiment is {EndedText}.");

        
        
        EditorGUILayout.Space();

        ShowBlockTable();

        EditorGUILayout.Space();

        ShowTrialTables();

        EditorGUILayout.Space();
    }

    void ShowTrialTables() {

        EditorGUILayout.LabelField("Trial Tables:", EditorStyles.boldLabel);

        List<Block> blocks = experiment.table.blocks;
        foreach (var block in blocks) {
            int blockIndex = blocks.IndexOf(block);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", IndentWidth);
            EditorGUILayout.LabelField($"Block: {block.Identity}");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", IndentWidth);
            EditorGUILayout.LabelField("", RunningTrialIndicatorWidth);
            EditorGUILayout.LabelField("", JumpToButtonWidth);
            EditorGUILayout.LabelField("", CompleteIndicatorWidth);
            EditorGUILayout.TextArea(block.table.HeaderAsString());
            EditorGUILayout.EndHorizontal();


            DataTable trialTable = block.table;
            foreach (DataRow trialRow in trialTable.Rows) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", IndentWidth);

                int indexOfRow = trialTable.Rows.IndexOf(trialRow);

                if (currentTrialIndex == indexOfRow && currentBlockIndex == blockIndex) {
                    EditorGUILayout.LabelField("Running", RunningTrialIndicatorWidth);
                }
                else {
                    EditorGUILayout.LabelField("", RunningTrialIndicatorWidth);
                }

                
                if (blockIndex == currentBlockIndex) {
                    // can't jump between blocks
                    if (GUILayout.Button("Go", JumpToButtonWidth)) {
                        ExperimentEvents.JumpToTrial(indexOfRow);
                    }
                }
                else {
                    EditorGUILayout.LabelField("", JumpToButtonWidth);
                }

                Trial trial = block.Trials[indexOfRow];

                Color color = trial.Success ? Color.green : Color.red;
                color = trial.Skipped ? Color.yellow : color;
                EditorGUILayout.ColorField(GUIContent.none, color, false, false, false, CompleteIndicatorWidth);


                EditorGUILayout.TextArea(trialRow.AsString());

                EditorGUILayout.EndHorizontal();
            }
        }

    }

    void ShowBlockTable() {


        EditorGUILayout.LabelField("Block Table:", EditorStyles.boldLabel);


        
        //BLOCK DISPLAY
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("", IndentWidth);
        EditorGUILayout.LabelField("", IndentWidth);
        EditorGUILayout.LabelField("", RunningTrialIndicatorWidth);
        EditorGUILayout.LabelField("", JumpToButtonWidth);
        EditorGUILayout.LabelField("", CompleteIndicatorWidth);
        EditorGUILayout.TextArea(experiment.table.blockTable.HeaderAsString());
        EditorGUILayout.EndHorizontal();

        DataTable blockTable = experiment.table.blockTable;
        foreach (DataRow blockRow in blockTable.Rows) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", IndentWidth);
            EditorGUILayout.LabelField("", IndentWidth);
            int indexOfBlock = blockTable.Rows.IndexOf(blockRow);
            //Debug.Log($"block index = {indexOfBlock}, current = {currentBlockIndex}");

            if (currentBlockIndex == indexOfBlock) {
                EditorGUILayout.LabelField("Running", RunningTrialIndicatorWidth);
            }
            else {
                EditorGUILayout.LabelField("", RunningTrialIndicatorWidth);
            }
            
            
            
            

            

            Block block = experiment.table.blocks[indexOfBlock];
            Color color = block.Complete ? Color.green : Color.red;
            EditorGUILayout.ColorField(GUIContent.none, color, false, false, false, CompleteIndicatorWidth);

            EditorGUILayout.TextArea(blockRow.AsString());

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

    //        string trialRunningText = currentBlockIndex == trials.IndexOf(trial) ? "Running" : "";

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
