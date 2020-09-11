using System;
using bmlTUX.Scripts.ExperimentParts;
using bmlTUX.Scripts.VariableSystem.VariableTypes;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace bmlTUX.Scripts.VariableSystem.VariableUI {
    public class VariableViewer {
        readonly SerializedProperty variableProperty;
        public readonly SerializedProperty ContainingList;
        readonly ExperimentDesignFileEditor editor;
        
        const int DeleteButtonWidth     = 100;

        
        bool deleted;
        readonly VariableType type;
        public readonly int Index;
        readonly SerializedProperty expandSettingsProp;
        SerializedProperty valuesProperty;
        SerializedProperty probabilitiesProperty;
        SerializedProperty blockProperty;

        ReorderableList valuesList;
        ReorderableList probabilitiesList;
        SerializedObject serializedObject;

        public VariableViewer(ExperimentDesignFileEditor editor, SerializedProperty variableProperty, SerializedProperty containingList, int index) {
            
            this.editor = editor;
            this.serializedObject = variableProperty.serializedObject;
            this.variableProperty = variableProperty;
            ContainingList = containingList;
            Index = index;
            expandSettingsProp = variableProperty.FindPropertyRelative(nameof(Variable.ExpandSettings));
            type = (VariableType)variableProperty.FindPropertyRelative(nameof(Variable.TypeOfVariable)).enumValueIndex;
            
            switch (type) {
                case VariableType.Independent:
                    valuesProperty = variableProperty.FindPropertyRelative("Values");
                    probabilitiesProperty = variableProperty.FindPropertyRelative("Probabilities");
                    InitValuesList();
                    break;
                case VariableType.Participant:
                    valuesProperty = variableProperty.FindPropertyRelative("PossibleValues");
                    InitValuesList();
                    break;
            }
            
            
        }

        public void UpdateView() {
            if (deleted) EditorGUILayout.HelpBox("Error: Trying to show deleted variable", MessageType.Error);
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            AddCommonVariableProperties();
            
            EditorGUI.indentLevel++;
            
                
            switch (type) {
                case VariableType.Independent:
                    valuesProperty = variableProperty.FindPropertyRelative("Values");
                    probabilitiesProperty = variableProperty.FindPropertyRelative("Probabilities");
                    AddIndependentVariableProperties();
                    break;
                case VariableType.Dependent:
                    AddDependentVariableProperties();
                    break;
                case VariableType.Participant:
                    valuesProperty = variableProperty.FindPropertyRelative("PossibleValues");
                    AddParticipantVariableProperties();
                    break;
                case VariableType.ChooseType:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            

            EditorGUILayout.Space();
            AddDeleteButton();
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            
        }
        
             
        void AddParticipantVariableProperties() {
            if (expandSettingsProp.boolValue) {

                EditorGUI.indentLevel++;
                
                SerializedProperty constrainProperty = variableProperty.FindPropertyRelative("ConstrainValues");
                EditorGUILayout.PropertyField(constrainProperty);

                if (constrainProperty.boolValue) {
                    EditorGUILayout.LabelField("Possible Values");

                    DisplayValues();

                }
                EditorGUI.indentLevel--;

            }
        }
     
        
        void AddDependentVariableProperties() {
            if (expandSettingsProp.boolValue) {
                SerializedProperty defaultValueProperty = variableProperty.FindPropertyRelative("DefaultValue");
                EditorGUILayout.PropertyField(defaultValueProperty);
            }
        }

        void AddDeleteButton() {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Delete Variable", GUILayout.Width(DeleteButtonWidth))) {
                if (EditorUtility.DisplayDialog("Delete Variable", "Are you sure you want to delete variable?",
                    "Delete Variable", "Cancel")) {
                    editor.ListToDelete.Add(this);
                    deleted = true;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        void AddCommonVariableProperties() {

            int oldIndentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            EditorGUILayout.BeginHorizontal();
            SerializedProperty name = variableProperty.FindPropertyRelative(nameof(Variable.Name));
            EditorGUILayout.PropertyField(name, GUIContent.none );

            SerializedProperty variableDataType = variableProperty.FindPropertyRelative(nameof(Variable.DataType));
            SupportedDataType dataType = (SupportedDataType) variableDataType.enumValueIndex;
            EditorGUILayout.LabelField($"{dataType.ToString()}", GUILayout.Width(75));
            AddEditViewButton();
            
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel = oldIndentLevel;
            VariableNameValidator validator = new VariableNameValidator(name.stringValue);
            if (!validator.Valid) {
                EditorGUI.indentLevel++;
                EditorGUILayout.HelpBox(validator.InvalidReasons, MessageType.Error);
                EditorGUI.indentLevel--;
            }

        }

        void AddEditViewButton() {
            if (expandSettingsProp.boolValue) {
                if (GUILayout.Button("Hide Settings", GUILayout.Width(DeleteButtonWidth))) {
                    expandSettingsProp.boolValue = false;
                }
            }
            else {
                if (GUILayout.Button("Edit Settings", GUILayout.Width(DeleteButtonWidth))) {
                    expandSettingsProp.boolValue = true;
                }
            }
        }

        void AddIndependentVariableProperties() {
            
            if (expandSettingsProp.boolValue) {
                SerializedProperty block = variableProperty.FindPropertyRelative(nameof(IndependentVariable.Block));
                EditorGUILayout.PropertyField(block);

                SerializedProperty mixType = variableProperty.FindPropertyRelative(nameof(IndependentVariable.MixingType));
                EditorGUILayout.PropertyField(mixType);
            }
            
            
            AddIndependentVariableValueProperties();
            
        }
        
          
        void AddIndependentVariableValueProperties() {
            if (valuesProperty.arraySize == 0) {
                EditorGUILayout.HelpBox("No values", MessageType.Error);
            }
            if (expandSettingsProp.boolValue) {
                blockProperty = variableProperty.FindPropertyRelative(nameof(IndependentVariable.Block));
                
                DisplayValues();
                
                CheckMaxBlockPermutationsAllowed();
            }
        }


        void DisplayValues() {
            
            bool isCustomProbability = CheckIfCustomProbability();
            
            
            
            valuesList.DoLayoutList();
            
            EditorGUILayout.LabelField("test");
            if (isCustomProbability) {
                CheckForProbabilityArrayErrors();
                CalculateFinalProbability();
                CheckProbabilityErrors(true);
            }
           
        }

        void InitValuesList() {
            
            valuesList = new ReorderableList(serializedObject, valuesProperty, true, true, true, true);
            

            valuesList.drawHeaderCallback = rect => {
                bool isCustomProbability = CheckIfCustomProbability();
                Rect leftRect = new Rect(rect.position, rect.size);
                if (isCustomProbability) leftRect.width /= 2;
                EditorGUI.LabelField(leftRect, "Values");

                if (isCustomProbability) {
                    Rect rightRect = new Rect(leftRect) {
                        x = leftRect.width + 20
                    };
                    EditorGUI.LabelField(rightRect, $"Custom Prob. (Tot=)");
                }
            };

            valuesList.drawElementCallback = (rect, index, isActive, isFocused) => {
                bool isCustomProbability = CheckIfCustomProbability();
                Rect leftRect = DrawValueElement(index, rect, isCustomProbability);

                if (isCustomProbability) {
                    DrawProbabilityElement(index, leftRect);
                }
            };

        }

        void DrawProbabilityElement(int index, Rect leftRect) {
            if (index > probabilitiesProperty.arraySize-1) {
                Debug.Log("index too big");
                CheckForProbabilityArrayErrors();
            }

            var probabilityElement = probabilitiesProperty.GetArrayElementAtIndex(index);
            
            Rect rightRect = new Rect(leftRect) {
                x = leftRect.width + 30,
                width = 80
            };

            if (probabilitiesProperty.arraySize >= 1) {
                if (index == valuesProperty.arraySize - 1) {
                    EditorGUI.LabelField(rightRect, probabilityElement.floatValue + " (Auto)");
                }
                else {
                    EditorGUI.PropertyField(rightRect, probabilityElement, GUIContent.none);
                }
            }
        }

        void CalculateFinalProbability() {
            if (probabilitiesProperty.arraySize < 1) return;
            
            float runningTotal = 0;

            int n = probabilitiesProperty.arraySize;

            for (int i = 0; i < n; i++) {
                SerializedProperty prob = probabilitiesProperty.GetArrayElementAtIndex(i);
                
                if (i == n - 1) {
                    float remainder = 1 - runningTotal;
                    if (remainder >= 0) {
                        prob.floatValue = remainder;
                    }
                    else {
                        prob.floatValue = 0;
                    }
                }
                else {
                    runningTotal += prob.floatValue; 
                }

            }
            
            
        }

        Rect DrawValueElement(int index, Rect rect, bool isCustomProbability) {
            var element = valuesList.serializedProperty.GetArrayElementAtIndex(index);

            Rect leftRect = new Rect(rect.position, rect.size);
            if (isCustomProbability) leftRect.width /= 2;
            EditorGUI.PropertyField(leftRect, element, GUIContent.none);
            return leftRect;
        }

        bool CheckIfCustomProbability() {
            bool customProb;
            if (type == VariableType.Independent) {
                SerializedProperty mixType =
                    variableProperty.FindPropertyRelative(nameof(IndependentVariable.MixingType));
                customProb = ((VariableMixingType) mixType.enumValueIndex) == VariableMixingType.CustomProbability;
            }
            else {
                customProb = false;
            }

            return customProb;
        }
        
        void CheckForProbabilityArrayErrors() {
            while (probabilitiesProperty.arraySize < valuesProperty.arraySize) {
                probabilitiesProperty.arraySize++;
            }
            while (probabilitiesProperty.arraySize > valuesProperty.arraySize) {
                probabilitiesProperty.DeleteArrayElementAtIndex(probabilitiesProperty.arraySize-1);
                probabilitiesProperty.arraySize--;
            }
        }

        
        float GetRunningTotal(bool skipLast = false) {
            float runningTotal = 0;

            int n = probabilitiesProperty.arraySize;
            if (skipLast) n--;

            for (int i = 0; i < n; i++) {
                SerializedProperty prob = probabilitiesProperty.GetArrayElementAtIndex(i);

                runningTotal += prob.floatValue;
            }

            return runningTotal;
        }
        

        void CheckMaxBlockPermutationsAllowed() {
            if (!blockProperty.boolValue || valuesProperty.arraySize <= ExperimentDesign.MaxBlockPermutationsAllowed) return;
            EditorGUILayout.HelpBox("Too many Block Values for automatic permutation.\n" +
                                    "Must define possible Block orders manually using BlockOrderDefinition files.\n" +
                                    "See Docs.",
              
                                    MessageType.Warning);
        }
        
        void CheckProbabilityErrors(bool customProb) {
            
            if (!customProb || probabilitiesProperty.arraySize == 0) return;

            float runningTotal = GetRunningTotal();
            string direction = "";
            float remainder = 1 - runningTotal;
            if (Math.Abs(remainder) > 0.01f && probabilitiesProperty.arraySize > 0) {
                if (runningTotal > 1) direction = " (too high)";
                if (runningTotal < 1) direction = " (too low)";

                EditorGUILayout.HelpBox($"Total = {runningTotal}{direction}", MessageType.Error );
            }
       
        }

        
    }
    
    
    
}