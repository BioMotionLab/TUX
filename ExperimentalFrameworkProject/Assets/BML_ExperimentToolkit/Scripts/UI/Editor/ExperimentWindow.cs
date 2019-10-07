using System.Collections.Generic;
using System.Data;
using BML_ExperimentToolkit.Scripts.ExperimentParts;
using BML_ExperimentToolkit.Scripts.Managers;
using BML_ExperimentToolkit.Scripts.Settings;
using BML_ExperimentToolkit.Scripts.VariableSystem;
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
        static readonly GUILayoutOption LabelWidth = GUILayout.Width(120);
        static readonly GUILayoutOption BlockIdentityWidth = GUILayout.Width(300);

        int currentBlockIndex = -1;
        int currentTrialIndex = -1;
        bool initialized;
        ExperimentRunner runner;
        bool autoName;
        Vector2 scrollPos = Vector2.zero;
        Session session;
        public int OrderChosenIndex;
        DesignPreviewer previewer;
        string trialTableFilePath;
        
        
        /// <summary>
        /// Add menu item to open this window
        /// </summary>
        [MenuItem(MenuNames.BmlMainMenu + "Experiment Runner Window")]
        public static void ShowWindow() {
            //Show existing window instance. If one doesn't exist, make one.
            GetWindow(typeof(ExperimentWindow), false, "Experiment Runner Window");
        }

        static ExperimentWindow instance;
        bool debugMode = false;
        string outputFolder;
        string fileName;
        bool started = false;
        string fileErrorLog;
        bool isValidFilePath = true;
        static bool IsOpen => instance != null;

        void OnEnable() {
            instance = this;
            ExperimentEvents.OnInitExperiment += InitWindow;
            ExperimentEvents.OnBlockUpdated += BlockCompleted;
            ExperimentEvents.OnTrialUpdated += TrialCompleted;
            ExperimentEvents.OnExperimentStarted += ExperimentStarted;
            ExperimentEvents.OnTrialHasStarted += TrialStarted;
            ExperimentEvents.OnCheckMainWindowIsOpen += CheckWindowOpen;
        }
        
        void OnDisable() {
            ExperimentEvents.OnInitExperiment -= InitWindow;
            ExperimentEvents.OnBlockUpdated -= BlockCompleted;
            ExperimentEvents.OnTrialUpdated -= TrialCompleted;
            ExperimentEvents.OnExperimentStarted -= ExperimentStarted;
            ExperimentEvents.OnTrialHasStarted -= TrialStarted;
            ExperimentEvents.OnCheckMainWindowIsOpen -= CheckWindowOpen;
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
            runner = runnerToInit;
            session = runnerToInit.Session;
            previewer = new DesignPreviewer(runner.VariableConfigFile);
            OrderChosenIndex = 0;
            currentBlockIndex = -1;
            currentTrialIndex = -1;

            isValidFilePath = true;
            fileErrorLog = string.Empty;
            started = false;
            debugMode = false;
            
            initialized = true;
            Repaint();
        }

        static void CheckWindowOpen(ExperimentRunner runnerToInit) {
            if (IsOpen) {
                runnerToInit.WindowOpen = true;
            }
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
            
            if (!IsPlayMode()) return;
            if (!ConfigFileGoodStatus()) return;
            
            if (!started) {
                ShowSessionSettings();
            }
            else {
                ShowExperimentControls();
                ShowTrialTables();
            }

            FinalizeUiElements();
        }

        static void FinalizeUiElements() {
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        bool ConfigFileGoodStatus() {
            if (initialized) return true;
            
            EditorGUILayout.HelpBox("VariableConfigFile not properly initialized. " +
                                    "\n\nMake sure you have created a configuration file from the menu and populated it with variables" +
                                    "\n\nAlso make sure the configurationFile file is dragged into the Runner GameObject inspector in your scene.",
                                    MessageType.Error);
            FinalizeUiElements();
            return false;
        }

        static bool IsPlayMode() {
            if (Application.isPlaying) return true;
            
            EditorGUILayout.HelpBox("Runner display needs to be in PlayMode,\n\n" +
                                    "Press play in Editor", 
                                    MessageType.Warning);
            FinalizeUiElements();
            return false;
        }


        /// <summary>
        /// Displays Session settings
        /// </summary>
        /// <returns></returns>
        void ShowSessionSettings() {
            
            if (started || debugMode) return;
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Session settings:", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            
            EditorGUILayout.LabelField("session properly linked to experiment");
            
            ShowStartInDebugModeButton();
            ShowOutputFileSettings();
            ShowParticipantVariables();
            
            if (runner.VariableConfigFile.TrialTableGeneration == TrialTableGenerationMode.PreGenerated) 
                ShowTrialTableLoadInput();
            else {
                previewer.ShowPreview();
                OrderChosenIndex = previewer.SelectedBlockOrderIndex;
            }
            
            ShowStartButton();
            
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        void ShowStartInDebugModeButton() {
            if (GUILayout.Button("Start in debug mode")) {
                StartInDebugMode();
            }
        }

        void ShowOutputFileSettings() {
            EditorGUILayout.Space();
            ShowOutputFolderInput();
            ShowOutputFileNameInput();
            EditorGUILayout.Space();
            if (!isValidFilePath) EditorGUILayout.HelpBox(fileErrorLog, MessageType.Error);
        }

        void ShowStartButton() {
            if (GUILayout.Button("Start Runner")) {
                StartExperiment();
            }
        }
        
        void StartExperiment() {
            session.OutputFolder = outputFolder;
            session.OutputFileName = fileName;
            session.BlockOrderChosenIndex = OrderChosenIndex;

            fileErrorLog = string.Empty;
            session.ValidateFilePath(ref fileErrorLog, ref isValidFilePath);
            
            if (!isValidFilePath) return;
            
            started = true;
            ExperimentEvents.StartRunningExperiment(session);
        }

        void StartInDebugMode() {
            debugMode = true;
            session.DebugMode = true;
     
            started = true;
            ExperimentEvents.StartRunningExperiment(session);
        }
        
        void ShowTrialTableLoadInput() {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.LabelField("Pre-Generated Trial Table File:");
            EditorGUILayout.LabelField(session.SelectedDesignFilePath);
            if (GUILayout.Button("Choose File")) {
                session.SelectedDesignFilePath = EditorUtility.OpenFilePanel("Choose Pre Generated Trial Table", "", "csv");
            }
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        void ShowOutputFileNameInput() {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("File Name: ", LabelWidth);
            fileName = EditorGUILayout.TextField(fileName);
            EditorGUILayout.EndHorizontal();
        }

        void ShowOutputFolderInput() {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Output Folder:", LabelWidth);
            EditorGUILayout.LabelField(outputFolder);
            if (GUILayout.Button("Choose")) {
                             outputFolder = EditorUtility.OpenFolderPanel("Choose Output Folder", "", "");
            }
            EditorGUILayout.EndHorizontal();
        }

        void ShowParticipantVariables() {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Fill In Participant Variables:", EditorStyles.boldLabel);
            
            foreach (ParticipantVariable participantVariable in runner.VariableConfigFile.Factory.Variables.ParticipantVariables) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(participantVariable.Name + ":", LabelWidth);
                if (participantVariable.ValuesAreConstrained) {
                    string[] possibleValues = participantVariable.PossibleValuesStringArray;
                    participantVariable.SelectedValue =
                            EditorGUILayout.Popup(participantVariable.SelectedValue, possibleValues);
                }
                else {
                    participantVariable.AddValueFieldInEditor();
                }

                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndVertical();
        }

       

        /// <summary>
        /// Display the Runner controls
        /// </summary>
        void ShowExperimentControls() {
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Runner Controls:", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            if (runner.Running || runner.Ended) {
                EditorGUILayout.BeginHorizontal();
                string runningText = runner.Running ? "Running" : "Not Running";
                EditorGUILayout.LabelField($"Runner {runningText}.");
                int currentTrial = (runner.RunnableDesign.TotalTrials/runner.RunnableDesign.BlockCount) * (currentBlockIndex) + currentTrialIndex + 1;
                if (currentTrial > runner.RunnableDesign.TotalTrials) currentTrial = runner.RunnableDesign.TotalTrials;
                EditorGUILayout.LabelField("Running Trial: " +
                                           $"{currentTrial}" +
                                           "/" +
                                           $"{runner.RunnableDesign.TotalTrials} ");
                EditorGUILayout.EndHorizontal();
            }

            string endedText = !runner.Ended ? "Not Finished" : "Ended";
            EditorGUILayout.LabelField($"Runner is {endedText}.");

            EditorGUILayout.Space();
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Displays tables of all trials
        /// </summary>
        void ShowTrialTables() {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Trial Tables:", EditorStyles.boldLabel);

            foreach (Block block in runner.RunnableDesign.Blocks) {
                int blockIndex = runner.RunnableDesign.Blocks.IndexOf(block);

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", IndentWidth);
                EditorGUILayout.TextArea($"Block: {block.Identity}", BlockIdentityWidth);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", IndentWidth);
                EditorGUILayout.LabelField("", RunningTrialIndicatorWidth);
                EditorGUILayout.LabelField("", JumpToButtonWidth);
                EditorGUILayout.LabelField("", CompleteIndicatorWidth);
                EditorGUILayout.TextArea(block.TrialTable.HeaderAsString());
                EditorGUILayout.EndHorizontal();
                
                for (int indexOfRow = 0; indexOfRow < block.TrialTable.Rows.Count; indexOfRow++) {
                    AddTrialRow(block, indexOfRow, blockIndex);
                }

                EditorGUILayout.EndVertical();
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        void AddTrialRow(Block block, int indexOfRow, int blockIndex) {
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
    }
}