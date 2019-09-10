using System;
using System.Data;
using BML_ExperimentToolkit.Scripts.ExperimentParts.SimpleExperimentParts;
using BML_ExperimentToolkit.Scripts.Managers;
using BML_ExperimentToolkit.Scripts.UI.Runtime;
using BML_ExperimentToolkit.Scripts.VariableSystem;
using BML_Utilities.Extensions;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {
    
    
    /// <summary>
    /// The main entry point for a user's experiments. Links the experiment framework to the unity scene.
    /// You must create a subclass that inherits from this class and drag it into your scene to make an experiment.
    /// Your custom ExperimentRunner should contain references to objects in your scene that
    /// need to be controlled by the framework.
    /// </summary>
    public abstract class ExperimentRunner : MonoBehaviour {

        
        [Header("Required:")]
        [Tooltip("Create a VariableConfigFile from asset menu and drag here")]
        [SerializeField]
        // ReSharper disable once InconsistentNaming
        VariableConfigurationFile variableConfigurationFile = default;

        [Header("Optional:")]
        [SerializeField]
        [Tooltip("References to your custom scripts")]
        // ReSharper disable once InconsistentNaming
        ScriptReferences scriptReferences = new ScriptReferences();
        
        // ReSharper disable once ConvertToAutoProperty
        public VariableConfigurationFile VariableConfigFile => variableConfigurationFile;
        
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
            if (VariableConfigFile == null) {
                Debug.LogError("Config file not set up properly, make sure you dragged a configuration file into your Runner GameObject in the inspector");
                ExitProgram();
                return;
            }
            VariableConfigFile.Validate();

            Session = Session.LoadSessionData();
            if (Session == null) {
                throw new NullReferenceException("Session null and not created properly");
            }

            switch (VariableConfigFile.TrialTableGeneration) {
                case TrialTableGenerationMode.OnTheFly:
                    ExperimentDesign = ExperimentDesign.CreateFrom(VariableConfigFile);
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
                gui = Instantiate(VariableConfigFile.GuiSettings.GuiPrefab);
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
            switch (VariableConfigFile.TrialTableGeneration) {
                case TrialTableGenerationMode.OnTheFly: {
                    DataTable finalDesignTable = ExperimentDesign.GetFinalExperimentTable(currentSession.BlockOrderChosenIndex);
                    RunnableDesign = new RunnableDesign(this, finalDesignTable, VariableConfigFile);
                    break;
                }
                case TrialTableGenerationMode.PreGenerated:
                    string selectedDesignFilePath = currentSession.SelectedDesignFilePath;
                    if (string.IsNullOrEmpty(selectedDesignFilePath)) throw new NullReferenceException("Trying to load custom design file, but none given");
                    RunnableDesign = RunnableDesign.CreateFromFile(this, currentSession.SelectedDesignFilePath,
                                                           VariableConfigFile);
                    break;
                default:
                    throw new NotImplementedException();
            }
            
            if (RunnableDesign == null)
            {
                throw new NullReferenceException("No Runnable design");
            }
            
            Running = true;
            outputManager = new OutputManager(currentSession.OutputFullPath);

            experiment = (Experiment)Activator.CreateInstance(ExperimentType, this, RunnableDesign);

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

    [Serializable]
    public class ScriptReferences {
        
        [SerializeField]
        [Tooltip("Script file that inherits from Trial class")]
        MonoScript DragTrialScriptHere = default;
        
        [SerializeField]
        [Tooltip("Script file that inherits from Block class")]
        MonoScript DragBlockScriptHere = default;
        
        [SerializeField]
        [Tooltip("Script file that inherits from Experiment class")]
        MonoScript DragExperimentScriptHere = default;


        public Type TrialType => GetScriptTypeFromInspector<Trial>(DragTrialScriptHere, true);
        public Type BlockType => GetScriptTypeFromInspector<Block>(DragBlockScriptHere, true);
        public Type ExperimentType => GetScriptTypeFromInspector<Experiment>(DragExperimentScriptHere, true);

        Type GetScriptTypeFromInspector<T>(MonoScript monoScript, bool optional = false) where T : ExperimentPart {

            string typeName = typeof(T).LastPartOfName();

            if (monoScript == null) {
                if (optional) return GetDefaultExperimentPart<T>();
                throw new NullReferenceException($"{typeName} Script null. Create custom {typeName} script and drag into inspector");
            }
            
            Type returnType = monoScript.GetClass();
            if (!returnType.IsSubclassOf(typeof(T)))
                throw new NullReferenceException($"{typeName} Script that was dragged in is not subclass of {typeName} Class");
            
            Debug.Log($"Successfully linked with {returnType.LastPartOfName()} script");
            return returnType;
        }

        Type GetDefaultExperimentPart<T>() where T : ExperimentPart {
            Type type;
            if (typeof(T).IsEquivalentTo(typeof(Trial))) type = typeof(SimpleTrial);
            else if (typeof(T).IsEquivalentTo(typeof(Block))) type =  typeof(SimpleBlock);
            else if (typeof(T).IsEquivalentTo(typeof(Experiment))) type =  typeof(SimpleExperiment);
            else throw new ArgumentOutOfRangeException($"Type {typeof(T).FullName} not recognized");
            Debug.LogWarning($"No Custom class defined for {typeof(T).FullName}, reverting to default {type.FullName}");
            return type;
        }
    }
}