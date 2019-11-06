using System;
using System.Collections.Generic;
using BML_TUX.Scripts.ExperimentParts;
using BML_TUX.Scripts.VariableSystem.VariableTypes;
using UnityEditor;
using UnityEngine;

namespace BML_TUX.Scripts.VariableSystem.VariableUI {
    public class VariableViewer {
        public readonly SerializedProperty variableProp;
        public readonly SerializedProperty containingList;
        readonly ExperimentDesignFileEditor editor;
        
        const int ValueButtonWidth      = 25;
        const int ValueProbabilityWidth = 70;
        const int DeleteButtonWidth     = 100;
        const int IndentSize            = 10;
        const int ValueIndentLevel      = 2;

        
        
        bool deleted;
        VariableType type;
        public int index;
        SerializedProperty expandSettingsProp;
        SerializedProperty valuesProperty;
        SerializedProperty probabilitiesProperty;
        SerializedProperty blockProperty;

        public VariableViewer(ExperimentDesignFileEditor editor,  SerializedProperty variableProp, SerializedProperty containingList, int index, bool showSettings = true) {
            this.editor = editor;
            this.variableProp = variableProp;
            this.containingList = containingList;
            this.index = index;
            expandSettingsProp = variableProp.FindPropertyRelative(nameof(Variable.ExpandSettings));
            type = (VariableType)variableProp.FindPropertyRelative(nameof(Variable.TypeOfVariable)).enumValueIndex;
        }

        public void Show() {
            if (deleted) EditorGUILayout.HelpBox("Error: Trying to show deleted variable", MessageType.Error);
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            AddCommonVariableProperties(variableProp);
            
            EditorGUI.indentLevel++;
            
            
                
            switch (type) {
                case VariableType.Independent:
                    AddIndependentVariableProperties(variableProp);
                    break;
                case VariableType.Dependent:
                    AddDependentVariableProperties(variableProp);
                    break;
                case VariableType.Participant:
                    AddParticipantVariableProperties(variableProp);
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
        
             
        void AddParticipantVariableProperties(SerializedProperty variableProperty) {

            if (expandSettingsProp.boolValue) {

                EditorGUI.indentLevel++;

                SerializedProperty valuesProperty = variableProperty.FindPropertyRelative("PossibleValues");
                SerializedProperty constrainProperty = variableProperty.FindPropertyRelative("ConstrainValues");

                EditorGUILayout.PropertyField(constrainProperty);

                if (constrainProperty.boolValue) {
                    EditorGUILayout.LabelField("Possible Values");

                    DisplayValues(VariableType.Participant, variableProperty, valuesProperty, null);

                }

                EditorGUI.indentLevel--;

            }
        }
     
        
        void AddDependentVariableProperties(SerializedProperty variableProperty) {
            if (expandSettingsProp.boolValue) {
                SerializedProperty defaultValueProperty = variableProperty.FindPropertyRelative("DefaultValue");
                EditorGUILayout.PropertyField(defaultValueProperty);
            }
        }

        void AddDeleteButton() {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Delete Variable", GUILayout.Width(DeleteButtonWidth))) {
                editor.listToDelete.Add(this);
                deleted = true;
            }
            EditorGUILayout.EndHorizontal();
        }

        void AddCommonVariableProperties(SerializedProperty variableProperty) {

            EditorGUILayout.BeginHorizontal();
            SerializedProperty name = variableProperty.FindPropertyRelative(nameof(Variable.Name));
            EditorGUILayout.PropertyField(name, GUIContent.none );

            SerializedProperty variableDataType = variableProperty.FindPropertyRelative(nameof(Variable.DataType));
            SupportedDataType dataType = (SupportedDataType) variableDataType.enumValueIndex;
            EditorGUILayout.LabelField($"Type: {dataType.ToString()}");
            
            AddEditViewButton();
            
            EditorGUILayout.EndHorizontal();
            
            
            VariableNameValidator validator = new VariableNameValidator(name.stringValue);
            if (!validator.Valid) {
                EditorGUI.indentLevel++;
                EditorGUILayout.HelpBox(validator.InvalidReasons, MessageType.Error);
                EditorGUI.indentLevel--;
            }

        }

        void AddEditViewButton() {
            if (expandSettingsProp.boolValue) {
                if (GUILayout.Button("Hide settings", GUILayout.Width(DeleteButtonWidth))) {
                    expandSettingsProp.boolValue = false;
                    Debug.Log("collapsing view");
                }
            }
            else {
                if (GUILayout.Button("Edit settings", GUILayout.Width(DeleteButtonWidth))) {
                    expandSettingsProp.boolValue = true;
                    Debug.Log("expanding view");
                }
            }
        }

        void AddIndependentVariableProperties(SerializedProperty independentVariableProperty) {

            if (expandSettingsProp.boolValue) {
                SerializedProperty block = independentVariableProperty.FindPropertyRelative(nameof(IndependentVariable.Block));
                EditorGUILayout.PropertyField(block);

                SerializedProperty mixType = independentVariableProperty.FindPropertyRelative(nameof(IndependentVariable.MixingType));
                EditorGUILayout.PropertyField(mixType);
            }
            
            
            AddIndependentVariableValueProperties(independentVariableProperty);
            
        }
        
          
        void AddIndependentVariableValueProperties(SerializedProperty variableProperty) {
            valuesProperty = variableProperty.FindPropertyRelative("Values");
            if (valuesProperty.arraySize == 0) {
                EditorGUILayout.HelpBox("No values", MessageType.Error);
            }

            if (expandSettingsProp.boolValue) {
                probabilitiesProperty = variableProperty.FindPropertyRelative("Probabilities");
                blockProperty = variableProperty.FindPropertyRelative(nameof(IndependentVariable.Block));
                
                DisplayValues(VariableType.Independent, variableProperty, valuesProperty, probabilitiesProperty);
                
                EditorGUI.indentLevel--;
                CheckMaxBlockPermutationsAllowed(blockProperty, valuesProperty);
            }
        }

        
         void DisplayValues(VariableType type, SerializedProperty variableProperty, SerializedProperty valuesProperty,
                           SerializedProperty probabilitiesProperty) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Values");
            bool customProb;
            
            if (type == VariableType.Independent) {
                SerializedProperty mixType = variableProperty.FindPropertyRelative(nameof(IndependentVariable.MixingType));
                customProb = (VariableMixingType) mixType.enumValueIndex == VariableMixingType.CustomProbability;
            }
            else {
                customProb = false;
            }
            if (customProb) EditorGUILayout.LabelField("Probability");
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel++;
            EditorGUI.indentLevel++;

            int oldIndentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            for (int i = 0; i < valuesProperty.arraySize; i++) {
                EditorGUILayout.BeginHorizontal();

                GUILayout.Space((oldIndentLevel + ValueIndentLevel) * IndentSize);
                if (AddMinusButton(valuesProperty, probabilitiesProperty, i)) break;
                SerializedProperty value = valuesProperty.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(value, GUIContent.none);
                if (customProb) AddCustomProbabilities(valuesProperty, probabilitiesProperty, customProb, i);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space((oldIndentLevel + ValueIndentLevel) * 10);
            AddPlusButton(valuesProperty, probabilitiesProperty);
            EditorGUILayout.LabelField("");
            if (customProb) AddTotalProbability(customProb, probabilitiesProperty);
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel = oldIndentLevel;

            EditorGUI.indentLevel--;
        }
         
         
        static void AddPlusButton(SerializedProperty valuesProperty,
                                  SerializedProperty probabilitiesProperty) {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUI.indentLevel * IndentSize);
            if (GUILayout.Button("+",  GUILayout.Width(ValueButtonWidth))) {
                int lastIndex = valuesProperty.arraySize;
                if (lastIndex < 0) lastIndex = 0;
                probabilitiesProperty?.InsertArrayElementAtIndex(lastIndex);
                valuesProperty.InsertArrayElementAtIndex(lastIndex);

                //make last one input equal to zero;
                if (probabilitiesProperty?.arraySize > 1)
                    probabilitiesProperty.GetArrayElementAtIndex(probabilitiesProperty.arraySize - 2).floatValue = 0;
            }
            EditorGUILayout.EndHorizontal();
        }
        
        
        bool AddMinusButton(SerializedProperty valuesProperty, SerializedProperty probabilitiesProperty, int i) {
            if (GUILayout.Button("-", GUILayout.Width(ValueButtonWidth))) {
                valuesProperty.DeleteArrayElementAtIndex(i);
                if (probabilitiesProperty?.arraySize > 0) probabilitiesProperty?.DeleteArrayElementAtIndex(i);
                return true;
            }
            return false;
        }


        static void AddCustomProbabilities(SerializedProperty valuesProperty, SerializedProperty probabilitiesProperty,
                                           bool               customProb,     int                i) {
            
            SerializedProperty prob = probabilitiesProperty.GetArrayElementAtIndex(i);
            if (customProb && probabilitiesProperty.arraySize >= 1) {
                if (i == valuesProperty.arraySize - 1) {
                    float runningTotalWithoutLast = GetRunningTotal(probabilitiesProperty, true);
                    float remainder = 1 - runningTotalWithoutLast;
                    if (remainder >= 0) {
                        prob.floatValue = remainder;
                    }
                    else {
                        prob.floatValue = 0;
                    }

                    EditorGUILayout.LabelField(prob.floatValue + " (Auto)", GUILayout.Width(ValueProbabilityWidth));
                }
                else {
                    EditorGUILayout.PropertyField(prob, GUIContent.none, GUILayout.Width(ValueProbabilityWidth));
                }
            }
        }

        static float GetRunningTotal(SerializedProperty probabilitiesProperty, bool skipLast = false) {
            float runningTotal = 0;

            int n = probabilitiesProperty.arraySize;
            if (skipLast) n--;

            for (int i = 0; i < n; i++) {
                SerializedProperty prob = probabilitiesProperty.GetArrayElementAtIndex(i);

                runningTotal += prob.floatValue;
            }

            return runningTotal;
        }
        

        static void CheckMaxBlockPermutationsAllowed( SerializedProperty block,
                                                      SerializedProperty valuesProperty) {
            if (!block.boolValue || valuesProperty.arraySize <= ExperimentDesign.MaxBlockPermutationsAllowed) return;
            EditorGUILayout.HelpBox("Too many Block Values for automatic permutation.\n" +
                                    "Must define possible Block orders manually using BlockOrderDefinition files.\n" +
                                    "See Docs.",
              
                                    MessageType.Warning);
        }
        
        static void AddTotalProbability(bool customProb, SerializedProperty probabilitiesProperty) {
            
            if (!customProb || probabilitiesProperty.arraySize == 0) return;

            float runningTotal = GetRunningTotal(probabilitiesProperty);
            string direction = "";
            float remainder = 1 - runningTotal;
            if (Math.Abs(remainder) > 0.01f && probabilitiesProperty.arraySize > 0) {
                if (runningTotal > 1) direction = " (too high)";
                if (runningTotal < 1) direction = " (too low)";

                EditorGUILayout.HelpBox($"Total = {runningTotal}{direction}", MessageType.Error );
            }
            else {
                EditorGUILayout.LabelField($"Total = {runningTotal}{direction}", GUILayout.Width(ValueProbabilityWidth));
            }
        }

        
    }
    
    
    
}