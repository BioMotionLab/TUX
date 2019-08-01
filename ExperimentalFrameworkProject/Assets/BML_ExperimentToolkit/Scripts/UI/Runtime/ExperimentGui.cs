using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using BML_ExperimentToolkit.Scripts.ExperimentParts;
using BML_ExperimentToolkit.Scripts.Managers;
using BML_ExperimentToolkit.Scripts.UI.Runtime;
using BML_ExperimentToolkit.Scripts.VariableSystem;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.UI {
    public class ExperimentGui : MonoBehaviour {

        Session session;
        ExperimentRunner runner;

        [SerializeField]
        TextMeshProUGUI SessionStatusText = default;

        [SerializeField]
        TMP_InputField OutputFileName = default;
        
        [SerializeField]
        TMP_InputField OutputFolder = default;

        [SerializeField]
        ParticipantVariableEntry ParticipantVariableEntryPrefab = default;

        [SerializeField]
        RectTransform ParticipantVariablesPanel = default;

        [SerializeField]
        TextMeshProUGUI ErrorText = default;

        [SerializeField]
        RectTransform ErrorPanel = default;

        [SerializeField]
        TMP_Dropdown BlockOrderSelector = default;

        [SerializeField]
        TextMeshProUGUI BlockOrderTitle = default;
        
        const string SelectText = "Choose...";
        
        List<ParticipantVariableEntry> participantVariableEntries = new List<ParticipantVariableEntry>();
        public void RegisterExperiment(ExperimentRunner experimentRunner) {
            ExperimentEvents.OnInitExperiment += Init;
            runner = experimentRunner;
        }

        public void OnDisable() {
            ExperimentEvents.OnInitExperiment -= Init;
        }

        void Init(ExperimentRunner experimentRunner) {
            session = experimentRunner.Session;
            session.DebugMode = false;
            if (session == null) throw new NullReferenceException("session null in gui");
            SessionStatusText.text = session != null ? "New session successfully created and linked to experiment" : "no session detected";

            ShowParticipantVariables();
            ShowBlockOrderSettings();
        }

        void ShowParticipantVariables() {

            List<Variable> variables = runner.VariableConfigFile.Factory.AllVariables;

            foreach (Variable variable in variables) {
                
                if (variable is ParticipantVariable participantVariable) {
                    
                    ParticipantVariableEntry newParticipantVariableEntry = Instantiate(ParticipantVariableEntryPrefab, ParticipantVariablesPanel);
                    
                    participantVariableEntries.Add(newParticipantVariableEntry);
                    
                    newParticipantVariableEntry.Display(participantVariable);
                }
            }
        }
        
        
        void ShowBlockOrderSettings() {
            
            if (!runner.Design.HasBlocks) {
                session.OrderChosenIndex = 0;
                session.BlockChosen = true;
                BlockOrderSelector.gameObject.SetActive(false);
                BlockOrderTitle.text = "No block variables configured";
                return;
                
            }
            
            try {
                List<string> blockPermutations = runner.Design.BlockPermutationsStrings;
                if (blockPermutations.Count == 1) {
                    session.BlockChosen = true;
                    session.OrderChosenIndex = 0;
                }
                else {
                    BlockOrderSelector.options.Clear();
                    blockPermutations.Insert(0, SelectText);
                    foreach (string order in blockPermutations) {
                        BlockOrderSelector.options.Add(new TMP_Dropdown.OptionData() {text = order});
                    }
                }
            }
            catch (TooManyPermutationsException e) {
                Console.WriteLine(e);
                throw;
            }
        }

        [PublicAPI]
        public void StartExperiment() {

            if (!InputsValid()) return;

            string folder = GetOutputFolderPath();

            session.OutputFileName = OutputFileName.text;
            session.OutputFolder = folder;

            session.OrderChosenIndex = BlockOrderSelector.value-1; // subtract 1 because added first one in.
            session.BlockChosen = true;
            
            gameObject.SetActive(false);
            ExperimentEvents.StartRunningExperiment(session);
        }

        string GetOutputFolderPath() {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            folder = Path.Combine(folder, OutputFolder.text);
            return folder;
        }

        [PublicAPI]
        public void HideErrorPanel() {
            ErrorPanel.gameObject.SetActive(false);
        }

        bool InputsValid() {

            bool isValid = true;
            string errorLog = string.Empty;
            
            ValidateDirectoryName(OutputFolder.text, ref errorLog, ref isValid);
            ValidateDirectoryName(OutputFileName.text, ref errorLog, ref isValid);
            ValidateFileDoesNotExist(ref errorLog, ref isValid);
            
            ValidateParticipantVariableValues(ref errorLog, ref isValid);

            ValidateBlockOrderChosen(ref errorLog, ref isValid);
            
            if (!isValid) {
                ErrorText.text = errorLog;
                ErrorPanel.gameObject.SetActive(true);
            }

            return isValid;
        }

        void ValidateFileDoesNotExist(ref string errorLog, ref bool isValid) {
            string fullFilePath = Path.Combine(GetOutputFolderPath(), OutputFileName.text + ".csv");
            Debug.Log(fullFilePath);
            if (!File.Exists(fullFilePath)) return;
            
            string errorString = $"Output File Already Exists @ {fullFilePath}";
            errorLog = LogErrorIntoString(errorLog, errorString);
            isValid = false;
        }

        void ValidateBlockOrderChosen(ref string errorLog, ref bool isValid) {
            string selectedText = BlockOrderSelector.options[BlockOrderSelector.value].text;

            if (selectedText != SelectText) return;
            string errorString = $"Need to select block order value";
            errorLog = LogErrorIntoString(errorLog, errorString);
            isValid = false;
        }

        void ValidateParticipantVariableValues(ref string errorLog, ref bool isValid) {
            foreach (ParticipantVariableEntry participantVariableEntry in participantVariableEntries) {
                try {
                    participantVariableEntry.ConfirmValue();
                }
                catch (FormatException) {
                    string errorString =
                        $"Input for Variable {participantVariableEntry.Variable.Name} is incorrect format or type";
                    errorLog = LogErrorIntoString(errorLog, errorString);
                    isValid = false;
                }
                catch (NoValueSelectedException) {
                    string errorString =
                        $"No value selected for {participantVariableEntry.Variable.Name}";
                    errorLog = LogErrorIntoString(errorLog, errorString);
                    isValid = false;
                }
            }
        }

        void ValidateDirectoryName(string fileOrDirectoryName, ref string errorLog, ref bool isValid ) {
            if (string.IsNullOrEmpty(fileOrDirectoryName)) {
                string errorString = $"Output Folder name not set or too short. {fileOrDirectoryName}";
                errorLog = LogErrorIntoString(errorLog, errorString);
                isValid = false;
                return;
            }

            if (!IsAllNumbersAndLetters(fileOrDirectoryName)) {
                string errorString = $"Output Folder name contains invalid characters. {fileOrDirectoryName}";
                errorLog = LogErrorIntoString(errorLog, errorString);
                isValid = false;
            }
        }

        static string LogErrorIntoString(string errorLog, string errorString) {
            Debug.LogWarning(errorString);
            errorLog += errorString + "\n";
            return errorLog;
        }

        bool IsAllNumbersAndLetters(string text) {
            return Regex.IsMatch(text, @"^[a-zA-Z0-9_]+$");
        }
        
    }
}
