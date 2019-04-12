using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using BML_ExperimentToolkit.Scripts.ExperimentParts;
using BML_ExperimentToolkit.Scripts.Managers;
using BML_Utilities;
using BML_Utilities.Extensions;
using UnityEditor;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.UI.Editor {

    public class ExperimentWindow : EditorWindow {

        static readonly GUILayoutOption CompleteIndicatorWidth = GUILayout.Width(20);
        static readonly GUILayoutOption JumpToButtonWidth = GUILayout.Width(40);
        static readonly GUILayoutOption RunningTrialIndicatorWidth = GUILayout.Width(70);
        static readonly GUILayoutOption IndentWidth = GUILayout.Width(40);
        static readonly GUILayoutOption SmallLabelWidth = GUILayout.Width(60);
        static readonly GUILayoutOption LabelWidth = GUILayout.Width(120);
        static readonly GUILayoutOption BlockIdentityWidth = GUILayout.Width(300);

        int currentBlockIndex = -1;
        int currentTrialIndex = -1;
        bool initialized;
        ExperimentRunner runner;
        bool autoName;
        Vector2 scrollPos = Vector2.zero;
        Session session;

        // Add menu item to open this window
        [MenuItem(MenuNames.BmlMainMenu + "Runner Runner Window")]
        public static void ShowWindow() {
            //Show existing window instance. If one doesn't exist, make one.
            GetWindow(typeof(ExperimentWindow), false, "Runner Runner Window");
        }

        void OnEnable() {
            //add listeners for events
            ExperimentEvents.OnInitExperiment += InitWindow;
            ExperimentEvents.OnBlockUpdated += BlockCompleted;
            ExperimentEvents.OnTrialUpdated += TrialCompleted;
            ExperimentEvents.OnExperimentStarted += ExperimentStarted;
            ExperimentEvents.OnTrialHasStarted += TrialStarted;

        }
        
        void OnDisable() {
            //remove listeners for events to prevent memory leaks
            ExperimentEvents.OnInitExperiment -= InitWindow;
            ExperimentEvents.OnBlockUpdated -= BlockCompleted;
            ExperimentEvents.OnTrialUpdated -= TrialCompleted;
            ExperimentEvents.OnExperimentStarted -= ExperimentStarted;
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

        void InitWindow(ExperimentRunner runnerToInit) {
            session = Session.LoadSessionData();
            currentBlockIndex = -1;
            currentTrialIndex = -1;
            runner = runnerToInit;
            initialized = true;
            Repaint();
        }


        void BlockCompleted(List<Block> blocks, int index) {
            currentBlockIndex = index + 1;
            Repaint();
        }

        void Update() {
            if (!Application.isPlaying) initialized = false;
            Repaint();
        }



        void OnGUI() {
            
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, 
                false, 
                false, 
                GUILayout.ExpandHeight(true));
            EditorGUILayout.BeginVertical();

            //Check if in play mode
            if (!Application.isPlaying) {
                EditorGUILayout
                    .HelpBox("Cannot display Runner design when Unity is not in PlayMode, " +
                             "\n\nPress play in Editor to Set up the Runner",
                             MessageType.Error);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndScrollView();
                return;
            }

            //Ensure config file initialized properly.
            if (!initialized) {
                EditorGUILayout.HelpBox("ConfigDesignFile not properly initialized. " +
                                        "\n\nMake sure you have created a config file from the menu and populated it with variables" +
                                        "\n\nAlso make sure the config file is dragged into the Runner GameObject inspector in your scene.",
                                        MessageType.Error);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndScrollView();
                return;
            }

            //Session PromptSettings
            if (!ShowSessionSettings()) {
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndScrollView();
                return;
            }

            //Block Order
            if (!ShowBlockOrderSettings()) {
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndScrollView();
                return;
            }


            //Runner controls
            ShowExperimentControls();


            //Blocks
            if (runner.Design.HasBlocks) {
                ShowBlockTable(runner.Design.OrderedBlockTable);
            }
            

            //Trials
            ShowTrialTables();
            


            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }


        /// <summary>
        /// Displays session settings
        /// </summary>
        /// <returns></returns>
        bool ShowSessionSettings() {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Session settings:", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Debug Mode:", LabelWidth);
            session.DebugMode = EditorGUILayout.Toggle(session.DebugMode);
            EditorGUILayout.EndHorizontal();

            //break out if debug mode
            if (session.DebugMode) {
                EditorGUILayout.EndVertical();
                EditorGUI.indentLevel--;
                return true;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Participant ID:", LabelWidth);
            session.ParticipantId = EditorGUILayout.TextField(session.ParticipantId);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Output File Path:", LabelWidth);
            if (GUILayout.Button("Choose Folder", LabelWidth)) {
                session.OutputFolder = EditorUtility.OpenFolderPanel("Choose Output Folder", "", "");
            }
            GUILayout.Box(session.OutputFolder);
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            autoName = EditorGUILayout.Toggle("Output file: Auto Name?", autoName);
            if (autoName) {
                session.OutputFileName =
                    DateTime.Now.ToString("yyyy-MM-dd_Thh-mm") + "_Participant-" + session.ParticipantId;
                EditorGUILayout.LabelField("Name: ", SmallLabelWidth);
                GUILayout.Box(session.OutputFileName);
            }
            else {
                EditorGUILayout.LabelField("Name: ", SmallLabelWidth);
                session.OutputFileName = EditorGUILayout.TextField(session.OutputFileName);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Full output path:", LabelWidth);
            GUILayout.Box(session.OutputFullPath);
            EditorGUILayout.EndHorizontal();

            if (File.Exists(session.OutputFullPath)) {
                EditorGUILayout.HelpBox("File already exists!", MessageType.Error);
                return false;
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
            return true;
        }

        /// <summary>
        /// Choose block order
        /// </summary>
        /// <returns></returns>
        bool ShowBlockOrderSettings() {
            if (!session.BlockChosen) {

                if (!runner.Design.HasBlocks) {
                    Debug.Log($"Runner has no blocks");
                    session.OrderChosenIndex = 0;
                    session.BlockChosen = true;
                    return true;
                }

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Block Order PromptSettings:", EditorStyles.boldLabel);

                List<string> blockPermutations = runner.Design.BlockPermutationsStrings;
                
                session.OrderChosenIndex = EditorGUILayout.Popup(session.OrderChosenIndex, blockPermutations.ToArray());
                DataTable selectedOrderTable = runner.Design.GetBlockOrderTable(session.OrderChosenIndex);
                
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Chosen block order:");

                ShowBlockTable(selectedOrderTable, orderSelected: false);

                if (GUILayout.Button("Confirm Order")) {
                    session.BlockChosen = true;
                }

                EditorGUILayout.EndVertical();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Display the Runner controls
        /// </summary>
        void ShowExperimentControls() {

            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Runner Controls:", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            if (!runner.Running && !runner.Ended) {
                if (GUILayout.Button("Start Runner")) {
                    ExperimentEvents.StartExperiment(session);
                }

            }
            else {
                EditorGUILayout.BeginHorizontal();
                string runningText = runner.Running ? "Running" : "Not Running";
                EditorGUILayout.LabelField($"Runner {runningText}.");
                int currentTrial = runner.Design.BlockCount * currentBlockIndex + currentTrialIndex + 1;
                if (currentTrial > runner.Design.TotalTrials) currentTrial = runner.Design.TotalTrials;
                EditorGUILayout.LabelField("Running Trial: " +
                                           $"{currentTrial}" +
                                           "/" +
                                           $"{runner.Design.TotalTrials} ");
                EditorGUILayout.EndHorizontal();
            }

            string endedText = !runner.Ended ? "Not Finished" : "Ended";
            EditorGUILayout.LabelField($"Runner is {endedText}.");

            EditorGUILayout.Space();
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Displays table for the blocks of the Runner
        /// </summary>
        /// <param name="blockTable"></param>
        /// <param name="orderSelected"></param>
        void ShowBlockTable(DataTable blockTable, bool orderSelected = true) {
            
            if (!runner.Design.HasBlocks) return;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Blocks:", EditorStyles.boldLabel);

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

                string runningText = currentBlockIndex == indexOfBlock ? "Running" : "";
                EditorGUILayout.LabelField(runningText, RunningTrialIndicatorWidth);

                if (orderSelected) {

                    Block block = runner.Design.Blocks[indexOfBlock];
                    Color color = block.Complete ? Color.green : Color.red;
                    EditorGUILayout.ColorField(GUIContent.none, color, false, false, false, CompleteIndicatorWidth);
                }
                else {
                    EditorGUILayout.LabelField("", CompleteIndicatorWidth);
                }

                EditorGUILayout.TextArea(blockRow.AsString());

                EditorGUILayout.EndHorizontal();

            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Displays tables of all trials
        /// </summary>
        void ShowTrialTables() {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Trials:", EditorStyles.boldLabel);

            foreach (var block in runner.Design.Blocks) {
                int blockIndex = runner.Design.Blocks.IndexOf(block);

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);


                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", IndentWidth);
                EditorGUILayout.TextArea($"Block Values: {block.Identity}", BlockIdentityWidth);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", IndentWidth);
                EditorGUILayout.LabelField("", RunningTrialIndicatorWidth);
                EditorGUILayout.LabelField("", JumpToButtonWidth);
                EditorGUILayout.LabelField("", CompleteIndicatorWidth);
                EditorGUILayout.TextArea(block.TrialTable.HeaderAsString());
                EditorGUILayout.EndHorizontal();


                for (int indexOfRow = 0; indexOfRow < block.TrialTable.Rows.Count; indexOfRow++) {
                    DataRow trialRow = block.TrialTable.Rows[indexOfRow];

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("", IndentWidth);

                    if (currentTrialIndex == indexOfRow && currentBlockIndex == blockIndex) {
                        EditorGUILayout.LabelField("Running", RunningTrialIndicatorWidth);
                    }
                    else {
                        EditorGUILayout.LabelField("", RunningTrialIndicatorWidth);
                    }
                    
                    if (blockIndex == currentBlockIndex) {
                        // can't jump between blocks, only allow jumping with block.
                        if (GUILayout.Button("Go", JumpToButtonWidth)) {
                            ExperimentEvents.JumpToTrial(indexOfRow);
                        }
                    }
                    else {
                        EditorGUILayout.LabelField("", JumpToButtonWidth);
                    }

                    Trial trial = block.Trials[indexOfRow];

                    Color color = trial.CompletedSuccessfully ? Color.green : Color.red;
                    color = trial.Skipped ? Color.yellow : color;
                    EditorGUILayout.ColorField(GUIContent.none, color, false, false, false, CompleteIndicatorWidth);
                    
                    EditorGUILayout.TextArea(trialRow.AsString());

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();

            }
            EditorGUILayout.EndVertical();
        }
    }
}