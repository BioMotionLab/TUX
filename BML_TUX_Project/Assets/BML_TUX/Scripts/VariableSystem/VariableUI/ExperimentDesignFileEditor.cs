using System.Collections.Generic;
using System.IO;
using BML_ExperimentToolkit.Scripts.UI.Editor;
using UnityEditor;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.VariableUI {
    [CustomEditor(typeof(ExperimentDesignFile))]
    public class ExperimentDesignFileEditor : Editor {


        bool               showAdvanced;
        SerializedProperty factory;
        SerializedProperty trialTableGenerationMode;
        SerializedProperty orderConfigs;
        
        SerializedProperty TrialRepetitions;
        SerializedProperty ExperimentRepetitions;
        SerializedProperty columnNameSettings;
        SerializedProperty controlSettings;
        SerializedProperty guiSettings;
        SerializedProperty blockRandomizationMode;
        SerializedProperty trialRandomizationMode;
        SerializedProperty trialRandomizationSubType;
        SerializedProperty blockPartialRandomizationSubType;

        void OnEnable() {
            factory = serializedObject.FindProperty(nameof(ExperimentDesignFile.Factory));
            trialTableGenerationMode = serializedObject.FindProperty(nameof(ExperimentDesignFile.TrialTableGeneration));
            orderConfigs = serializedObject.FindProperty(nameof(ExperimentDesignFile.BlockOrderConfigurations));
            
            blockRandomizationMode = serializedObject.FindProperty(nameof(ExperimentDesignFile.BlockRandomizationMode));
            trialRandomizationMode = serializedObject.FindProperty(nameof(ExperimentDesignFile.TrialRandomizationMode));
            trialRandomizationSubType = serializedObject.FindProperty(nameof(ExperimentDesignFile.TrialRandomizationSubType));
            blockPartialRandomizationSubType = serializedObject.FindProperty(nameof(ExperimentDesignFile.BlockPartialRandomizationSubType));
            TrialRepetitions = serializedObject.FindProperty(nameof(ExperimentDesignFile.TrialRepetitions));
            ExperimentRepetitions = serializedObject.FindProperty(nameof(ExperimentDesignFile.ExperimentRepetitions));
            columnNameSettings = serializedObject.FindProperty(nameof(ExperimentDesignFile.ColumnNamesSettings));
            controlSettings = serializedObject.FindProperty(nameof(ExperimentDesignFile.ControlSettings));
            guiSettings = serializedObject.FindProperty(nameof(ExperimentDesignFile.GuiSettings));
            
            
        }
        

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            
            ShowRepetitionAndRandomizationSettings();
            
            
            EditorGUILayout.PropertyField(factory);
            
            EditorGUILayout.LabelField("--------");
            
            if (GUILayout.Button("Preview Design", GUILayout.Width(250), GUILayout.Height(50))){
                DesignPreviewWindow.ShowWindow(target as ExperimentDesignFile);
                
            }
            
            EditorGUILayout.LabelField("--------");
            EditorGUILayout.LabelField("Advanced (see wiki for information):", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical();
            EditorGUI.indentLevel++;
            if (showAdvanced) {
                ShowAdvancedOptions();
            }
            else {
                if (GUILayout.Button("Show", GUILayout.Width(250))) {
                    showAdvanced = true;
                }
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }

        void ShowRepetitionAndRandomizationSettings() {
            EditorGUILayout.LabelField("Randomization and Repetition settings:");

            EditorGUILayout.PropertyField(TrialRepetitions);
            EditorGUILayout.PropertyField(ExperimentRepetitions);

            EditorGUILayout.PropertyField(blockRandomizationMode);

            if (blockRandomizationMode.enumValueIndex == (int) BlockRandomizationMode.PartialRandomization) {
                EditorGUI.indentLevel += 2;
                EditorGUILayout.PropertyField(blockPartialRandomizationSubType);
                EditorGUI.indentLevel -= 2;
            }
            
            EditorGUILayout.PropertyField(trialRandomizationMode);

            if (trialRandomizationMode.enumValueIndex == (int) TrialRandomizationMode.Randomized) {
                EditorGUI.indentLevel += 2;
                EditorGUILayout.PropertyField(trialRandomizationSubType);
                EditorGUI.indentLevel -= 2;
            }
                
        }

        void ShowAdvancedOptions() {
            
            if (GUILayout.Button("Hide Advanced Options", GUILayout.Width(250))) {
                showAdvanced = false;
            }

            EditorGUILayout.LabelField("Pre-generated experiment table options:");
            EditorGUI.indentLevel += 2;

            EditorGUILayout.PropertyField(trialTableGenerationMode);


            if (GUILayout.Button("Generate Design File Manually", GUILayout.Width(250))) {
                DesignSaverWindow.ShowWindow(Selection.activeObject as ExperimentDesignFile);
            }

            EditorGUI.indentLevel -= 2;

            EditorGUILayout.LabelField("Manual block order configuration");
            
            EditorGUI.indentLevel += 2;
            EditorGUILayout.LabelField("Note: This system works is but due for an overhaul, see wiki", EditorStyles.miniLabel);

            for (int i = 0; i < orderConfigs.arraySize; i++) {
                SerializedProperty order = orderConfigs.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(order);
            }


            if (GUILayout.Button("Add New BlockOrderDefinition")) {
                ExperimentDesignFile experimentDesignFile = target as ExperimentDesignFile;
                if (experimentDesignFile != null) {
                    List<BlockOrderDefinition> orders = experimentDesignFile.BlockOrderConfigurations;
                    BlockOrderDefinition newBlockOrderDefinition = CreateInstance<BlockOrderDefinition>();
                    newBlockOrderDefinition.InitFromDesign(experimentDesignFile);
                    orders.Add(newBlockOrderDefinition);
                    string savePath = Path.GetDirectoryName(path: AssetDatabase.GetAssetPath(Selection.activeObject)) + "/New Block Order Definition.asset";
                    AssetDatabase.CreateAsset(newBlockOrderDefinition, savePath);
                    AssetDatabase.SaveAssets();

                    EditorUtility.FocusProjectWindow();

                    Selection.activeObject = newBlockOrderDefinition;
                }
            }
            
            
            EditorGUI.indentLevel -= 2;
            
            EditorGUILayout.LabelField("Settings:");
            EditorGUI.indentLevel += 2;
            EditorGUILayout.PropertyField(columnNameSettings);
            EditorGUILayout.PropertyField(controlSettings);
            EditorGUILayout.PropertyField(guiSettings);
            EditorGUI.indentLevel -= 2;
        }
    }
}