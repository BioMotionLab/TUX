using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using bmlTUX.Scripts.ExperimentParts;
using bmlTUX.Scripts.UI.EditorUI;
using bmlTUX.Scripts.Utilities;
using bmlTUX.Scripts.VariableSystem.VariableTypes;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using VariableSystem;

namespace bmlTUX.Scripts.VariableSystem.VariableUI {

    [CustomEditor(typeof(ExperimentDesignFile2))]
    public class ExperimentDesignFile2Editor : Editor {
        
        VariableFactory2 factory;
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
        SerializedProperty showAdvanced;
        ExperimentDesignFile2 designFileTarget;
        Dictionary<string, VariableViewer> ivViewers;
        Dictionary<string, VariableViewer> dvViewers;
        Dictionary<string, VariableViewer> pvViewers;
     
        ReorderableList blockOrderFileList;

        public VariableType selectedVariableType;
        public SupportedDataType selectedDataType;
       
   
        const int IndentWidth  = 10;
        const float VariablePanelBackgroundLightness = .55f;
        Color variablePanelBackgroundColor;
        SerializedProperty fileLocationSettings;
        Dictionary<string,VariableViewer> allViewers;

        void OnEnable() {
            variablePanelBackgroundColor = new Color(VariablePanelBackgroundLightness, VariablePanelBackgroundLightness, VariablePanelBackgroundLightness, 1);

            designFileTarget = target as ExperimentDesignFile2;
            if (designFileTarget == null) {
                Debug.LogError($"Null ExperimentDesignFile2 in editor {target.name}", this);
                throw new NullReferenceException("Null ExperimentDesignFile");
            }

            factoryProp = serializedObject.FindProperty(nameof(ExperimentDesignFile2.Factory));
            if (factoryProp == null) throw new NullReferenceException("null factory prop");
            factory = designFileTarget.Factory;
            if (factory == null) throw new NullReferenceException("null factory");
            
            
            trialTableGenerationMode = serializedObject.FindProperty(nameof(ExperimentDesignFile2.TrialTableGeneration));
            orderConfigs = serializedObject.FindProperty(nameof(ExperimentDesignFile2.BlockOrderConfigurations));
            blockRandomizationMode = serializedObject.FindProperty(nameof(ExperimentDesignFile2.BlockRandomization));
            trialRandomizationMode = serializedObject.FindProperty(nameof(ExperimentDesignFile2.TrialRandomization));
            trialRandomizationSubType =
                serializedObject.FindProperty(nameof(ExperimentDesignFile2.TrialPermutationType));
            blockPartialRandomizationSubType =
                serializedObject.FindProperty(nameof(ExperimentDesignFile2.BlockPartialRandomizationSubType));
            trialRepetitions = serializedObject.FindProperty(nameof(ExperimentDesignFile2.TrialRepetitions));
            experimentRepetitions = serializedObject.FindProperty(nameof(ExperimentDesignFile2.ExperimentRepetitions));
            columnNameSettings = serializedObject.FindProperty(nameof(ExperimentDesignFile2.ColumnNamesSettings));
            controlSettings = serializedObject.FindProperty(nameof(ExperimentDesignFile2.ControlSettings));
            guiSettings = serializedObject.FindProperty(nameof(ExperimentDesignFile2.GuiSettings));
            fileLocationSettings = serializedObject.FindProperty(nameof(ExperimentDesignFile2.FileLocationSettings));
            showAdvanced = serializedObject.FindProperty(nameof(ExperimentDesignFile2.ShowAdvancedEditor));
            
            InitializeBlockOrderList();

            selectedDataType = SupportedDataType.ChooseType;
            selectedVariableType = VariableType.ChooseType;

        }

        void InitializeBlockOrderList() {
            blockOrderFileList = new ReorderableList(serializedObject,
                serializedObject.FindProperty(nameof(ExperimentDesignFile2.BlockOrderConfigurations)), true, false, true, true);
            blockOrderFileList.drawHeaderCallback = rect => { EditorGUI.LabelField(rect, "Selected Configurations:"); };
            blockOrderFileList.drawElementCallback = (rect, index, isActive, isFocused) => {
                var element = blockOrderFileList.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, element, GUIContent.none);
            };
        }

        public override void OnInspectorGUI() {
            DateTime time = DateTime.Now;
            //Debug.Log($"StartUpdate {time.Millisecond}");
            serializedObject.Update();
            
            
            if (ivViewers == null) ivViewers =  new Dictionary<string, VariableViewer>();
            if (dvViewers == null) dvViewers = new Dictionary<string, VariableViewer>();
            if (pvViewers == null) pvViewers =  new Dictionary<string, VariableViewer>();
            if (allViewers == null) allViewers = new Dictionary<string, VariableViewer>();


            ShowRepetitionAndRandomizationSettings();
            EditorGUILayout.Space();

            ShowVariableFactory();
            EditorGUILayout.Space();
            ShowPreviewButton();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            ShowAdvancedOptions();
            EditorGUILayout.Space();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            
            
            
            serializedObject.ApplyModifiedProperties();
            
            serializedObject.Update();
            CheckVariablesAddedDeletedFromFactoryList(nameof(VariableFactory2.IndependentVariables));
            CheckVariablesAddedDeletedFromFactoryList(nameof(VariableFactory2.DependentVariables));
            CheckVariablesAddedDeletedFromFactoryList(nameof(VariableFactory2.ParticipantVariables));
            serializedObject.ApplyModifiedProperties();
            
            //Debug.Log($"End Update {time.Millisecond}");
        }

        void ShowVariableFactory() {
            if (factory == null) return;
            
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Variables", EditorStyles.boldLabel);
            ShowVariableCreationInterface();

            //Debug.Log("show ivs");
            ShowViewers(ivViewers, $"Independent Variables", EditorGuiHelper.IndependentVarColor);
            //Debug.Log("show dvs");
            ShowViewers(dvViewers, $"Dependent Variables", EditorGuiHelper.DependentVarColor);
            //Debug.Log("show pvs");
            ShowViewers(pvViewers, $"Participant Variables", EditorGuiHelper.ParticipantVarColor);

            EditorGUILayout.EndVertical();

        }

        void CheckVariablesAddedDeletedFromFactoryList(string factoryListName) {

            
            List<int> IndexesToDelete = new List<int>();
            
            SerializedProperty list = factoryProp.FindPropertyRelative(factoryListName);
            for (int index = 0; index < list.arraySize; index++) {
                SerializedProperty variableProperty = list.GetArrayElementAtIndex(index);
                
                string variablePropertyPath = variableProperty.propertyPath;
                if (allViewers.TryGetValue(variablePropertyPath, out VariableViewer existingViewer)) {
                    if (existingViewer.ReadyToDelete) {
                        IndexesToDelete.Add(index);
                        allViewers.Remove(variablePropertyPath);
                        RemoveFromSubList(variablePropertyPath);
                        existingViewer.Deleted = true;
                    }
                }
                else {
//                    Debug.Log($"Creating viewer for {variableProperty.type}, {variableProperty.displayName}");
                    string fieldName = nameof(Variable.VariableType);
                    //Debug.Log($"getting field {fieldName} ");
                    SerializedProperty typeProp = variableProperty.FindPropertyRelative(fieldName);
                    //Debug.Log($"typeProp name {typeProp.displayName}, type {typeProp.type}, intVal {typeProp.intValue}");
                    VariableType variableType = (VariableType) typeProp.intValue;
                    VariableViewer newViewer;
                    
                    switch (variableType) {
                        case VariableType.Independent:
                            newViewer = new IndependentVariableViewer(variableProperty);
                            break;
                        case VariableType.Dependent:
                            newViewer = new DependentVariableViewer(variableProperty);
                            break;
                        case VariableType.Participant:
                            newViewer = new ParticipantVariableViewer(variableProperty);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("variableType", variableType.ToString());
                    }
                    
                    allViewers.Add(variablePropertyPath, newViewer);
                    AddToSubList(variablePropertyPath, newViewer);
                }
            }

            //descending order to avoid changing indexes on deletion
            var descendingDeletionIndexes = IndexesToDelete.OrderByDescending(i => i);
            foreach (int i in descendingDeletionIndexes) {
                list.DeleteArrayElementAtIndex(i);
            }

            factoryProp.serializedObject.ApplyModifiedProperties();

        }

        void AddToSubList(string variablePropertyPath, VariableViewer newViewer) {
            switch (newViewer.VariableType) {
                case VariableType.Independent:
                    ivViewers.Add(variablePropertyPath, newViewer);
                    break;
                case VariableType.Dependent:
                    dvViewers.Add(variablePropertyPath, newViewer);
                    break;
                case VariableType.Participant:
                    pvViewers.Add(variablePropertyPath, newViewer);
                    break;
                case VariableType.ChooseType:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void RemoveFromSubList(string variablePropertyPath) {
            bool successfullyRemoved = false;
            
            if (ivViewers.Remove(variablePropertyPath)) successfullyRemoved = true;
            if (dvViewers.Remove(variablePropertyPath)) successfullyRemoved = true;
            if (pvViewers.Remove(variablePropertyPath)) successfullyRemoved = true;
            
            if (!successfullyRemoved) Debug.LogError($"Trying to remove {variablePropertyPath} but not found in lists");
        }
        

        void ShowVariableCreationInterface() {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            //GUILayout.Space((EditorGUI.indentLevel +1) * IndentSize);
            if (GUILayout.Button("Create Variable:")) {
                CreateNewVariable();
            }

            selectedVariableType = (VariableType) EditorGUILayout.EnumPopup(selectedVariableType);
            selectedDataType = (SupportedDataType) EditorGUILayout.EnumPopup(selectedDataType);
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            
        }

        void CreateNewVariable() {

            if (selectedVariableType == VariableType.ChooseType || selectedDataType == SupportedDataType.ChooseType) {
                Debug.LogWarning($"{TuxLog.Prefix} Need to select variable type and data type before creating a variable");
                return;
            }
            
            factory.AddNew(selectedVariableType, selectedDataType);
            
            selectedDataType = SupportedDataType.ChooseType;
            selectedVariableType = VariableType.ChooseType;
            serializedObject.Update();
        }


        void ShowViewers(Dictionary<string, VariableViewer> viewerDict, string title, Color background) {
            
            EditorGUILayout.LabelField($"{title}: {viewerDict.Count}", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorGuiHelper.MakeBackgroundStyle(background));
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUI.indentLevel++;
            
            if (viewerDict.Count == 0) {
                EditorGUILayout.LabelField("None");
            }
            
            foreach (KeyValuePair<string,VariableViewer> item in viewerDict) {
                item.Value.DrawInspector();
            }
            
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            
            
        }

        void ShowPreviewButton() {
            designFileTarget = target as ExperimentDesignFile2;
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
                designFileTarget.GetHasBlocks) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(trialRandomizationSubType);
                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        void ShowAdvancedOptions() {
            
            EditorGUILayout.BeginVertical(EditorGuiHelper.MakeBackgroundStyle(EditorGuiHelper.RedColor));
            
            ShowNameAndEditButton();
            
            EditorGUI.indentLevel++;

            EditorGUILayout.HelpBox("See documentation for information.", MessageType.Info);
            
            if (showAdvanced.boolValue) {
                
                ShowTrialTableOptions();
                EditorGUILayout.Space();
                
                ShowBlockOrderConfiguration();
                EditorGUILayout.Space();
                ShowSettingsFields();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
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

            EditorGUILayout.BeginVertical(EditorGuiHelper.AlternateColorBox);
            
            EditorGUILayout.LabelField("Manual Block Order",  EditorStyles.boldLabel);
            EditorGUI.indentLevel += 2;
            
            CheckValidBlockOrder();

            blockOrderFileList.DoLayoutList();
            
            DrawBlockConfigButtons();

            EditorGUILayout.Space();
            
            
            
            EditorGUI.indentLevel -= 2;
            
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }

        void DrawBlockConfigButtons() {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space((EditorGUI.indentLevel + 1) * IndentWidth);
            if (GUILayout.Button("Create New Block Order File")) {
                CreateNewBlockOrderDefinition();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space((EditorGUI.indentLevel + 1) * IndentWidth);
            if (GUILayout.Button("Clear List")) {
                designFileTarget.BlockOrderConfigurations.Clear();
                EditorUtility.SetDirty(target);
            }

            if (GUILayout.Button("Delete All Files")) {
                foreach (var config in designFileTarget.BlockOrderConfigurations) {
                    string path = AssetDatabase.GetAssetPath(config);
                    AssetDatabase.DeleteAsset(path);
                }

                designFileTarget.BlockOrderConfigurations.Clear();
            }

            EditorGUILayout.EndHorizontal();
        }

        void CheckValidBlockOrder() {

            IExperimentDesignFile designFile = serializedObject.targetObject as IExperimentDesignFile;
            if (designFile == null) return;
                
            if (!designFile.GetHasBlocks) return;
                
            if (!designFile.GetBlockOrderIsValid) {
                EditorGUILayout.HelpBox("A recent change in your design has invalidated any manual block order configurations. Please update them before running your experiment",
                    MessageType.Error); 
                EditorGUILayout.Space();
            }
        }

        void CreateNewBlockOrderDefinition() {
            
           if (!designFileTarget.HasBlocks) {
                EditorUtility.DisplayDialog("bmlTUX: Error Creating Block Order File", 
                    $"No block variables have been defined.\nYou need to define a block variable first.", "Ok");
                return;
           }
           
           BlockOrderDefinition newBlockOrderDefinition = CreateInstance<BlockOrderDefinition>();
           newBlockOrderDefinition.Init(designFileTarget);
           
           List<BlockOrderDefinition> orders = designFileTarget.GetBlockOrderConfigurations;
           orders.Add(newBlockOrderDefinition);
           EditorUtility.SetDirty(designFileTarget);
           try {
               string savePath = Path.GetDirectoryName(path: AssetDatabase.GetAssetPath(Selection.activeObject)) +
                                 "/New Block Order Definition.asset";
               string uniqueSavePath = EditorGuiHelper.GetUniqueName(savePath);
               AssetDatabase.CreateAsset(newBlockOrderDefinition, uniqueSavePath);
               AssetDatabase.SaveAssets();
           }
           catch (ArgumentNullException) {
               Debug.LogError($"{TuxLog.Prefix} Could not create BlockOrderDefinition. There is probably an error in variable definitions.");
               orders.Remove(newBlockOrderDefinition);
           }
            
           EditorUtility.FocusProjectWindow();
            
           Selection.activeObject = newBlockOrderDefinition;
           if (designFileTarget == null) throw new NullReferenceException("DesignFileNull");
           
           
            
                
                
            
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
                    DesignSaverEditorWindow.ShowWindow(Selection.activeObject as IExperimentDesignFile);
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
            if (showAdvanced.boolValue) {
                if (GUILayout.Button("Hide", GUILayout.Width(150))) {
                    showAdvanced.boolValue = false;
                }
            }
            else {
                if (GUILayout.Button("Show", GUILayout.Width(150))) {
                    showAdvanced.boolValue = true;
                }
            }

            EditorGUILayout.EndHorizontal();
            
        }


       
    }
}