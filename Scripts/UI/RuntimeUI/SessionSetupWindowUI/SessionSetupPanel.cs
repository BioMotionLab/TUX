using System;
using System.Collections.Generic;
using bmlTUX.Scripts.ExperimentParts;
using bmlTUX.Scripts.Managers;
using bmlTUX.Scripts.VariableSystem;
using TMPro;
using UnityEngine;

namespace bmlTUX.Scripts.UI.Runtime {
    public class SessionSetupPanel : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI SessionStatusText = default;

        [SerializeField]
        OutputFilePanel OutputFilePanel = default;

        [SerializeField]
        DesignFilePanel DesignFilePanel = default;

        [SerializeField]
        BlockOrderPanel BlockOrderPanel = default;

        [SerializeField]
        ParticipantVariablePanel ParticipantVariablePanel = default;
        
        public int SelectedBlockOrder => BlockOrderPanel.SelectedBlockOrder;

        [SerializeField]
        ErrorDisplayPanel ErrorDisplayPanel = default;
       
        BlockOrderData blockOrderData;
        Session session;
        ExperimentRunner runner;
        public bool ValidSession;


        public void Init(ExperimentRunner runnerToInit) {
            runner = runnerToInit;
            session = runner.Session;
            
            if (session == null) throw new NullReferenceException("session could not be created");
            
            SessionStatusText.text = "New session successfully created";
            
            List<ParticipantVariable> participantVariables = runner.DesignFile.Factory.Variables.ParticipantVariables;
            ParticipantVariablePanel.ShowParticipantVariables(participantVariables);
            
            
            switch (runner.DesignFile.TrialTableGeneration) {
                case TrialTableGenerationMode.OnTheFly:
                    ShowBlockOrderSettings();
                    break;
                case TrialTableGenerationMode.PreGenerated:
                    DesignFilePanel.Show();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            SetSelectedBlockOrder(session.BlockOrderChosenIndex + 1);
            
            OutputFilePanel.SetupFields(session.OutputFile);


        }
        
        void ShowBlockOrderSettings() {
            blockOrderData = new BlockOrderData(runner.ExperimentDesign);

            if (!blockOrderData.SelectionRequired) {
                BlockOrderPanel.Deactivate();
                return;
            }
            BlockOrderPanel.GetBlockOrderFromPopup(runner.ExperimentDesign.BlockPermutationsStrings);
            BlockOrderPanel.SetTitle(blockOrderData.BlockOrderText);
            BlockOrderPanel.Activate();
        }


        void SetSelectedBlockOrder(int selectedBlockOrder) {
            BlockOrderPanel.SelectedBlockOrder = selectedBlockOrder;
        }

        public Session GetSession() {

            OutputFile outputFile = OutputFilePanel.GetOutputFile();
            
            List<InputValidator> validators = new List<InputValidator>();
            validators.Add(new OutputFileValidationResult(outputFile));
            validators.Add(new ParticipantVariableValuesValidator(ParticipantVariablePanel));

            int blockOrderChosen = 0;
            string designFilePath = "";
            switch (runner.DesignFile.TrialTableGeneration) {
                case TrialTableGenerationMode.OnTheFly:
                    if (blockOrderData.SelectionRequired) {
                        blockOrderChosen = BlockOrderPanel.SelectedBlockOrder - 1; // subtract 1 because added first one in.
                    }
                    else {
                        blockOrderChosen = blockOrderData.DefaultBlockOrderIndex;
                    }
                    validators.Add(new BlockOrderValidationResult(blockOrderData, BlockOrderPanel));
                    break;
                case TrialTableGenerationMode.PreGenerated:
                    designFilePath = DesignFilePanel.DesignFilePath;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            ValidSession = true;
            string errorString = "";
            foreach (InputValidator validator in validators) {
                if (!validator.Valid) {
                    ValidSession = false;
                    foreach (string error in validator.Errors) {
                        errorString += error + "\n";
                    }
                }
            }

            if (!ValidSession) {

                ErrorDisplayPanel.Display(errorString);
            }
            else {
                gameObject.SetActive(false);
                session.BlockOrderChosenIndex = blockOrderChosen;
                session.OutputFile = outputFile;
                session.SelectedDesignFilePath = designFilePath;
                
            }
            return session;
        }

    }
}
