using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using BML_ExperimentToolkit.Scripts.ExperimentParts.SimpleExperimentParts;
using BML_ExperimentToolkit.Scripts.Managers;
using BML_Utilities;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {


    /// <summary>
    /// The Experiment class is the main backbone of the toolkit. This sets up the experiment design, the blocks and trial structure, and manages output
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    /// <seealso cref="BML_ExperimentToolkit.Scripts.Managers.Outputtable" />
    public abstract class Experiment : MonoBehaviour, Outputtable {

        //stores the script of the Trial used in this experiment. This is used to customize trial behaviour
        public virtual Type TrialType => typeof(SimpleTrial);
        //stores the script of the Block used in this experiment. This is used to customize trial behaviour
        public virtual Type BlockType => typeof(SimpleBlock);

        public ExperimentDesign Design;
        Session session;
        OutputManager outputManager;

        [Header("Required:")]
        [Tooltip("Drag the configuration file here")]
        public ConfigDesignFile ConfigDesignFile;
        [Header("Optional:")]
        [Tooltip("Drag the GameObject with attached ConfigOptions script here")]
        public ConfigOptions ConfigOptions;

        [HideInInspector]
        public bool Running;
        [HideInInspector]
        public bool Ended;

        /// <summary>
        /// Called first frame of the experiment
        /// </summary>
        void Start() {

            //check if config file is loaded
            if (ConfigDesignFile == null) {
                Debug.LogError("Design Configuration not set up properly, make sure you dragged a configDesign file into your experiment GameObject");
                UnityEditor.EditorApplication.isPlaying = false;
                Application.Quit();
                return;
            }

            //check if trial config GameObject is loaded
            if (ConfigOptions == null) {
                Debug.LogWarning("No Custom Trial Config Script Used.");
            }

            //set up experiment design
            Design = ConfigDesignFile.Factory.ToTable(this, ConfigDesignFile.ShuffleTrialOrder, ConfigDesignFile.NumberOfTimesToRepeatTrials);

            //initialize the experiment
            ExperimentEvents.InitExperiment(this);

        }

        /// <summary>
        /// Called when experiment is loaded
        /// </summary>
        void OnEnable() {
            //add event listeners
            ExperimentEvents.OnStartExperiment += StartExperiment;
            ExperimentEvents.OnTrialUpdated += TrialUpdated;
            ExperimentEvents.OnEndExperiment += EndExperiment;
        }

        /// <summary>
        /// Called when experiment is over
        /// </summary>
        void OnDisable() {
            //remove event listeners
            ExperimentEvents.OnStartExperiment -= StartExperiment;
            ExperimentEvents.OnTrialUpdated -= TrialUpdated;
            ExperimentEvents.OnEndExperiment -= EndExperiment;
        }

        /// <summary>
        /// Called when a trial gets updated
        /// </summary>
        /// <param name="trials"></param>
        /// <param name="index"></param>
        void TrialUpdated(List<Trial> trials, int index) {
            OutputUpdated();
        }

        /// <summary>
        /// Updates the experiment's output
        /// </summary>
        void OutputUpdated() {
            ExperimentEvents.OutputUpdated(this);
        }

        /// <summary>
        /// Starts the Experiment. The experiment does not start automatically, because it waits for an event to start it.
        /// </summary>
        /// <param name="currentSession"></param>
        void StartExperiment(Session currentSession) {
            Debug.Log("Starting experiment");
            session = currentSession;
            Running = true;
            outputManager = new OutputManager(currentSession.OutputFullPath);
            StartCoroutine(RunPreExperiment());
        }

        /// <summary>
        /// Start running the code that occurs before the main part of the experiment
        /// </summary>
        /// <returns></returns>
        IEnumerator RunPreExperiment() {
            yield return null; // let last frame finish before starting

            yield return Pre();
            ExperimentEvents.ExperimentStarted();
            BlockSequenceRunner blockRunner = new BlockSequenceRunner(this, Design.Blocks);
            blockRunner.Start();
        }

        /// <summary>
        /// Start running the code that occurs after the main part of the experiment
        /// </summary>
        /// <returns></returns>
        IEnumerator RunPostExperiment() {
            yield return null; // let last frame finish before starting
            
            yield return Post();
            
            Running = false;
            Ended = true;
            Design.Disable();
            outputManager.Disable();
        }

        /// <summary>
        /// Code that occurs before the main part of the experiment. This should be overwritten for custom behaviour
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator Pre() {
            //Debug.Log("No pre experiment code defined");
            yield return null;
        }

        /// <summary>
        /// Code that occurs after the main part of the experiment. This should be overwritten for custom behaviour
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator Post() {
            //Debug.Log("No post experiment code defined");
            yield return null;
        }

        /// <summary>
        /// Called when experiment ends
        /// </summary>
        void EndExperiment() {
            StartCoroutine(RunPostExperiment());
        }

        /// <summary>Gets the experiment as string.</summary>
        /// <value>The experiment as a string.</value>
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