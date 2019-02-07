using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ExperimentWindow : EditorWindow {
    static readonly GUILayoutOption CompleteIndicatorWidth = GUILayout.Width(20);
    static readonly GUILayoutOption JumpToButtonWidth = GUILayout.Width(40);
    static readonly GUILayoutOption RunningTrialIndicatorWidth = GUILayout.Width(60);
    static readonly GUILayoutOption IndentWidth = GUILayout.Width(40);
    static readonly GUILayoutOption SmallLabelWidth = GUILayout.Width(60);
    static readonly GUILayoutOption LabelWidth = GUILayout.Width(120);

    int currentBlockIndex = -1;
    int currentTrialIndex = -1;
    int OrderChosenIndex = 0;
    bool initialized = false;
    Experiment experiment;
    bool blockChosen;
    string participantID;
    string outputFilePath;
    string outputFileName;
    bool autoName;

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
        Repaint();
    }

    void TrialCompleted(List<Trial> trials, int index) {
        Repaint();
    }

    void ExperimentStarted() {
        currentBlockIndex = 0;
        currentTrialIndex = 0;
        Repaint();
    }

    void InitWindow(Experiment experiment) {
        currentBlockIndex = -1;
        currentTrialIndex = -1;
        this.experiment = experiment;
        initialized = true;
        blockChosen = false;
        OrderChosenIndex = 0;
        Repaint();
    }

    
    void BlockCompleted(List<Block> blocks, int index) {
        currentBlockIndex = index+1;
        Repaint();
    }

    void Update() {
        if (!Application.isPlaying) initialized = false;
        Repaint();
    }

    void OnGUI() {
        EditorGUILayout.BeginVertical();

        
        if (!Application.isPlaying) {
            EditorGUILayout.HelpBox("Cannot display experiment when Unity is not in PlayMode, Press play in Editor to start experiment Setup", MessageType.Error);
            return;
        }

        if (!initialized) {
            EditorGUILayout.HelpBox($"Experiment and Config File not properly initialized.", MessageType.Error);
            return;
        }

        if (!ShowSessionSettings()) return;

        EditorGUILayout.Space();

        if (!ShowBlockOrderSettings()) return;

        EditorGUILayout.Space();


        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Experiment Controls:", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        if (!experiment.Running) {
            if (GUILayout.Button("Start Experiment")) {
                ExperimentEvents.StartExperiment();
            }
        }
        else {
            EditorGUILayout.BeginHorizontal();
            string runningText = experiment.Running ? "Running" : "Not Running";
            EditorGUILayout.LabelField($"Experiment {runningText}.");
            EditorGUILayout.LabelField($"Running Trial: " +
                                       $"{experiment.Design.BlockCount*currentBlockIndex+currentTrialIndex + 1}" +
                                       $"/" +
                                       $"{experiment.Design.TotalTrials} ");
            EditorGUILayout.EndHorizontal();
        }

        string endedText = !experiment.Ended ? "Not Finished" : "Ended";
        EditorGUILayout.LabelField($"Experiment is {endedText}.");

        EditorGUILayout.Space();
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();



        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Blocks:", EditorStyles.boldLabel);
        ShowBlockTable(experiment.Design.OrderedBlockTable);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Trials:", EditorStyles.boldLabel);
        ShowTrialTables();
        EditorGUILayout.EndVertical();


        EditorGUILayout.Space();
    }

    bool ShowBlockOrderSettings() {
        if (!blockChosen) {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Block Order Settings:", EditorStyles.boldLabel);

            var blockPermutations = experiment.Design.BlockPermutationsStrings;
            OrderChosenIndex = EditorGUILayout.Popup(OrderChosenIndex, blockPermutations.ToArray());
            var selectedOrderTable = experiment.Design.GetBlockOrderTable(OrderChosenIndex);


            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Chosen block order:");

            ShowBlockTable(selectedOrderTable, orderSelected: false);

            if (GUILayout.Button("Confirm Order")) {
                Debug.Log($"Block order chosen: {OrderChosenIndex}");
                experiment.Design.BlockOrderSelected(OrderChosenIndex);
                blockChosen = true;
            }

            EditorGUILayout.EndVertical();
            return false;
        }

        return true;
    }

    bool ShowSessionSettings() {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Session settings:", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        //Session Settings
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Participant ID:", LabelWidth);
        participantID = EditorGUILayout.TextField(participantID);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Output File Path:", LabelWidth);
        if (GUILayout.Button("Choose Folder", LabelWidth)) {
            outputFilePath = EditorUtility.OpenFolderPanel("Choose Output Folder", "", "");
        }

        GUILayout.Box(outputFilePath);
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        autoName = EditorGUILayout.Toggle("Output file: Auto Name?", autoName);
        if (autoName) {
            outputFileName = participantID + " " + DateTime.Now.ToString("yyyy_MM_dd hh_mm");
            EditorGUILayout.LabelField($"Name: ", SmallLabelWidth);
            GUILayout.Box(outputFileName);
        }
        else {
            EditorGUILayout.LabelField("Name: ", SmallLabelWidth);
            outputFileName = EditorGUILayout.TextField(outputFileName);
        }

        EditorGUILayout.EndHorizontal();
        string extension = ".csv";
        string fullPath = Path.Combine(outputFilePath, outputFileName, extension);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Full output path:", LabelWidth);
        GUILayout.Box(fullPath);
        EditorGUILayout.EndHorizontal();

        if (File.Exists(fullPath)) {
            EditorGUILayout.HelpBox("File already exists!", MessageType.Error);
            return false;
        }

        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
        return true;
    }

    void ShowTrialTables() {

        foreach (var block in experiment.Design.Blocks) {
            int blockIndex = experiment.Design.Blocks.IndexOf(block);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", IndentWidth);
            EditorGUILayout.LabelField($"Block Values: {block.Identity}");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", IndentWidth);
            EditorGUILayout.LabelField("", RunningTrialIndicatorWidth);
            EditorGUILayout.LabelField("", JumpToButtonWidth);
            EditorGUILayout.LabelField("", CompleteIndicatorWidth);
            EditorGUILayout.TextArea(block.trialTable.HeaderAsString());
            EditorGUILayout.EndHorizontal();


            for (int indexOfRow = 0; indexOfRow < block.trialTable.Rows.Count; indexOfRow++) {
                DataRow trialRow = block.trialTable.Rows[indexOfRow];
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", IndentWidth);

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

                Color color = trial.CompletedSuccesssfully ? Color.green : Color.red;
                color = trial.Skipped ? Color.yellow : color;
                EditorGUILayout.ColorField(GUIContent.none, color, false, false, false, CompleteIndicatorWidth);


                EditorGUILayout.TextArea(trialRow.AsString());

                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

        }

    }

    void ShowBlockTable(DataTable blockTable, bool orderSelected = true) {

        //BLOCK DISPLAY
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("", IndentWidth);
        EditorGUILayout.LabelField("", RunningTrialIndicatorWidth);
        EditorGUILayout.LabelField("", JumpToButtonWidth);
        EditorGUILayout.LabelField("", CompleteIndicatorWidth);
        EditorGUILayout.TextArea(blockTable.HeaderAsString());
        EditorGUILayout.EndHorizontal();

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

            if (orderSelected) {

                Block block = experiment.Design.Blocks[indexOfBlock];
                Color color = block.Complete ? Color.green : Color.red;
                EditorGUILayout.ColorField(GUIContent.none, color, false, false, false, CompleteIndicatorWidth);
            }
            else {
                EditorGUILayout.LabelField("", CompleteIndicatorWidth);
            }

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
