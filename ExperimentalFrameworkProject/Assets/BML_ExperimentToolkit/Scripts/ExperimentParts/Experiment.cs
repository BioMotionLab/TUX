using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using BML_ExperimentToolkit.Scripts.ExperimentParts.SimpleExperimentParts;
using BML_ExperimentToolkit.Scripts.Managers;
using BML_Utilities;
using MyNamespace;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {
    
    public abstract class Experiment : MonoBehaviour, Outputtable {

        public virtual Type TrialType => typeof(SimpleTrial);
        public virtual Type BlockType => typeof(SimpleBlock);

        public ExperimentDesign Design;
        Session                 session;
        OutputManager           outputManager;
        public ConfigDesignFile           ConfigDesignFile;
        public ConfigOptions ConfigOptions;

        [HideInInspector]
        public bool             Running = false;
        [HideInInspector]
        public bool             Ended   = false;

        void Start() {
            if (ConfigDesignFile == null) {
                Debug.LogError("Design Configuration not set up properly, make sure you dragged a configDesign file into your experiment GameObject");
                UnityEditor.EditorApplication.isPlaying = false;
                Application.Quit();
                return;
            }

            if (ConfigOptions == null) {
                Debug.Log("No Trial Config Script Selected.");
            }

            Design = ConfigDesignFile.Factory.ToTable(this, ConfigDesignFile.ShuffleTrialOrder, ConfigDesignFile.NumberOfTimesToRepeatTrials);
            ExperimentEvents.InitExperiment(this);

        }

        void OnEnable() {
            ExperimentEvents.OnStartExperiment += StartExperiment;
            ExperimentEvents.OnTrialUpdated += TrialUpdated;
            ExperimentEvents.OnEndExperiment += EndExperiment;
        }

        void OnDisable() {
            ExperimentEvents.OnStartExperiment -= StartExperiment;
            ExperimentEvents.OnTrialUpdated -= TrialUpdated;
            ExperimentEvents.OnEndExperiment -= EndExperiment;
        }

        void TrialUpdated(List<Trial> trials, int index) {
            OutputUpdated();
        }

        void OutputUpdated() {
            ExperimentEvents.OutputUpdated(this);
        }

        void StartExperiment(Session currentSession) {
            this.session = currentSession;
            Running = true;
            outputManager = new OutputManager(currentSession.OutputFullPath, session.DebugMode);
            StartCoroutine(RunPreExperiment());
        }

        IEnumerator RunPreExperiment() {
            yield return Pre();
            ExperimentEvents.ExperimentStarted();
            BlockSequenceRunner blockRunner = new BlockSequenceRunner(this, Design.Blocks);
            blockRunner.Start();
        }

        IEnumerator RunPostExperiment() {
            yield return Post();


            Running = false;
            Ended = true;
            Design.Disable();
            outputManager.Disable();
        }

        protected virtual IEnumerator Pre() {
            Debug.Log("No pre experiment code defined");
            yield return null;
        }

        protected virtual IEnumerator Post() {
            Debug.Log("No post experiment code defined");
            yield return null;
        }

        void EndExperiment() {
            StartCoroutine(RunPostExperiment());


        }

        public string AsString {
            get {
                StringBuilder sb = new StringBuilder();

                string header = Design.TrialTableHeader;
                sb.AppendLine(header);
                foreach (Block block in Design.Blocks) {
                    foreach (Trial trial in block.Trials) {
                        sb.AppendLine(trial.Data.AsString(separator: Delimiter.Comma, truncate: -1));
                    }
                }

                return sb.ToString();

            }
        }
    }
}