using System;
using System.Data;
using BML_TUX.Scripts.Managers;
using BML_TUX.Scripts.UI.Runtime;
using BML_TUX.Scripts.VariableSystem;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace BML_TUX.Scripts.ExperimentParts {
    
    
    /// <summary>
    /// The main entry point for a user's experiments. Links the experiment framework to the unity scene.
    /// You must create a subclass that inherits from this class and drag it into your scene to make an experiment.
    /// Your custom ExperimentRunner should contain references to objects in your scene that
    /// need to be controlled by the framework.
    /// </summary>
    public abstract class ExperimentRunner : MonoBehaviour {

        
        [FormerlySerializedAs("variableConfigurationFile")]
        [Header("Required:")]
        [Tooltip("Create a DesignFile from asset menu and drag here")]
        [SerializeField]
        // ReSharper disable once InconsistentNaming
        ExperimentDesignFile ExperimentDesignFile = default;

        [Header("Optional:")]
        [SerializeField]
        [Tooltip("References to your custom scripts")]
        // ReSharper disable once InconsistentNaming
        public ScriptReferences scriptReferences = new ScriptReferences();
        
        // ReSharper disable once ConvertToAutoProperty
        public ExperimentDesignFile DesignFile {
            get => ExperimentDesignFile;
            set => ExperimentDesignFile = value;
        }

        public ExperimentDesign ExperimentDesign;
        public RunnableDesign RunnableDesign;
        
        OutputManager outputManager;
        Experiment experiment;
        ExperimentGui gui;
        
        
        /// <summary>
        /// Type of custom Trial used.
        /// </summary>
        [PublicAPI]
        public Type TrialType;

        /// <summary>
        /// Type of custom Block used.
        /// /// </summary>
        [PublicAPI]
        public Type BlockType; 

        /// <summary>
        /// Type of custom Experiment used.
        /// </summary>
        [PublicAPI]
        public Type ExperimentType;

        [HideInInspector]
        public bool Ended;

        [HideInInspector]
        public bool Running;

        [HideInInspector]
        public bool WindowOpen = false;
        
        public Session Session { get; private set; }

        void OnEnable() {
            ExperimentEvents.OnStartRunningExperiment += StartRunningRunningExperiment;
            ExperimentEvents.OnEndExperiment += EndExperiment;

        }
        
        void Start() {

            TrialType = scriptReferences.TrialType;
            BlockType = scriptReferences.BlockType;
            ExperimentType = scriptReferences.ExperimentType;
            
            #if UNITY_EDITOR
            ExperimentEvents.CheckMainWindowIsOpen(this);
            #endif

            //check if configurationFile file is loaded
            if (DesignFile == null) {
                Debug.LogError("Config file not set up properly, make sure you dragged a configuration file into your Runner GameObject in the inspector");
                ExitProgram();
                return;
            }
            DesignFile.Validate();

            Session = Session.LoadSessionData(DesignFile.FileLocationSettings);
            if (Session == null) {
                throw new NullReferenceException("Session null and not created properly");
            }

            switch (DesignFile.TrialTableGeneration) {
                case TrialTableGenerationMode.OnTheFly:
                    ExperimentDesign = ExperimentDesign.CreateFrom(DesignFile);
                    if (ExperimentDesign == null) {
                        throw new NullReferenceException("ExperimentDesign null");
                    }
                    break;
                    
                case TrialTableGenerationMode.PreGenerated:
                    //RunnableDesign created later
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!WindowOpen) {
                gui = Instantiate(DesignFile.GuiSettings.GuiPrefab);
                gui.gameObject.SetActive(true);
                gui.RegisterExperiment(this);
            }
            
            ExperimentEvents.InitExperiment(this);
        }

        static void ExitProgram() {
            #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
            #endif
            Application.Quit();
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
            switch (DesignFile.TrialTableGeneration) {
                case TrialTableGenerationMode.OnTheFly: {
                    DataTable finalDesignTable = ExperimentDesign.GetFinalExperimentTable(currentSession.BlockOrderChosenIndex);
                    RunnableDesign = new RunnableDesign(this, finalDesignTable, DesignFile);
                    break;
                }
                case TrialTableGenerationMode.PreGenerated:
                    string selectedDesignFilePath = currentSession.SelectedDesignFilePath;
                    if (string.IsNullOrEmpty(selectedDesignFilePath)) throw new NullReferenceException("Trying to load custom design file, but none given");
                    RunnableDesign = RunnableDesign.CreateFromFile(this, currentSession.SelectedDesignFilePath,
                                                           DesignFile);
                    break;
                default:
                    throw new NotImplementedException();
            }
            
            if (RunnableDesign == null)
            {
                throw new NullReferenceException("No Runnable design");
            }
            
            Running = true;
            outputManager = new OutputManager(currentSession.OutputFile);

            experiment = (Experiment)Activator.CreateInstance(ExperimentType, this, RunnableDesign);

            if (experiment == null) {
                throw new NullReferenceException("Experiment object instance could not be created");
            }
            
            ExperimentEvents.ExperimentStarted();

            StartCoroutine(DesignFile.ControlSettings.Run());
            
            ExperimentEvents.StartPart(experiment);
            
        }

        void EndExperiment() {
            Running = false;
            Ended = true;
        }

    }
}