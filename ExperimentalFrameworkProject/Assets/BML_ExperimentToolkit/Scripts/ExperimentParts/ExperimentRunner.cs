using System;
using BML_ExperimentToolkit.Scripts.ExperimentParts.SimpleExperimentParts;
using BML_ExperimentToolkit.Scripts.Managers;
using BML_ExperimentToolkit.Scripts.UI;
using BML_ExperimentToolkit.Scripts.VariableSystem;
using BML_Utilities;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {
    public abstract class ExperimentRunner : MonoBehaviour {
        [Header("Required:")]
        public VariableConfig VariableConfigFile;

        public ExperimentDesign Design;
        
        [SerializeField]
        ExperimentGui gui = default;
        
        OutputManager outputManager;


        Experiment experiment;

        /// <summary>
        /// Stores the script of the custom Trial used in this Runner.
        /// Override this to customize trial behaviour
        /// </summary>
        public virtual Type TrialType => typeof(SimpleTrial);

        /// <summary>
        /// Stores the script of the custom Block used in this Runner.
        /// Override this to customize block behaviour
        /// </summary>
        public virtual Type BlockType => typeof(SimpleBlock);

        /// <summary>
        /// Stores the script of the custom Runner used in this Runner.
        /// Override this to customize Runner behaviour
        /// </summary>
        // ReSharper disable once MemberCanBeProtected.Global
        public virtual Type ExperimentType => typeof(SimpleExperiment);

        [HideInInspector]
        public bool Ended;

        [HideInInspector]
        public bool Running;

        [HideInInspector]
        public bool WindowOpen = false;

        public Session Session { get; private set; }

        void Start() {

            

            //check if config file is loaded
            if (VariableConfigFile == null) {
                Debug.LogError("Config file not set up properly, make sure you dragged a configuration file into your Runner GameObject in the inspector");
                ExitProgram();
                return;
            }
            VariableConfigFile.Validate();
            
            

            Design = VariableConfigFile.Factory.ToTable(this, VariableConfigFile.ShuffleTrialOrder, 
                                                        VariableConfigFile.RepeatTrialsInBlock, 
                                                        VariableConfigFile.ShuffleDifferentlyForEachBlock, 
                                                        VariableConfigFile.RepeatAllBlocks, 
                                                        VariableConfigFile.OrderConfigs);
            if (Design == null) {
                throw new NullReferenceException("Design null");
            }

            experiment = (Experiment)Activator.CreateInstance(ExperimentType, this, Design);

            if (experiment == null) {
                throw new NullReferenceException("Experiment object instance could not be created");
            }
            
            Session = Session.LoadSessionData();
            if (Session == null) {
                throw new NullReferenceException("Session nul and not created properly");
            }
            
            if (gui != null) {
                gui.RegisterExperiment(this);
            }
            
            ExperimentEvents.InitExperiment(this);
            
            
        }

        static void ExitProgram() {
            UnityEditor.EditorApplication.isPlaying = false;
            Application.Quit();
        }

        void OnEnable() {
            ExperimentEvents.OnStartExperiment += StartExperiment;
            ExperimentEvents.OnEndExperiment += EndExperiment;

        }

        void OnDisable() {
            ExperimentEvents.OnStartExperiment -= StartExperiment;
            ExperimentEvents.OnEndExperiment -= EndExperiment;
            Design?.Disable();
            outputManager?.Disable();
            experiment?.Disable();
        
        }

        /// <summary>
        /// Starts the Runner. The Runner does not start automatically, because it waits for an event to start it.
        /// </summary>
        /// <param name="currentSession"></param>
        void StartExperiment(Session currentSession) {

            Running = true;
            outputManager = new OutputManager(currentSession.OutputFullPath);

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