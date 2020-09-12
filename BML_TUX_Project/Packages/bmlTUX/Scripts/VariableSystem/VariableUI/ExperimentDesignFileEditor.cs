using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Management.Instrumentation;
using bmlTUX.Scripts.ExperimentParts;
using bmlTUX.Scripts.UI.EditorUI;
using bmlTUX.Scripts.Utilities;
using bmlTUX.Scripts.VariableSystem.VariableTypes;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace bmlTUX.Scripts.VariableSystem.VariableUI {

    [CustomEditor(typeof(ExperimentDesignFile))]
    public class ExperimentDesignFileEditor : Editor {
        
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
        SerializedProperty showAdvanced;
        ExperimentDesignFile designFileTarget;
        List<VariableViewer> ivViewers;
        List<VariableViewer> dvViewers;
        List<VariableViewer> pvViewers;
        public List<VariableViewer> ListToDelete = new List<VariableViewer>();

        ReorderableList blockOrderFileList;

        public VariableType selectedVariableType;
        public SupportedDataType selectedDataType;
       
   
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
            showAdvanced = serializedObject.FindProperty(nameof(ExperimentDesignFile.ShowAdvancedEditor));
            
            InitializeBlockOrderList();

            CreateAllViewers();

            selectedDataType = factory.DataTypeToCreate;
            selectedVariableType = factory.VariableTypeToCreate;

        }

        void InitializeBlockOrderList() {
            blockOrderFileList = new ReorderableList(serializedObject,
                serializedObject.FindProperty(nameof(ExperimentDesignFile.BlockOrderConfigurations)), true, false, true, true);
            blockOrderFileList.drawHeaderCallback = rect => { EditorGUI.LabelField(rect, "Selected Configurations:"); };
            blockOrderFileList.drawElementCallback = (rect, index, isActive, isFocused) => {
                var element = blockOrderFileList.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, element, GUIContent.none);
            };
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

        void CreateAllViewers() {
            ivViewers =  new List<VariableViewer>();
            dvViewers = new List<VariableViewer>();
            pvViewers =  new List<VariableViewer>();
            
            CreateViewersFrom(nameof(VariableFactory.IndependentVariables), ivViewers);
            CreateViewersFrom( nameof(VariableFactory.DependentVariables), dvViewers);
            CreateViewersFrom(nameof(VariableFactory.ParticipantVariables), pvViewers);
        }

        void CreateViewersFrom(string variableListName, List<VariableViewer> viewerList) {
            SerializedProperty list = factoryProp.FindPropertyRelative(variableListName);
            
            for (int index = 0; index < list.arraySize; index++) {
                SerializedProperty variableProp = list.GetArrayElementAtIndex(index);
                viewerList.Add(new VariableViewer(this, variableProp, viewerList));
            }
        }


        void DeleteVariablesFlaggedForDeletion() {
           
            
            foreach (VariableViewer variableViewer in ListToDelete) {
                
                RemoveVariableFromFactory(variableViewer);
                variableViewer.inViewerList.Remove(variableViewer);
                
            }
            ListToDelete.Clear();

        }

        void RemoveVariableFromFactory(VariableViewer variableViewer) {
            
            
            
            SerializedProperty containingList;
            switch ((VariableType)variableViewer.VariableProperty.FindPropertyRelative(nameof(Variable.TypeOfVariable)).enumValueIndex) {
                case VariableType.Independent:
                    containingList = factoryProp.FindPropertyRelative(nameof(VariableFactory.IndependentVariables));
                    break;
                case VariableType.Dependent:
                    containingList = factoryProp.FindPropertyRelative(nameof(VariableFactory.DependentVariables));
                    break;
                case VariableType.Participant:
                    containingList = factoryProp.FindPropertyRelative(nameof(VariableFactory.ParticipantVariables));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            for (int i = 0; i < containingList.arraySize; i++) {
                var element = containingList.GetArrayElementAtIndex(i);
                string elementName = element.FindPropertyRelative(nameof(Variable.Name)).stringValue;
                string variableName = variableViewer.VariableProperty.FindPropertyRelative(nameof(Variable.Name))
                    .stringValue;
                if (elementName == variableName) {
                    containingList.DeleteArrayElementAtIndex(i);
                    Debug.Log($"Match found: elem {elementName}, variable to delete {variableName}"); 
                }
            }
        }


        void ShowVariableFactory() {
            
            if (factory == null) return;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Variables", EditorStyles.boldLabel);
            ShowVariableCreationInterface();


            GUIStyle variableStyle = new GUIStyle(GUI.skin.box) {
                                                                    normal = {background = MakeTex(variablePanelBackgroundColor)}
                                                                };
            
            EditorGUILayout.LabelField($"Independent Variables: {factory.IndependentVariables.Count}", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(variableStyle);
            EditorGUI.indentLevel++;
            ShowViewers(ivViewers);
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField($"Dependent Variables: {factory.DependentVariables.Count}", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(variableStyle);
            EditorGUI.indentLevel++;
            ShowViewers(dvViewers);
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField($"Participant Variables: {factory.ParticipantVariables.Count}", EditorStyles.boldLabel);
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

            selectedVariableType = (VariableType) EditorGUILayout.EnumPopup(selectedVariableType);
            selectedDataType = (SupportedDataType) EditorGUILayout.EnumPopup(selectedDataType);

            factoryProp.serializedObject.Update();
            factoryProp.FindPropertyRelative(nameof(VariableFactory.DataTypeToCreate)).enumValueIndex = (int) selectedDataType;
            factoryProp.FindPropertyRelative(nameof(VariableFactory.VariableTypeToCreate)).enumValueIndex = (int) selectedVariableType;
            factoryProp.serializedObject.ApplyModifiedProperties();            
            EditorGUILayout.EndHorizontal();
        }

        void CreateNewVariableAndViewer() {

            if (factory.VariableTypeToCreate == VariableType.ChooseType || factory.DataTypeToCreate == SupportedDataType.ChooseType) {
                Debug.LogWarning($"{TuxLog.Prefix} Need to select variable type and data type before creating a variable");
                return;
            }

            VariableType typeToCreate = factory.VariableTypeToCreate;
            Variable variable = factory.AddNew();
            Debug.Log($"VariableCreated with type {variable.TypeOfVariable}");
            
            switch (typeToCreate) {
                case VariableType.Independent:
                    CreateViewerFrom(nameof(VariableFactory.IndependentVariables), ivViewers);
                    break;
                case VariableType.Dependent:
                    CreateViewerFrom(nameof(VariableFactory.DependentVariables), dvViewers);
                    break;
                case VariableType.Participant:
                    CreateViewerFrom(nameof(VariableFactory.ParticipantVariables), pvViewers);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            selectedDataType = SupportedDataType.ChooseType;
            selectedVariableType = VariableType.ChooseType;
            
            serializedObject.Update();
        }
        
        void CreateViewerFrom(string variableRelativeName, List<VariableViewer> viewerList) {
            serializedObject.Update();
            SerializedProperty list = factoryProp.FindPropertyRelative(variableRelativeName);
          
            int index = list.arraySize-1;
            Debug.Log($"New Index {index} num vars = {list.arraySize}");
            SerializedProperty variableProp = list.GetArrayElementAtIndex(index);
            var name = variableProp.FindPropertyRelative(nameof(Variable.Name));
         
            VariableViewer variableViewer = new VariableViewer(this, variableProp, viewerList);
            viewerList.Add(variableViewer);
        }


        void ShowViewers(List<VariableViewer> dict) {
            if (CheckEmptyDict(dict)) return;
            foreach (VariableViewer item in dict) {
                item.UpdateView();
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

            EditorGUILayout.HelpBox("See documentation for information.", MessageType.Info);
            
            if (showAdvanced.boolValue) {
                
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
            
            EditorGUILayout.LabelField("Manual Block Order",  EditorStyles.boldLabel);
            EditorGUI.indentLevel += 2;
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space((EditorGUI.indentLevel + 1) * IndentWidth);
            if (GUILayout.Button("Create and Add New Block Order File", GUILayout.Width(240))) {
                CreateNewBlockOrderDefinition();
            }
            
            EditorGUILayout.EndHorizontal();
            
            blockOrderFileList.DoLayoutList();
            
            CheckValidBlockOrder();
            
            EditorGUI.indentLevel -= 2;
        }
        
        void CheckValidBlockOrder() {

            ExperimentDesignFile experimentDesignFile = serializedObject.targetObject as ExperimentDesignFile;
            if (experimentDesignFile == null) return;
                
            if (!experimentDesignFile.HasBlocks) return;
                
            if (!experimentDesignFile.BlockOrderIsValid) {
                EditorGUILayout.HelpBox("A recent change has invalidated your manual block order configurations. Please update them before running your experiment",
              
                    MessageType.Error); 
            }
        }

        void CreateNewBlockOrderDefinition() {
            
            ExperimentDesignFile experimentDesignFile = target as ExperimentDesignFile;
            if (experimentDesignFile == null) throw new NullReferenceException("Can't read experimental design from file check for errors");
            if (!experimentDesignFile.HasBlocks) {
                Debug.LogError($"{TuxLog.Prefix} Tried to make block order definition file, but no block variables have been defined!");
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
                    Debug.LogError($"{TuxLog.Prefix} Could not create BlockOrderDefinition. There is probably an error in variable definitions.");
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

    public class VariableViewerRegistry {
        List<VariableViewer> viewerList;
        SerializedProperty containingFactoryList;
    }
}