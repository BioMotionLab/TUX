using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using BML_ExperimentToolkit.Scripts.ExperimentParts;
using BML_ExperimentToolkit.Scripts.Managers;
using BML_ExperimentToolkit.Scripts.VariableSystem;
using BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes;
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
            //add listeners for events
            ExperimentEvents.OnInitExperiment += InitWindow;
            ExperimentEvents.OnBlockUpdated += BlockCompleted;
            ExperimentEvents.OnTrialUpdated += TrialCompleted;
            ExperimentEvents.OnExperimentStarted += ExperimentStarted;
            ExperimentEvents.OnTrialHasStarted += TrialStarted;
            ExperimentEvents.OnCheckMainWindowIsOpen += CheckWindowOpen;
        }
        
        void OnDisable() {
            //remove listeners for events to prevent memory leaks
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

        void InitWindow(ExperimentRunner runner) {
            this.runner = runner;
            session = runner.Session;

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

            //Check if in play mode
            if (!Application.isPlaying) {
                EditorGUILayout
                    .HelpBox("Runner display needs to be in PlayMode, " +
                             "\n\nPress play in Editor",
                             MessageType.Warning);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndScrollView();
                return;
            }

            //Ensure config file initialized properly.
            if (!initialized) {
                EditorGUILayout.HelpBox("VariableConfigFile not properly initialized. " +
                                        "\n\nMake sure you have created a config file from the menu and populated it with variables" +
                                        "\n\nAlso make sure the config file is dragged into the Runner GameObject inspector in your scene.",
                                        MessageType.Error);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndScrollView();
                return;
            }

            if (!started) {
                ShowSessionSettings();
            }
            else {
                ShowExperimentControls();
                
                //Blocks
                if (runner.Design.HasBlocks) {
                    ShowBlockTable(runner.ExperimentDesign.OrderedBlockTable);
                }

                //NumberOfTrials
                ShowTrialTables();
            }

            
            
            
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }


        /// <summary>
        /// Displays Session settings
        /// </summary>
        /// <returns></returns>
        void ShowSessionSettings() {
            
            EditorGUILayout.LabelField("session properly linked to experiment");

            if (started || debugMode) {
                Debug.Log("didn't pass check");
                return;
            }
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Session settings:", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            
            if (GUILayout.Button("Start in debug mode")) {
                StartInDebugMode();
            }
            
            ShowOuputFolderInput();
            ShowOutputFileNameInput();
            
            if (!isValidFilePath) {
                EditorGUILayout.HelpBox(fileErrorLog, MessageType.Error);
            }
            
            ShowParticipantVariables();
            
            ShowBlockOrderSettings();
            
            ShowStartButton();
            
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

        }

        void ShowStartButton() {
            if (GUILayout.Button("Start Runner")) {
                StartExperiment();
            }
        }

        void StartExperiment() {
            
            session.OutputFolder = outputFolder;
            session.OutputFileName = fileName;

            fileErrorLog = string.Empty;
            session.ValidateFilePath(ref fileErrorLog, ref isValidFilePath);
            
            if (!isValidFilePath) {
                return;
            }
           
            started = true;
            ExperimentEvents.StartRunningExperiment(session);
        }

        void StartInDebugMode() {
            debugMode = true;
            session.DebugMode = true;
     
            started = true;
            ExperimentEvents.StartRunningExperiment(session);
        }
        
        

        void ShowOutputFileNameInput() {
            EditorGUILayout.LabelField("File Name: ", SmallLabelWidth);
            fileName = EditorGUILayout.TextField(fileName);
        }

        void ShowOuputFolderInput() {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Output Folder:", LabelWidth);
            if (GUILayout.Button("Choose", LabelWidth)) {
                outputFolder = EditorUtility.OpenFolderPanel("Choose Output Folder", "", "");
            }

            GUILayout.Box(outputFolder);
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
                    
                    switch (participantVariable.DataType) {
                        case SupportedDataTypes.Int:
                            ParticipantVariableInt intVariable = (ParticipantVariableInt) participantVariable;
                            intVariable.Value = EditorGUILayout.IntField(intVariable.Value);
                            break;
                        case SupportedDataTypes.Float:
                            ParticipantVariableFloat floatVariable = (ParticipantVariableFloat)participantVariable;
                            floatVariable.Value = EditorGUILayout.FloatField(floatVariable.Value);
                            break;
                        case SupportedDataTypes.String:
                            ParticipantVariableString stringVariable = (ParticipantVariableString)participantVariable;
                            stringVariable.Value = EditorGUILayout.TextField(stringVariable.Value);
                            break;
                        case SupportedDataTypes.Bool:
                            ParticipantVariableBool boolVariable = (ParticipantVariableBool)participantVariable;
                            boolVariable.Value = EditorGUILayout.Toggle(boolVariable.Value);
                            break;
                        case SupportedDataTypes.GameObject:
                        case SupportedDataTypes.Vector3:
                        case SupportedDataTypes.CustomDataType_NotYetImplemented:
                        case SupportedDataTypes.ChooseType:
                            throw new NotImplementedException();
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            
            EditorGUILayout.EndVertical();
        }


        /// <summary>
        /// Choose block order
        /// </summary>
        /// <returns></returns>
        void ShowBlockOrderSettings() {
            if (!runner.Design.HasBlocks) {
                return;
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Choose Block Order:", EditorStyles.boldLabel);

            try {
                List<string> blockPermutations = runner.ExperimentDesign.BlockPermutationsStrings;
                if (blockPermutations.Count == 1) {
                }
                else {
                    OrderChosenIndex = EditorGUILayout.Popup(OrderChosenIndex, blockPermutations.ToArray());
                }
            }
            catch (TooManyPermutationsException e) {
                Console.WriteLine(e);
                throw;
            }
            
            
            DataTable selectedOrderTable = runner.ExperimentDesign.GetBlockOrderTable(OrderChosenIndex);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Chosen block order:");

            ShowBlockTable(selectedOrderTable, false);
            
            EditorGUILayout.EndVertical();
            
        }

        public int OrderChosenIndex;

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
                int currentTrial = (runner.Design.TotalTrials/runner.Design.BlockCount) * (currentBlockIndex) + currentTrialIndex + 1;
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
            EditorGUILayout.LabelField("Trial Tables:", EditorStyles.boldLabel);

            foreach (Block block in runner.Design.Blocks) {
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

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
    }
}