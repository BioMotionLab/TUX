using System;
using System.Collections.Generic;
using System.IO;
using BML_TUX.Scripts.ExperimentParts;
using BML_TUX.Scripts.Managers;
using BML_TUX.Scripts.UI.Editor;
using BML_TUX.Scripts.VariableSystem;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace BML_TUX.Scripts.UI.Runtime {
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
        RectTransform DesignFileLoadPanel = default;
        
        [SerializeField]
        TextMeshProUGUI ErrorText = default;

        [SerializeField]
        RectTransform ErrorPanel = default;
        
        [SerializeField]
        RectTransform BlockOrderSettingsPanel = default;

        [SerializeField]
        TMP_InputField DesignFilePath = default;
        
        [SerializeField]
        TMP_Dropdown BlockOrderSelector = default;

        [SerializeField]
        TextMeshProUGUI BlockOrderTitle = default;

        [SerializeField]
        TextMeshProUGUI PreviewText = default;
        
        const string SelectText = "Choose...";

        readonly List<ParticipantVariableEntry> participantVariableEntries = new List<ParticipantVariableEntry>();
        BlockOrderData blockOrderData;

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
            
            switch (runner.DesignFile.TrialTableGeneration) {
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

            OutputFileName.text = session.OutputFileName;
            OutputFolder.text = session.OutputFolder;
            BlockOrderSelector.value = session.BlockOrderChosenIndex + 1;
            
            DesignPreviewer previewer = new DesignPreviewer(runner.DesignFile);
            PreviewText.text = previewer.ShowRuntimePreview();
        }

        void ShowDesignFileLoadSettings() {
            DesignFileLoadPanel.gameObject.SetActive(true);
        }

        void ShowParticipantVariables() {

            List<ParticipantVariable> participantVariables = runner.DesignFile.Factory.Variables.ParticipantVariables;

            foreach (ParticipantVariable participantVariable in participantVariables) {
                
                ParticipantVariableEntry newParticipantVariableEntry = 
                    Instantiate(ParticipantVariableEntryPrefab, ParticipantVariablesPanel);
                participantVariableEntries.Add(newParticipantVariableEntry);
                newParticipantVariableEntry.Display(participantVariable);
            }
        }

        void GetBlockOrderFromPopup() {
            List<string> blockPermutations = runner.ExperimentDesign.BlockPermutationsStrings;
            BlockOrderSelector.options.Clear();
            blockPermutations.Insert(0, SelectText);
            foreach (string order in blockPermutations) {
                BlockOrderSelector.options.Add(new TMP_Dropdown.OptionData() {text = order});
            }
        }
        
        void ShowBlockOrderSettings() {
            blockOrderData = new BlockOrderData(runner.ExperimentDesign);

            if (!blockOrderData.SelectionRequired) {
                BlockOrderSettingsPanel.gameObject.SetActive(false);
                return;
            }


            GetBlockOrderFromPopup();
            BlockOrderTitle.text = blockOrderData.BlockOrderText;
            BlockOrderSettingsPanel.gameObject.SetActive(true);
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
            
            switch (runner.DesignFile.TrialTableGeneration) {
                case TrialTableGenerationMode.OnTheFly:
                    List<IndependentVariable> blockVariables = runner.DesignFile.Variables.BlockVariables;
                    if (blockOrderData.SelectionRequired) {
                        session.BlockOrderChosenIndex = BlockOrderSelector.value - 1; // subtract 1 because added first one in.
                    }
                    else {
                        session.BlockOrderChosenIndex = blockOrderData.DefaultBlockOrderIndex;
                    }
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
            else {
                gameObject.SetActive(false);
                ExperimentEvents.StartRunningExperiment(session);
            }

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
            if (!blockOrderData.SelectionRequired) return;
            
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
