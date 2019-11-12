using System;
using System.Collections.Generic;
using System.Data;
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

        [SerializeField] FileLocationSettings FileLocationSettings = default;

        public const string SelectText = "Choose...";

        readonly List<ParticipantVariableEntry> participantVariableEntries = new List<ParticipantVariableEntry>();
        BlockOrderData blockOrderData;

        public TableViewer TableDisplay;
        DesignPreviewer previewer;

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

            OutputFileName.text = session.OutputFile.outputFileName;
            OutputFolder.text = session.OutputFile.outputFolder;
            
            int selectedBlockOrder = session.BlockOrderChosenIndex + 1;
            BlockOrderSelector.value = selectedBlockOrder;

            previewer = new DesignPreviewer(runner.DesignFile);
            DataTable preview = previewer.GetPreview(selectedBlockOrder);
            TableDisplay.Display(preview);
        }

        public void ReRandomizePreview() {
            previewer.ReRandomizeTable();
            DataTable preview = previewer.GetPreview(BlockOrderSelector.value);
            TableDisplay.Display(preview);
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
            
            List<InputValidator> validators = new List<InputValidator>();
            
            OutputFile outputFile = new OutputFile(GetOutputFolderPath(),
                OutputFileName.text);
            
            
            validators.Add(new OutputFileValidationResult(outputFile));
            
            validators.Add(new ParticipantVariableValuesValidator(participantVariableEntries));

            int blockOrderChosen = 0;
            string designFilePath = "";
            switch (runner.DesignFile.TrialTableGeneration) {
                case TrialTableGenerationMode.OnTheFly:
                    List<IndependentVariable> blockVariables = runner.DesignFile.Variables.BlockVariables;
                    if (blockOrderData.SelectionRequired) {
                        blockOrderChosen = BlockOrderSelector.value - 1; // subtract 1 because added first one in.
                    }
                    else {
                        blockOrderChosen = blockOrderData.DefaultBlockOrderIndex;
                    }
                    validators.Add(new BlockOrderValidationResult(blockOrderData, BlockOrderSelector));
                    break;
                case TrialTableGenerationMode.PreGenerated:
                    designFilePath = DesignFilePath.text;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            bool isValid = true;
            string errorString = "";
            foreach (InputValidator validator in validators) {
                if (!validator.Valid) {
                    isValid = false;
                    foreach (string error in validator.Errors) {
                        errorString += error + "\n";
                    }
                }
            }

            if (!isValid) {
                
                ErrorText.text = errorString;
                ErrorPanel.gameObject.SetActive(true);
            }
            else {
                gameObject.SetActive(false);

                session.BlockOrderChosenIndex = blockOrderChosen;
                session.OutputFile = outputFile;
                session.SelectedDesignFilePath = designFilePath;
                
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
        

        static string LogErrorIntoString(string errorLog, string errorString) {
            Debug.LogWarning(errorString);
            errorLog += errorString + "\n";
            return errorLog;
        }
        
        
        
        
    }

    public class BlockOrderValidationResult : InputValidator {
        
        public BlockOrderValidationResult(BlockOrderData blockOrderData, TMP_Dropdown blockOrderSelector ) {
            if (!blockOrderData.SelectionRequired) return;
            Errors = new List<string>();
            string selectedText = blockOrderSelector.options[blockOrderSelector.value].text;
            if (selectedText != ExperimentGui.SelectText) return;
            Errors.Add($"Need to select block order value");
            Valid = false;
        }

        public List<string> Errors { get; }
        public bool Valid { get; }
    }

    public interface InputValidator {
        List<string> Errors { get; }
        bool Valid { get; }
    }

    public class ParticipantVariableValuesValidator : InputValidator {
        
        public List<string> Errors { get; }
        public bool Valid { get; }
        public ParticipantVariableValuesValidator( List<ParticipantVariableEntry> participantVariableEntries) {
            Errors = new List<string>();
            
            foreach (ParticipantVariableEntry participantVariableEntry in participantVariableEntries) {
                try {
                    participantVariableEntry.ConfirmValue();
                }
                catch (FormatException) {
                    Errors.Add($"Input for Variable {participantVariableEntry.Variable.Name} is incorrect format or type");
                    Valid = false;
                }
                catch (NoValueSelectedException) {
                    Errors.Add($"No value selected for {participantVariableEntry.Variable.Name}");
                    Valid = false;
                }
            }
        }

   
    }
}
