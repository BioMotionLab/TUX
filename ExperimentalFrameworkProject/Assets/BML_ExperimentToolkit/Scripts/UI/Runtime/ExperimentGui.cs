using System;
using System.Collections.Generic;
using System.IO;
using BML_ExperimentToolkit.Scripts.ExperimentParts;
using BML_ExperimentToolkit.Scripts.Managers;
using BML_ExperimentToolkit.Scripts.VariableSystem;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.UI.Runtime {
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
        RectTransform DesignFileLoadPanel;
        
        [SerializeField]
        TextMeshProUGUI ErrorText = default;

        [SerializeField]
        RectTransform ErrorPanel = default;

        
        [SerializeField]
        RectTransform BlockOrderSettingsPanel;

        [SerializeField]
        TMP_InputField DesignFilePath = default;
        
        [SerializeField]
        TMP_Dropdown BlockOrderSelector = default;

        [SerializeField]
        TextMeshProUGUI BlockOrderTitle = default;
        
        const string SelectText = "Choose...";

        readonly List<ParticipantVariableEntry> participantVariableEntries = new List<ParticipantVariableEntry>();
        public void RegisterExperiment(ExperimentRunner experimentRunner) {
            ExperimentEvents.OnInitExperiment += Init;
            runner = experimentRunner;
        }

        public void OnDisable() {
            ExperimentEvents.OnInitExperiment -= Init;
        }

        void Init(ExperimentRunner unused) {
            session = runner.Session;
            session.DebugMode = false;
            if (session == null) throw new NullReferenceException("session null in gui");
            SessionStatusText.text = session != null ? 
                "New session successfully created and linked to experiment" : 
                "no session detected";


            switch (runner.VariableConfigFile.TrialTableGenerationMode) {
                case TrialTableGenerationMode.OnTheFly:
                    ShowParticipantVariables();
                    ShowBlockOrderSettings();
                    break;
                case TrialTableGenerationMode.PreGenerated:
                    ShowDesignFileLoadSettings();
                    ShowParticipantVariables();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void ShowDesignFileLoadSettings() {
            DesignFileLoadPanel.gameObject.SetActive(true);
        }

        void ShowParticipantVariables() {

            List<ParticipantVariable> participantVariables = runner.VariableConfigFile.Factory.Variables.ParticipantVariables;

            foreach (ParticipantVariable participantVariable in participantVariables) {
                
                ParticipantVariableEntry newParticipantVariableEntry = 
                    Instantiate(ParticipantVariableEntryPrefab, ParticipantVariablesPanel);
                participantVariableEntries.Add(newParticipantVariableEntry);
                newParticipantVariableEntry.Display(participantVariable);
            }
        }
        
        void ShowBlockOrderSettings() {
            BlockOrderSettingsPanel.gameObject.SetActive(true);
            
            if (!runner.ExperimentDesign.HasBlocks) {
                session.BlockOrderChosenIndex = 0;
                BlockOrderSelector.gameObject.SetActive(false);
                BlockOrderTitle.text = "No block variables configured";
                return;
            }

            try {
                List<string> blockPermutations = runner.ExperimentDesign.BlockPermutationsStrings;
                if (blockPermutations.Count == 1) {
                    session.BlockOrderChosenIndex = 0;
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

           
            
            
            bool isValid = true;
            string errorLog = string.Empty;
            
            session.OutputFileName = OutputFileName.text;
            string folder = GetOutputFolderPath();
            session.OutputFolder = folder;
            session.ValidateFilePath(ref errorLog, ref isValid);
            
            
            ValidateParticipantVariableValues(ref errorLog, ref isValid);
            
            switch (runner.VariableConfigFile.TrialTableGenerationMode) {
                case TrialTableGenerationMode.OnTheFly:
                    session.BlockOrderChosenIndex = BlockOrderSelector.value-1; // subtract 1 because added first one in.
                    ValidateBlockOrderChosen(ref errorLog, ref isValid);
                    break;
                case TrialTableGenerationMode.PreGenerated:
                    session.SelectedDesignFilePath = DesignFilePath.text;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            
            if (!isValid) {
                ErrorText.text = errorLog;
                ErrorPanel.gameObject.SetActive(true);
            }
            
            
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

        void ValidateBlockOrderChosen(ref string errorLog, ref bool isValid) {
            string selectedText = BlockOrderSelector.options[BlockOrderSelector.value].text;
            if (selectedText != SelectText) return;
            string errorString = $"Need to select block order value";
            errorLog = LogErrorIntoString(errorLog, errorString);
            isValid = false;
        }

        static string LogErrorIntoString(string errorLog, string errorString) {
            Debug.LogWarning(errorString);
            errorLog += errorString + "\n";
            return errorLog;
        }

        
        
    }
}
