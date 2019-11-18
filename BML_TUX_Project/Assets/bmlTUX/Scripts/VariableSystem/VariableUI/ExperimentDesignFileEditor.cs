using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using bmlTUX.Scripts.ExperimentParts;
using bmlTUX.Scripts.UI.EditorUI;
using bmlTUX.Scripts.VariableSystem.VariableTypes;
using UnityEditor;
using UnityEngine;

namespace bmlTUX.Scripts.VariableSystem.VariableUI {
    [CustomEditor(typeof(ExperimentDesignFile))]
    public class ExperimentDesignFileEditor : Editor {


        bool               showAdvanced;
        VariableFactory factory;
        SerializedProperty factoryProp;
        SerializedProperty trialTableGenerationMode;
        SerializedProperty orderConfigs;

        SerializedProperty   trialRepetitions;
        SerializedProperty   experimentRepetitions;
        SerializedProperty   columnNameSettings;
        SerializedProperty   controlSettings;
        SerializedProperty   guiSettings;
        SerializedProperty   blockRandomizationMode;
        SerializedProperty   trialRandomizationMode;
        SerializedProperty   trialRandomizationSubType;
        SerializedProperty   blockPartialRandomizationSubType;
        ExperimentDesignFile designFileTarget;
        List<VariableViewer> ivViewers;
        List<VariableViewer> dvViewers;
        List<VariableViewer> pvViewers;
        public List<VariableViewer> ListToDelete = new List<VariableViewer>();

       
   
        const int IndentWidth  = 10;
        const float VariablePanelBackgroundLightness = .55f;
        Color variablePanelBackgroundColor;
        SerializedProperty fileLocationSettings;

        void OnEnable() {
            variablePanelBackgroundColor = new Color(VariablePanelBackgroundLightness, VariablePanelBackgroundLightness, VariablePanelBackgroundLightness, 1);
            
            
            designFileTarget = target as ExperimentDesignFile;
            if (designFileTarget == null) {
                throw new NullReferenceException("Null ExperimentDesignFile");
            }

            factoryProp = serializedObject.FindProperty(nameof(ExperimentDesignFile.Factory));
            if (factoryProp == null) throw new NullReferenceException("null factory prop");
            factory = designFileTarget.Factory;
            if (factory == null) throw new NullReferenceException("null factory");
            
            
            trialTableGenerationMode = serializedObject.FindProperty(nameof(ExperimentDesignFile.TrialTableGeneration));
            orderConfigs = serializedObject.FindProperty(nameof(ExperimentDesignFile.BlockOrderConfigurations));
            blockRandomizationMode = serializedObject.FindProperty(nameof(ExperimentDesignFile.BlockRandomization));
            trialRandomizationMode = serializedObject.FindProperty(nameof(ExperimentDesignFile.TrialRandomization));
            trialRandomizationSubType =
                serializedObject.FindProperty(nameof(ExperimentDesignFile.TrialPermutationType));
            blockPartialRandomizationSubType =
                serializedObject.FindProperty(nameof(ExperimentDesignFile.BlockPartialRandomizationSubType));
            trialRepetitions = serializedObject.FindProperty(nameof(ExperimentDesignFile.TrialRepetitions));
            experimentRepetitions = serializedObject.FindProperty(nameof(ExperimentDesignFile.ExperimentRepetitions));
            columnNameSettings = serializedObject.FindProperty(nameof(ExperimentDesignFile.ColumnNamesSettings));
            controlSettings = serializedObject.FindProperty(nameof(ExperimentDesignFile.ControlSettings));
            guiSettings = serializedObject.FindProperty(nameof(ExperimentDesignFile.GuiSettings));
            fileLocationSettings = serializedObject.FindProperty(nameof(ExperimentDesignFile.FileLocationSettings));
            CreateAllViewers();
            
        }
        
        void CreateIndependentVariableViewers() {
            ivViewers =  new List<VariableViewer>();
            CreateViewersFrom(nameof(VariableFactory.IntIVs), ivViewers);
            CreateViewersFrom(nameof(VariableFactory.FloatIVs), ivViewers);
            CreateViewersFrom(nameof(VariableFactory.StringIVs), ivViewers);
            CreateViewersFrom(nameof(VariableFactory.BoolIVs), ivViewers);
            CreateViewersFrom(nameof(VariableFactory.GameObjectIVs), ivViewers);
            CreateViewersFrom(nameof(VariableFactory.Vector2IVs), ivViewers);
            CreateViewersFrom(nameof(VariableFactory.Vector3IVs), ivViewers);
            CreateViewersFrom(nameof(VariableFactory.CustomDataTypeIVs), ivViewers);
        }
        
        void CreateParticipantVariableViewers() {
            pvViewers =  new List<VariableViewer>();
            CreateViewersFrom(nameof(VariableFactory.IntParticipantVariables), pvViewers);
            CreateViewersFrom(nameof(VariableFactory.FloatParticipantVariables), pvViewers);
            CreateViewersFrom(nameof(VariableFactory.StringParticipantVariables), pvViewers);
            CreateViewersFrom(nameof(VariableFactory.BoolParticipantVariables), pvViewers);
            CreateViewersFrom(nameof(VariableFactory.GameObjectParticipantVariables), pvViewers);
            CreateViewersFrom(nameof(VariableFactory.Vector2ParticipantVariables), pvViewers);
            CreateViewersFrom(nameof(VariableFactory.Vector3ParticipantVariables), pvViewers);
            CreateViewersFrom(nameof(VariableFactory.CustomDataParticipantVariables), pvViewers);
        }
        
        void CreateDependentVariableViewers() {
            dvViewers = new List<VariableViewer>();
            CreateViewersFrom(nameof(VariableFactory.IntDVs), dvViewers);
            CreateViewersFrom(nameof(VariableFactory.FloatDVs), dvViewers);
            CreateViewersFrom(nameof(VariableFactory.StringDVs), dvViewers);
            CreateViewersFrom(nameof(VariableFactory.BoolDVs), dvViewers);
            CreateViewersFrom(nameof(VariableFactory.GameObjectDVs), dvViewers);
            CreateViewersFrom(nameof(VariableFactory.Vector2DVs), dvViewers);
            CreateViewersFrom(nameof(VariableFactory.Vector3DVs), dvViewers);
            CreateViewersFrom(nameof(VariableFactory.CustomDataTypeDVs), dvViewers);
        }

        void CreateViewersFrom(string variableRelativeName, List<VariableViewer> viewerList) {
            SerializedProperty list = factoryProp.FindPropertyRelative(variableRelativeName);
            
            for (int index = 0; index < list.arraySize; index++) {
                SerializedProperty variableProp = list.GetArrayElementAtIndex(index);
                viewerList.Add(new VariableViewer(this, variableProp, list, index));
            }
        }
        

        public override void OnInspectorGUI() {
            serializedObject.Update();

            ShowRepetitionAndRandomizationSettings();
            EditorGUILayout.Space();

            ShowVariableFactory();
            EditorGUILayout.Space();
            ShowPreviewButton();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            ShowAdvancedOptions();
            EditorGUILayout.Space();
            DeleteVariablesFlaggedForDeletion();
            
           
            serializedObject.ApplyModifiedProperties();

        }

        void DeleteVariablesFlaggedForDeletion() {
            foreach (VariableViewer variableViewer in ListToDelete) {
                
                int variableViewerIndex = variableViewer.Index;
                SerializedProperty containingList = variableViewer.ContainingList;
                containingList.DeleteArrayElementAtIndex(variableViewerIndex);
            }
            ListToDelete.Clear();
            RebuildEditor();
            
        }


        void RebuildEditor() {
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
            
            CreateAllViewers();
            EditorUtility.SetDirty(this);
        }

        void CreateAllViewers() {
            CreateIndependentVariableViewers();
            CreateDependentVariableViewers();
            CreateParticipantVariableViewers();
        }


        void ShowVariableFactory() {
            
            if (factory == null) return;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Variables", EditorStyles.boldLabel);

            ShowVariableCreationInterface();


            GUIStyle variableStyle = new GUIStyle(GUI.skin.box) {
                                                                    normal = {background = MakeTex(variablePanelBackgroundColor)}
                                                                };
            
            EditorGUILayout.LabelField("Independent Variables", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(variableStyle);
            EditorGUI.indentLevel++;
            ShowViewers(ivViewers);
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Dependent Variables", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(variableStyle);
            EditorGUI.indentLevel++;
            ShowViewers(dvViewers);
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Participant Variables", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(variableStyle);
            EditorGUI.indentLevel++;
            ShowViewers(pvViewers);
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            
            EditorGUILayout.EndVertical();
            
        }

        void ShowVariableCreationInterface() {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            //GUILayout.Space((EditorGUI.indentLevel +1) * IndentSize);
            if (GUILayout.Button("Create Variable:")) {
                CreateNewVariableAndViewer();
            }

            factory.VariableTypeToCreate = (VariableType) EditorGUILayout.EnumPopup(factory.VariableTypeToCreate);
            factory.DataTypeToCreate = (SupportedDataType) EditorGUILayout.EnumPopup(factory.DataTypeToCreate);
            EditorGUILayout.EndHorizontal();
        }

        void CreateNewVariableAndViewer() {
            if (factory.VariableTypeToCreate == VariableType.ChooseType || factory.DataTypeToCreate == SupportedDataType.ChooseType) {
                Debug.LogWarning("Need to select variable type and data type before creating a variable");
                return;
            }
            factory.AddNew();
            RebuildEditor();
        }


        void ShowViewers(List<VariableViewer> dict) {
            if (CheckEmptyDict(dict)) return;
            foreach (VariableViewer item in dict) {
                item.Show();
            }
        }
        bool CheckEmptyDict(IList dict) {
            if (dict.Count == 0) {
                EditorGUILayout.LabelField("None");
                return true;
            }
            return false;
        }


        void ShowPreviewButton() {
            designFileTarget = target as ExperimentDesignFile;
            if (GUILayout.Button("Preview Design", GUILayout.Height(50))) {
                DesignPreviewEditorWindow.ShowWindow(designFileTarget);
            }
        }

        void ShowRepetitionAndRandomizationSettings() {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Randomization and Repetition", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(trialRepetitions);
            EditorGUILayout.PropertyField(experimentRepetitions);

            EditorGUILayout.PropertyField(blockRandomizationMode);

            if (blockRandomizationMode.enumValueIndex == (int) BlockRandomizationMode.PartialRandomization) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(blockPartialRandomizationSubType);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(trialRandomizationMode);

            if (trialRandomizationMode.enumValueIndex == (int) TrialRandomizationMode.Randomized &&
                designFileTarget.HasBlocks) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(trialRandomizationSubType);
                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        void ShowAdvancedOptions() {
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            ShowNameAndEditButton();
            
            EditorGUI.indentLevel++;

            EditorGUILayout.HelpBox("See wiki for information.", MessageType.Info);
            
            if (showAdvanced) {
                
                ShowTrialTableOptions();
                EditorGUILayout.Space();
                
                ShowBlockOrderConfiguration();
                EditorGUILayout.Space();
                ShowSettingsFields();
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

        }

        void ShowSettingsFields() {
            EditorGUILayout.LabelField("Other Settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel += 2;
            EditorGUILayout.PropertyField(columnNameSettings);
            EditorGUILayout.PropertyField(controlSettings);
            EditorGUILayout.PropertyField(guiSettings);
            EditorGUI.indentLevel+= 2;
            EditorGUILayout.HelpBox("Change the monitor to which the UI is rendered", MessageType.Info);
            EditorGUI.indentLevel-= 2;
            EditorGUILayout.PropertyField(fileLocationSettings);
            EditorGUI.indentLevel -= 2;
        }

        void ShowBlockOrderConfiguration() {
            EditorGUILayout.LabelField("Manual block order configuration",  EditorStyles.boldLabel);
            EditorGUI.indentLevel += 2;


            if (orderConfigs.arraySize > 0) {
                for (int i = 0; i < orderConfigs.arraySize; i++) {
                    SerializedProperty order = orderConfigs.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(order, true);
                }
            }
            else {
                EditorGUILayout.LabelField("None defined");
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space((EditorGUI.indentLevel + 1) * IndentWidth);
            if (GUILayout.Button("Add New", GUILayout.Width(150))) {
                CreateNewBlockOrderDefinition();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel -= 2;
        }

        void CreateNewBlockOrderDefinition() {
            
            ExperimentDesignFile experimentDesignFile = target as ExperimentDesignFile;
            if (experimentDesignFile == null) throw new NullReferenceException("Can't read experimental design from file check for errors");
            if (!experimentDesignFile.HasBlocks) {
                Debug.LogError("Tried to make block order definition file, but no block variables have been defined!");
                return;
            }

            if (experimentDesignFile != null) {
                List<BlockOrderDefinition> orders = experimentDesignFile.BlockOrderConfigurations;
                BlockOrderDefinition newBlockOrderDefinition = CreateInstance<BlockOrderDefinition>();
                try {
                    newBlockOrderDefinition.InitFromDesign(experimentDesignFile);
                    orders.Add(newBlockOrderDefinition);
                    string savePath =
                        Path.GetDirectoryName(path: AssetDatabase.GetAssetPath(Selection.activeObject)) +
                        "/New Block Order Definition.asset";
                    AssetDatabase.CreateAsset(newBlockOrderDefinition, savePath);
                    AssetDatabase.SaveAssets();

                    EditorUtility.FocusProjectWindow();

                    Selection.activeObject = newBlockOrderDefinition;
                }
                catch (ArgumentNullException) {
                    Debug.LogError("Could not create BlockOrderDefinition. There is probably an error in variable definitions.");
                }
            }
        }

        void ShowTrialTableOptions() {
            
            EditorGUILayout.LabelField("Pre-generated experiment table options", EditorStyles.boldLabel);
            EditorGUI.indentLevel += 2;

            EditorGUILayout.PropertyField(trialTableGenerationMode);

            GUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUI.indentLevel * IndentWidth);
            if (trialTableGenerationMode.enumValueIndex == (int) TrialTableGenerationMode.PreGenerated) {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(EditorGUI.indentLevel * IndentWidth);
                if (GUILayout.Button("Generate a Trial Table", GUILayout.Width(250))) {
                    DesignSaverEditorWindow.ShowWindow(Selection.activeObject as ExperimentDesignFile);
                }
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.EndHorizontal();

            EditorGUI.indentLevel -= 2;
        }

        void ShowNameAndEditButton() {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Advanced Settings", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            if (showAdvanced) {
                if (GUILayout.Button("Hide", GUILayout.Width(150))) {
                    showAdvanced = false;
                }
            }
            else {
                if (GUILayout.Button("Show", GUILayout.Width(150))) {
                    showAdvanced = true;
                }
            }

            EditorGUILayout.EndHorizontal();
            
        }


        Texture2D MakeTex(Color col )
        {
            Color[] pix = new Color[1 * 1];
            for(int i = 0; i < pix.Length; ++i ) {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(1, 1 );
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}