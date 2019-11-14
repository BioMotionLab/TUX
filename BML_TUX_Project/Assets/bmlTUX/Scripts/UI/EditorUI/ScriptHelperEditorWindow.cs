using System;
using System.IO;
using bmlTUX.Scripts.ExperimentParts;
using bmlTUX.Scripts.Settings;
using bmlTUX.Scripts.VariableSystem;
using UnityEditor;
using UnityEngine;

namespace bmlTUX.Scripts.UI.EditorUI {
    public class ScriptHelperEditorWindow : EditorWindow {
        const string RunnerKey = "ExperimentRunner";
        const string TrialKey = "Trial";
        const string BlockKey = "Block";
        const string ExperimentKey = "Experiment";
        const string DesignFileKey = "DesignFile";

        string experimentName;
        
        public string ExperimentRunnerTemplatePath => FileLocationSettings.TemplatePath + "ExperimentRunnerTemplate.cs";
        public string TrialTemplatePath => FileLocationSettings.TemplatePath + "TrialScriptTemplate.cs";
        public string BlockTemplatePath => FileLocationSettings.TemplatePath + "BlockScriptTemplate.cs";
        public string ExperimentTemplatePath => FileLocationSettings.TemplatePath + "ExperimentScriptTemplate.cs";
        
        /// <summary>
        /// Add menu item to open this window
        /// </summary>
        [MenuItem(TUXMenuNames.BmlMainMenu + "Script Helper Tool")]
        public static void ShowWindow() {
            //Show existing window instance. If one doesn't exist, make one.
            GetWindow(typeof(ScriptHelperEditorWindow), false, "Script Helper Window");
        }


        void OnGUI() {
            
            EditorGUILayout.HelpBox("This tool will generate custom scripts for you that you can then edit. " +
                                     "The script names are derived from the name of your experiment. " +
                                     "Make sure the name contains only letters and no spaces", MessageType.Info);
            
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
                                        "\tDesign file\n" +
                                            "\tRunner GameObject with attached Runner script\n" +
                                            "\tCustom Trial script\n" +
                                            "\tCustom Block script\n" +
                                            "\tCustom Experiment script\n", 
                                        MessageType.Info);
                
                if (GUILayout.Button("Automatically set everything up for me!")) {
                    CreateExperimentRunnerScriptFromTemplate();
                    CreateTrialScriptFromTemplate();
                    CreateBlockScriptFromTemplate();
                    CreateExperimentScriptFromTemplate();
                    CreateDesignFile();
                }
                
                

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                
                EditorGUILayout.LabelField("Manual set up of experiment components:", EditorStyles.boldLabel);

                EditorGUILayout.HelpBox("If you make scripts individually, remember to drag them into the fields in the ExperimentRunner Script in your unity scene", MessageType.Warning);

                if (GUILayout.Button("Create Experiment Design File")) {
                    CreateDesignFile();
                }
            

                if (GUILayout.Button("Create ExperimentRunner Script")) {
                    CreateExperimentRunnerScriptFromTemplate();
                }
                
                
                if (GUILayout.Button("Create Custom Trial Script")) {
                    CreateTrialScriptFromTemplate();
                }
                
                if (GUILayout.Button("Create Custom Block Script")) {
                    CreateBlockScriptFromTemplate();
                }
                
                if (GUILayout.Button("Create Custom Experiment Script")) {
                    CreateExperimentScriptFromTemplate();
                }
                
            }


        }

        void CreateDesignFile() {
            ExperimentDesignFile file = CreateInstance<ExperimentDesignFile>();
            file.name = experimentName + DesignFileKey;
            AssetDatabase.CreateAsset(file, "Assets/" + file.name + ".asset");
            AssetDatabase.SaveAssets();
            EditorPrefs.SetString(DesignFileKey, file.name);
        }

        void CreateExperimentRunnerScriptFromTemplate() {
            string fileText = File.ReadAllText(ExperimentRunnerTemplatePath);
            string updatedText = fileText.Replace("___ExperimentRunnerClassName___", experimentName + RunnerKey);
            string outPath = Application.dataPath + "/" + experimentName + RunnerKey + ".cs";
            string typeName = Path.GetFileNameWithoutExtension(outPath);
            EditorPrefs.SetString (RunnerKey, typeName);
            
            WriteAsset(outPath, updatedText);
            
            
        }
        
        void CreateTrialScriptFromTemplate() {
            string fileText = File.ReadAllText(TrialTemplatePath);
            string updatedText = fileText.Replace("___TrialClassName___", experimentName + "TrialScript");
            string outPath = Application.dataPath + "/" + experimentName + "TrialScript" + ".cs";
            string typeName = Path.GetFileNameWithoutExtension(outPath);
            EditorPrefs.SetString (TrialKey, typeName);
            WriteAsset(outPath, updatedText);
        }
        
        void CreateBlockScriptFromTemplate() {
            string fileText = File.ReadAllText(BlockTemplatePath);
            string updatedText = fileText.Replace("___BlockClassName___", experimentName + "BlockScript");
            string outPath = Application.dataPath + "/" + experimentName + "BlockScript" + ".cs";
            string typeName = Path.GetFileNameWithoutExtension(outPath);
            EditorPrefs.SetString (BlockKey, typeName);
            WriteAsset(outPath, updatedText);
        }
        
        void CreateExperimentScriptFromTemplate() {
            string fileText = File.ReadAllText(ExperimentTemplatePath);
            string updatedText = fileText.Replace("___ExperimentClassName___", experimentName + "ExperimentScript");
            string outPath = Application.dataPath + "/" + experimentName + "ExperimentScript" + ".cs";
            string typeName = Path.GetFileNameWithoutExtension(outPath);
            EditorPrefs.SetString (ExperimentKey, typeName);
            WriteAsset(outPath, updatedText);
        }

        static void WriteAsset(string outPath, string updatedText) {
            File.WriteAllText(outPath, updatedText);
            AssetDatabase.Refresh();
        }


        [UnityEditor.Callbacks.DidReloadScripts]
        static void ScriptReloaded() {
            ExperimentRunner runner = CheckRunnerCreated();
            
            if (runner == null) return;
            
            CheckTrialCreated(runner);
            CheckBlockCreated(runner);
            CheckExperimentCreated(runner);
            CheckDesignFile(runner);

        }

        static void CheckTrialCreated(ExperimentRunner runner) {
            if (!EditorPrefs.HasKey(TrialKey)) return;
            
            string typeName = EditorPrefs.GetString(TrialKey);
            EditorPrefs.DeleteKey(TrialKey);

            string pathToTrialScript = "Assets/" + typeName + ".cs";
            runner.scriptReferences.DragTrialScriptHere = AssetDatabase.LoadAssetAtPath(pathToTrialScript, typeof(TextAsset)) as MonoScript;
        }
        
        static void CheckDesignFile(ExperimentRunner runner) {
            if (!EditorPrefs.HasKey(DesignFileKey)) return;
            
            string typeName = EditorPrefs.GetString(DesignFileKey);
            EditorPrefs.DeleteKey(DesignFileKey);

            string pathToTrialScript = "Assets/" + typeName + ".asset";
            runner.DesignFile = AssetDatabase.LoadAssetAtPath(pathToTrialScript, typeof(ScriptableObject)) as ExperimentDesignFile;
        }
        
        static void CheckBlockCreated(ExperimentRunner runner) {
            if (!EditorPrefs.HasKey(BlockKey)) return;
            
            string typeName = EditorPrefs.GetString(BlockKey);
            EditorPrefs.DeleteKey(BlockKey);

            string pathToTrialScript = "Assets/" + typeName + ".cs";
            runner.scriptReferences.DragBlockScriptHere = AssetDatabase.LoadAssetAtPath(pathToTrialScript, typeof(TextAsset)) as MonoScript;
        }
        
        static void CheckExperimentCreated(ExperimentRunner runner) {
            if (!EditorPrefs.HasKey(ExperimentKey)) return;
            
            string typeName = EditorPrefs.GetString(ExperimentKey);
            EditorPrefs.DeleteKey(ExperimentKey);

            string pathToTrialScript = "Assets/" + typeName + ".cs";
            runner.scriptReferences.DragExperimentScriptHere = AssetDatabase.LoadAssetAtPath(pathToTrialScript, typeof(TextAsset)) as MonoScript;
        }

        static ExperimentRunner CheckRunnerCreated() {
            if (!EditorPrefs.HasKey(RunnerKey)) return null;
            string typeName = EditorPrefs.GetString(RunnerKey);
            EditorPrefs.DeleteKey(RunnerKey);
            
            // Get the new type from the reloaded assembly!
            // (It won’t work without assembly specification, because this
            //  is an editor script, so the default assembly is “Assembly-CSharp-editor”)

            Type type = null;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies) {
                type = assembly.GetType(typeName);
                if (type != null)
                    break;
            }

            GameObject newGameObject = new GameObject();
            newGameObject.name = typeName;
            ExperimentRunner runner = newGameObject.AddComponent(type) as ExperimentRunner;

            return runner;
        }
    }
}
