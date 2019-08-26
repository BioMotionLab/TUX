using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using BML_ExperimentToolkit.Scripts.ExperimentParts;
using BML_ExperimentToolkit.Scripts.Settings;
using BML_Utilities;
using BML_Utilities.Extensions;
using UnityEditor;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.VariableSystem {

    [CreateAssetMenu]
    public class VariableConfig : ScriptableObject {

        public bool ShuffleTrialOrder = false;
        public bool ShuffleDifferentlyForEachBlock = false;
        
        [Range(1,100)]
        public int  RepeatTrialsInBlock = 1;
        
        [Range(1,100)]
        public int RepeatAllBlocks = 1;
        
        [SerializeField]
        public VariableFactory Factory = new VariableFactory();

        public ColumnNamesSettings ColumnNamesSettings;
        public ControlSettings ControlSettings;
        public GuiSettings GuiSettings;

        [Space]
        [Header("Advanced:")]
        [Space]
        [SerializeField]
        public TrialTableGenerationMode TrialTableGenerationMode = TrialTableGenerationMode.OnTheFly;
        
        [SerializeField]
        public List<OrderConfig> OrderConfigs = new List<OrderConfig>();

        public void Validate() {
            
            if (ColumnNamesSettings == null) {
                throw new NullReferenceException(
                                                 "Configuration file does not have column name settings defined. " + 
                                                 "Please drag column name settings into the proper place in the config file");
            }

            if (ControlSettings == null) {
                throw new NullReferenceException(
                                                 "Configuration file does not have Control Settings defined. " +
                                                 "Please drag control settings into the proper place in the config file");
            }
        }

        public Variables Variables => Factory.Variables;
        
        
    }
    
    
    public class DesignSaverWindow : EditorWindow {
        
        [SerializeField]
        int orderIndex = default;

        string fileName = "experimentDesignSave";
        
        string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

        VariableConfig configFile;

        void OnGUI() {
            
            
            
            configFile = Selection.activeObject as VariableConfig;
            if (configFile == null) {
                EditorGUILayout.HelpBox("Need to have a Variable Config File Selected", MessageType.Warning);
                return;
            }

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField($"Config File Selected: {configFile.name}");
            
            EditorGUILayout.Space();
            
            ExperimentDesign design = ExperimentDesign.CreateFrom(configFile);
            
            EditorGUILayout.LabelField("Select A Block Order");
            
            string[] orderStrings = design.BlockPermutationsStrings.ToArray();
            orderIndex = EditorGUILayout.Popup(orderIndex, orderStrings);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            fileName = EditorGUILayout.TextField("FileName:", fileName);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"folder: {folder}");
            if (GUILayout.Button("Change Folder")) {
                folder = EditorUtility.OpenFolderPanel("Select Folder", "", "");
            }
            EditorGUILayout.EndHorizontal();
            
            DataTable finalTable = design.GetFinalExperimentTable(orderIndex);
            
            if (GUILayout.Button("Save")) {
                string fileNameWithExtension = fileName + ".csv";
                string path = Path.Combine(folder, fileNameWithExtension);
                File.WriteAllText(path, finalTable.AsString(truncateLength:-1, separator:Delimiter.Comma)); 
            }
            
            EditorGUILayout.Space();
            
            EditorGUILayout.TextArea(finalTable.AsString());
            
            EditorGUILayout.EndVertical();

        }

        public static void ShowWindow() {
            DesignSaverWindow window = (DesignSaverWindow) GetWindow(typeof(DesignSaverWindow), false, "Design Saver");
            window.Show();
            
        }
    }
}