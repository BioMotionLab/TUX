using System;
using System.IO;
using System.Reflection;
using System.Text;
using bmlTUX.Scripts.ExperimentParts;
using bmlTUX.Scripts.Settings;
using bmlTUX.Scripts.Utilities;
using bmlTUX.Scripts.VariableSystem;
using UnityEditor;
using UnityEngine;

namespace bmlTUX.UI.EditorUI {
    public class ScriptHelperEditorWindow : EditorWindow {
        
        static readonly RunnerScriptTemplateComponent Runner = new RunnerScriptTemplateComponent();
        static readonly TrialScriptTemplateComponent Trial = new TrialScriptTemplateComponent();
        static readonly BlockScriptTemplateComponent Block = new BlockScriptTemplateComponent();
        static readonly ExperimentScriptTemplateComponent Experiment = new ExperimentScriptTemplateComponent();
        
        static readonly DesignExperimentAsset Design = new DesignExperimentAsset();
        
        static string experimentName;
        Vector2 scrollPos;


        /// <summary>
        /// Add menu item to open this window
        /// </summary>
        [MenuItem(MenuNames.BmlMainMenu + "Script Helper Tool")]
        public static void ShowWindow() {
            //Show existing window instance. If one doesn't exist, make one.
            GetWindow(typeof(ScriptHelperEditorWindow), false, "Script Helper Window");
        }


        void OnGUI() {

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.ExpandHeight(true),
                GUILayout.ExpandWidth(true));
            EditorGUILayout.BeginVertical();
            
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("This tool will generate custom scripts for you that you can then edit. " +
                                     "The script names are derived from the name of your experiment. " +
                                     "Make sure the name contains only letters and no spaces", MessageType.None);
            
            EditorGUILayout.Space();
            
            
            EditorGUILayout.LabelField("Experiment Name (only letters, no spaces):");
            experimentName = EditorGUILayout.TextField(experimentName);
            
            
            
            if (!string.IsNullOrEmpty(experimentName)) {

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Automatic set up of all experiment components:", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox("This will create an ExperimentRunner gameObject in the currently open scene, " +
                                        "and will attach the ExperimentRunnerScript to it. " +
                                        "It will automatically create and reference a Design File and all custom scripts for you.\n\n" +
                                        "Creates:\n" +
                                            "\tDesign file asset\n" +
                                            "\tRunner GameObject with attached Runner script\n" +
                                            "\tCustom Trial script\n" +
                                            "\tCustom Block script\n" +
                                            "\tCustom Experiment script\n" +
                                            "\tCustom Settings asset", 
                                        MessageType.None);
                
                if (GUILayout.Button("Automatically set everything up for me!")) {
                    Runner.CreateScript(experimentName);
                    Trial.CreateScript(experimentName);
                    Block.CreateScript(experimentName);
                    Experiment.CreateScript(experimentName);
                    Design.CreateAsset(experimentName);
                }

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                
                EditorGUILayout.LabelField("Manual set up of experiment components:", EditorStyles.boldLabel);

                EditorGUILayout.HelpBox("If you make scripts individually, remember to drag them into the fields in the ExperimentRunner Script in your unity scene", MessageType.None);

                AddCreationButtons();
                
            }
            
            EditorGUILayout.EndVertical();
            GUILayout.EndScrollView();
        }

      

        void AddCreationButtons() {
            if (GUILayout.Button("Create Experiment Design File")) Design.CreateAsset(experimentName);;
            if (GUILayout.Button("Create Custom Runner Script")) Runner.CreateScript(experimentName);
            if (GUILayout.Button("Create Custom Trial Script"))  Trial.CreateScript(experimentName);
            if (GUILayout.Button("Create Custom Block Script")) Block.CreateScript(experimentName);
            if (GUILayout.Button("Create Custom Experiment Script")) Experiment.CreateScript(experimentName);
            if (GUILayout.Button("Create Experiment Settings File"))
                new SettingsExperimentAsset().CreateAsset(experimentName);
        }
        
        
        


        [UnityEditor.Callbacks.DidReloadScripts]
        static void ScriptReloaded() {

            AssetDatabase.Refresh();

            ExperimentRunner runner = Runner.CreateGameObjectInSceneWithReferences();
            if (runner == null) return;
            
            Debug.Log("Linking Scripts");
            
            runner.scriptReferences.DragTrialScriptHere = Trial.GetReference();
            runner.scriptReferences.DragBlockScriptHere = Block.GetReference();
            runner.scriptReferences.DragExperimentScriptHere = Experiment.GetReference();
            
            runner.SetExperimentDesignFile(Design.CheckCreated());
            
         
        }
        
        
    }

    public abstract class ExperimentAsset<T> where T : ScriptableObject{

        const string AssetsFolderPath = "Assets";
        const string AssetExtension = ".asset";
        
        protected abstract string ComponentName { get; }
        
        public T CreateAsset(string experimentName)  {
            T fileObject = ScriptableObject.CreateInstance<T>();
            fileObject.name = GetName(experimentName);
            Setup(fileObject, experimentName);
            string assetFilePath = GetAssetFilePath(experimentName);
            Directory.CreateDirectory(Path.GetDirectoryName(assetFilePath) ?? throw new NullReferenceException("DirectoryName Null"));
            AssetDatabase.CreateAsset(fileObject, assetFilePath);
            AssetDatabase.SaveAssets();
            EditorPrefs.SetString(ComponentName, experimentName);
            Debug.Log($"Created asset from template {assetFilePath} type; {typeof(T)}");
            return fileObject;
        }
        
        public T CheckCreated() {
            if (!EditorPrefs.HasKey(ComponentName)) return null;
            string experimentName = EditorPrefs.GetString(ComponentName);
            EditorPrefs.DeleteKey(ComponentName);

            string pathToTrialScript = GetAssetFilePath(experimentName);
            T loadAssetAtPath = AssetDatabase.LoadAssetAtPath(pathToTrialScript, typeof(ScriptableObject)) as T;
            return loadAssetAtPath;
        }

        string GetName(string experimentName) {
            return experimentName + ComponentName;
        }
        
        string GetAssetFilePath(string experimentName) {
            string folder = Path.Combine(AssetsFolderPath, experimentName + "Experiment");
            return Path.Combine(folder, GetName(experimentName) + AssetExtension);
        }
        
        protected abstract void Setup(T fileObject, string experimentName);


    }

    public class DesignExperimentAsset : ExperimentAsset<ExperimentDesignFile2> {
        protected override string ComponentName => "Design";
        protected override void Setup(ExperimentDesignFile2 fileObject, string experimentName) {
            fileObject.ExperimentSettings = new SettingsExperimentAsset().CreateAsset(experimentName);
        }
        
    }

    public class SettingsExperimentAsset : ExperimentAsset<ExperimentSettings> {
        protected override string ComponentName => "Settings";
        protected override void Setup(ExperimentSettings fileObject, string experimentName) {
        }
        
    }

    public abstract class ScriptTemplateComponent {
        
        const string AssetsFolderPath = "Assets";
        const string ScriptExtension = ".cs";
        const string CustomScriptsFolder = "CustomScripts";
        protected abstract string ComponentName { get; }
        protected abstract string TemplateName { get; }
        
        public abstract string ReplacementParseString { get; }
        

        public string TemplatePath => Path.Combine(FileLocationSettings.TemplatePath , TemplateName);

        
        public void CreateScript(string experimentName) {
            string fileText = File.ReadAllText(TemplatePath);
            string updatedText = fileText.Replace(ReplacementParseString, experimentName + ComponentName);

            string outPath = GetScriptFilePath(experimentName);
            Directory.CreateDirectory(Path.GetDirectoryName(outPath) ?? throw new NullReferenceException("DirectoryName Null"));
            
            EditorPrefs.SetString (ComponentName, experimentName);
            
            File.WriteAllText(outPath, updatedText);
            Debug.Log($"Created Script from template {outPath} fullName");
            AssetDatabase.Refresh();
        }

        string GetScriptFilePath(string experimentName) {
            string fileName = GetName(experimentName) + ScriptExtension;
            string folder = Path.Combine(AssetsFolderPath, experimentName + "Experiment", CustomScriptsFolder);
            return Path.Combine( folder, fileName);
        } 
        
        string GetName(string experimentName) {
            return experimentName + ComponentName;
        }


        public MonoScript GetReference() {
            if (!EditorPrefs.HasKey(ComponentName)) {
                Debug.LogWarning($"No Key {ComponentName}");
                return null;
            }

            string experimentName = EditorPrefs.GetString(ComponentName);
            EditorPrefs.DeleteKey(ComponentName);

            string pathToTrialScript = GetScriptFilePath(experimentName);
            
            MonoScript loadAssetAtPath = AssetDatabase.LoadAssetAtPath(pathToTrialScript, typeof(TextAsset)) as MonoScript;
            string message = loadAssetAtPath != null ? loadAssetAtPath.name : "null script";
            Debug.Log($"Linking {message}: {pathToTrialScript}");
            
            return loadAssetAtPath;
        }

    }

    public class RunnerScriptTemplateComponent : ScriptTemplateComponent {
        protected override string ComponentName => "Runner";
        protected override string TemplateName => "ExperimentRunnerTemplate.cs";
        public override string ReplacementParseString => "___ExperimentRunnerClassName___";

        public string ReferencedKey => ComponentName + "Referenced";
        

        public ExperimentRunner CreateGameObjectInSceneWithReferences() {

            if (!EditorPrefs.HasKey(ComponentName)) return null;
            string experimentName = EditorPrefs.GetString(ComponentName);
            string typeName = experimentName + ComponentName;
            Debug.Log($"Linking {ComponentName} type {typeName}");
            EditorPrefs.DeleteKey(ComponentName);
            
            // Get the new type from the reloaded assembly!
            // (It won’t work without assembly specification, because this
            //  is an editor script, so the default assembly is “Assembly-CSharp-editor”)

            Type type = null;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies) {
                type = assembly.GetType(typeName);
                if (type != null)
                    break;
            }

            if (type == null) {

                Debug.LogError($"{TuxLog.Prefix} Cannot find runner script to create. Name: {typeName} Key: {ComponentName}, String: {EditorPrefs.GetString(ComponentName)}");
                Debug.LogError($"{TuxLog.Prefix} Printing Verbose error report:");
                Debug.LogError($"{TuxLog.Prefix}");
                foreach (Assembly assembly in assemblies) {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(assembly.ToString());
                    foreach (Type listedType in assembly.GetTypes()) {
                        sb.AppendLine(listedType.ToString());
                    }

                    Debug.Log($"{TuxLog.Prefix} {sb}");
                }

                
            }
            GameObject newGameObject = new GameObject();
            newGameObject.name = typeName;
            Debug.Log($"{TuxLog.Prefix} Creating GameObject type {type} in currently open scene");
            
            ExperimentRunner runner = newGameObject.AddComponent(type) as ExperimentRunner;
            return runner;
        }


        
    }
    
    public class ExperimentScriptTemplateComponent : ScriptTemplateComponent {
        protected override string ComponentName => "Experiment";
        protected override string TemplateName => "ExperimentScriptTemplate.cs";
        public override string ReplacementParseString => "___ExperimentClassName___";
    }

    public class TrialScriptTemplateComponent : ScriptTemplateComponent {
        protected override string ComponentName => "Trial";
        protected override string TemplateName => "TrialScriptTemplate.cs";
        public override string ReplacementParseString => "___TrialClassName___";
    }

    public class BlockScriptTemplateComponent : ScriptTemplateComponent {
        protected override string ComponentName => "Block";
        protected override string TemplateName => "BlockScriptTemplate.cs";
        public override string ReplacementParseString => "___BlockClassName___";
    }
    
}
