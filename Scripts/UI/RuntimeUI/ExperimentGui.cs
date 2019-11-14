using System.Data;
using bmlTUX.Scripts.ExperimentParts;
using bmlTUX.Scripts.Managers;
using bmlTUX.Scripts.UI.RuntimeUI.TableViewer;
using bmlTUX.Scripts.VariableSystem;
using JetBrains.Annotations;
using UnityEngine;

namespace bmlTUX.Scripts.UI.Runtime {
    public class ExperimentGui : MonoBehaviour {
        
        ExperimentRunner runner;

        [SerializeField]
        SessionSetupPanel SessionSetupPanel = default;
        
        [SerializeField]
        TableViewer TableDisplay = default;

        [SerializeField]
        ExperimentRunnerPanel ExperimentRunnerPanel = default;

        [SerializeField]
        RectTransform ExperimentStartPanel = default;
        
        DesignPreviewer previewer;
        FileLocationSettings fileLocationSettings;
        public void RegisterExperiment(ExperimentRunner experimentRunner) {
            ExperimentEvents.OnInitExperiment += Init;
            runner = experimentRunner;
            fileLocationSettings = runner.DesignFile.FileLocationSettings;
        }

        public void OnDisable() {
            ExperimentEvents.OnInitExperiment -= Init;
        }

        void Init(ExperimentRunner unused) {
            
            SessionSetupPanel.Init(runner);

            previewer = new DesignPreviewer(runner.DesignFile);
            DisplayPreview();
        }

        void DisplayPreview() {
            DataTable preview = previewer.GetPreview(SessionSetupPanel.SelectedBlockOrder);
            TableDisplay.Display(preview);
        }

        [PublicAPI]
        public void ReRandomizePreview() {
            previewer.ReRandomizeTable();
            DisplayPreview();
        }
        

        [PublicAPI]
        public void StartExperiment() {
            Session session = SessionSetupPanel.GetSession();
            if (SessionSetupPanel.ValidSession) {
                StartRunningExperiment(session);
            }
        }

        void StartRunningExperiment(Session session) {
            ExperimentEvents.StartRunningExperiment(session);
            ExperimentStartPanel.gameObject.SetActive(false);
            ExperimentRunnerPanel.ShowPanel();
        }

        [PublicAPI]
        public void StartDebugExperiment() {
            if (fileLocationSettings == null) Debug.LogError($"fileLocationSettings null when debug started");
            Session session = new DebugSession(fileLocationSettings);

            foreach (ParticipantVariable variable in runner.DesignFile.Variables.ParticipantVariables) {
                variable.SetValueDefaultValue();
            }
            
            StartRunningExperiment(session);
        }

    }
}
