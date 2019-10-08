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
        SerializedProperty randomizationMode;
        SerializedProperty repeatTrialsInBlock;
        SerializedProperty repeatAllBlocks;
        SerializedProperty columnNameSettings;
        SerializedProperty controlSettings;
        SerializedProperty guiSettings;

        void OnEnable() {
            factory = serializedObject.FindProperty(nameof(ExperimentDesignFile.Factory));
            trialTableGenerationMode = serializedObject.FindProperty(nameof(ExperimentDesignFile.TrialTableGeneration));
            orderConfigs = serializedObject.FindProperty(nameof(ExperimentDesignFile.BlockOrderConfigurations));
            
            randomizationMode = serializedObject.FindProperty(nameof(ExperimentDesignFile.RandomizationMode));
            repeatTrialsInBlock = serializedObject.FindProperty(nameof(ExperimentDesignFile.RepeatTrialsInBlock));
            repeatAllBlocks = serializedObject.FindProperty(nameof(ExperimentDesignFile.RepeatAllBlocks));
            columnNameSettings = serializedObject.FindProperty(nameof(ExperimentDesignFile.ColumnNamesSettings));
            controlSettings = serializedObject.FindProperty(nameof(ExperimentDesignFile.ControlSettings));
            guiSettings = serializedObject.FindProperty(nameof(ExperimentDesignFile.GuiSettings));
            
            
        }
        

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.LabelField("Randomization and Repetition settings:");
            EditorGUILayout.PropertyField(randomizationMode);
            EditorGUILayout.PropertyField(repeatTrialsInBlock);
            EditorGUILayout.PropertyField(repeatAllBlocks);
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