using System;
using BML_ExperimentToolkit.Scripts.ExperimentParts.SimpleExperimentParts;
using BML_ExperimentToolkit.Scripts.Managers;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {
    public abstract class ExperimentRunner : MonoBehaviour {
        [Header("Required:")]
        public ConfigDesignFile ConfigDesignFile;

        public ExperimentDesign Design;

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
        protected virtual Type ExperimentType => typeof(SimpleExperiment);

        public bool Ended;

        public bool Running;

        public bool inited;
        void Start() {

            //check if config file is loaded
            if (ConfigDesignFile == null) {
                Debug.LogError("Design Configuration not set up properly, make sure you dragged a configDesign file into your Runner GameObject");
                UnityEditor.EditorApplication.isPlaying = false;
                Application.Quit();
                return;
            }


            Design = ConfigDesignFile.Factory.ToTable(this, ConfigDesignFile.ShuffleTrialOrder, ConfigDesignFile.RepeatTrialBlock, ConfigDesignFile.ShuffleDifferentlyForEachBlock);
            if (Design == null) {
                Debug.Log("Design not created properly");
                throw new NullReferenceException("Design null");
            }


            experiment = (Experiment)Activator.CreateInstance(ExperimentType, this, Design);

            ExperimentEvents.InitExperiment(this);
            inited = true;
        }

        void OnEnable() {
            ExperimentEvents.OnStartExperiment += StartExperiment;

        }

        void OnDisable() {
            ExperimentEvents.OnStartExperiment -= StartExperiment;
            Design.Disable();
            outputManager.Disable();
            experiment.Disable();
        
        }

        /// <summary>
        /// Starts the Runner. The Runner does not start automatically, because it waits for an event to start it.
        /// </summary>
        /// <param name="currentSession"></param>
        void StartExperiment(Session currentSession) {
            if (!inited) {
                throw new NullReferenceException("Experiment started before inited");
            }
            
            
            outputManager = new OutputManager(currentSession.OutputFullPath);

            Debug.Log("Starting Runner");
            ExperimentEvents.ExperimentStarted();

            StartCoroutine(ExperimentControls.RunExperimentControls());
            ExperimentEvents.StartPart(experiment);


        }

       
            
    }
}