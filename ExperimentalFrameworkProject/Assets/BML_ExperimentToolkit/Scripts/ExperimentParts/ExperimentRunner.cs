using System;
using System.Data;
using BML_ExperimentToolkit.Scripts.ExperimentParts.SimpleExperimentParts;
using BML_ExperimentToolkit.Scripts.Managers;
using BML_ExperimentToolkit.Scripts.UI;
using BML_ExperimentToolkit.Scripts.UI.Runtime;
using BML_ExperimentToolkit.Scripts.VariableSystem;
using BML_Utilities;
using JetBrains.Annotations;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {
    public abstract class ExperimentRunner : MonoBehaviour {
        [Header("Required:")]
        public VariableConfig VariableConfigFile;

        public ExperimentDesign ExperimentDesign;
        public RunnableDesign Design;
        
        OutputManager outputManager;

        Experiment experiment;

        /// <summary>
        /// Stores the script of the custom Trial used in this Runner.
        /// Override this to customize trial behaviour
        /// </summary>
        [PublicAPI]
        public virtual Type TrialType => typeof(SimpleTrial);

        /// <summary>
        /// Stores the script of the custom Block used in this Runner.
        /// Override this to customize block behaviour
        /// </summary>
        [PublicAPI]
        public virtual Type BlockType => typeof(SimpleBlock);

        /// <summary>
        /// Stores the script of the custom Runner used in this Runner.
        /// Override this to customize Runner behaviour
        /// </summary>
        // ReSharper disable once MemberCanBeProtected.Global
        [PublicAPI]
        public virtual Type ExperimentType => typeof(SimpleExperiment);

        [HideInInspector]
        public bool Ended;

        [HideInInspector]
        public bool Running;

        [HideInInspector]
        public bool WindowOpen = false;

        ExperimentGui gui;

        public Session Session { get; private set; }

        void Start() {

            #if UNITY_EDITOR
            
            ExperimentEvents.CheckMainWindowIsOpen(this);

            #endif

            //check if config file is loaded
            if (VariableConfigFile == null) {
                Debug.LogError("Config file not set up properly, make sure you dragged a configuration file into your Runner GameObject in the inspector");
                ExitProgram();
                return;
            }
            VariableConfigFile.Validate();

            Session = Session.LoadSessionData();
            if (Session == null) {
                throw new NullReferenceException("Session nul and not created properly");
            }

            if (VariableConfigFile.TrialTableGenerationMode == TrialTableGenerationMode.OnTheFly) {
                ExperimentDesign = ExperimentDesign.CreateFrom(VariableConfigFile, this);
            }
            else {
                throw new NotImplementedException();
            }
            
            if (ExperimentDesign == null) {
                throw new NullReferenceException("ExperimentDesign null");
            }
            
            if (!WindowOpen) {
                gui = Instantiate(VariableConfigFile.GuiSettings.GuiPrefab);
                gui.gameObject.SetActive(true);
                gui.RegisterExperiment(this);
            }
            
            ExperimentEvents.InitExperiment(this);
            
            
        }

        static void ExitProgram() {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
            Application.Quit();
        }

        void OnEnable() {
            ExperimentEvents.OnStartRunningExperiment += StartRunningRunningExperiment;
            ExperimentEvents.OnEndExperiment += EndExperiment;

        }

        void OnDisable() {
            ExperimentEvents.OnStartRunningExperiment -= StartRunningRunningExperiment;
            ExperimentEvents.OnEndExperiment -= EndExperiment;
            outputManager?.Disable();
            experiment?.Disable();
        
        }

        /// <summary>
        /// Starts the Runner. The Runner does not start automatically, because it waits for an event to start it.
        /// </summary>
        /// <param name="currentSession"></param>
        void StartRunningRunningExperiment(Session currentSession) {

            if (VariableConfigFile.TrialTableGenerationMode == TrialTableGenerationMode.OnTheFly) {
                DataTable finalDesignTable = ExperimentDesign.GetFinalExperimentTable(currentSession.BlockOrderChosenIndex);
                Design = new RunnableDesign(this, finalDesignTable, VariableConfigFile);
            }
            else {
                throw new NotImplementedException();
            }
            
            if (Design == null)
            {
                throw new NullReferenceException("No Runnable design");
            }
            
            Running = true;
            outputManager = new OutputManager(currentSession.OutputFullPath);

            experiment = (Experiment)Activator.CreateInstance(ExperimentType, this, Design);

            if (experiment == null) {
                throw new NullReferenceException("Experiment object instance could not be created");
            }
            
            ExperimentEvents.ExperimentStarted();

            StartCoroutine(VariableConfigFile.ControlSettings.Run());
            ExperimentEvents.StartPart(experiment);


        }

        void EndExperiment() {
            Running = false;
            Ended = true;
        }

       
            
    }
}