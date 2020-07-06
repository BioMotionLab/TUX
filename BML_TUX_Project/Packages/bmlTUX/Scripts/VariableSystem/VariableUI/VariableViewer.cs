using System;
using bmlTUX.Scripts.ExperimentParts;
using bmlTUX.Scripts.VariableSystem.VariableTypes;
using UnityEditor;
using UnityEngine;

namespace bmlTUX.Scripts.VariableSystem.VariableUI {
    public class VariableViewer {
        readonly SerializedProperty variableProperty;
        public readonly SerializedProperty ContainingList;
        readonly ExperimentDesignFileEditor editor;
        
        const int ValueButtonWidth      = 25;
        const int ValueProbabilityWidth = 70;
        const int DeleteButtonWidth     = 100;
        const int IndentSize            = 10;
        const int ValueIndentLevel      = 2;

        
        
        bool deleted;
        readonly VariableType type;
        public readonly int Index;
        readonly SerializedProperty expandSettingsProp;
        SerializedProperty valuesProperty;
        SerializedProperty probabilitiesProperty;
        SerializedProperty blockProperty;

        public VariableViewer(ExperimentDesignFileEditor editor,  SerializedProperty variableProperty, SerializedProperty containingList, int index) {
            this.editor = editor;
            this.variableProperty = variableProperty;
            ContainingList = containingList;
            Index = index;
            expandSettingsProp = variableProperty.FindPropertyRelative(nameof(Variable.ExpandSettings));
            type = (VariableType)variableProperty.FindPropertyRelative(nameof(Variable.TypeOfVariable)).enumValueIndex;
            
        }

        public void Show() {
            if (deleted) EditorGUILayout.HelpBox("Error: Trying to show deleted variable", MessageType.Error);
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            AddCommonVariableProperties();
            
            EditorGUI.indentLevel++;
            
            
                
            switch (type) {
                case VariableType.Independent:
                    AddIndependentVariableProperties();
                    break;
                case VariableType.Dependent:
                    AddDependentVariableProperties();
                    break;
                case VariableType.Participant:
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
            valuesProperty = variableProperty.FindPropertyRelative("PossibleValues");
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
            valuesProperty = variableProperty.FindPropertyRelative("Values");
            if (valuesProperty.arraySize == 0) {
                EditorGUILayout.HelpBox("No values", MessageType.Error);
            }

            if (expandSettingsProp.boolValue) {
                probabilitiesProperty = variableProperty.FindPropertyRelative("Probabilities");
                blockProperty = variableProperty.FindPropertyRelative(nameof(IndependentVariable.Block));
                
                DisplayValues();
                
                EditorGUI.indentLevel--;
                CheckMaxBlockPermutationsAllowed();
            }
        }


        void DisplayValues() {
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Values");
            bool customProb;

            if (type == VariableType.Independent) {
                SerializedProperty mixType =
                    variableProperty.FindPropertyRelative(nameof(IndependentVariable.MixingType));
                customProb = ((VariableMixingType) mixType.enumValueIndex) == VariableMixingType.CustomProbability;
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
                if (AddMinusButton(i)) break;
                SerializedProperty value = valuesProperty.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(value, GUIContent.none);
                if (customProb) AddCustomProbabilities(true, i);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space((oldIndentLevel + ValueIndentLevel) * 10);
            AddPlusButton();
            EditorGUILayout.LabelField("");
            if (customProb) AddTotalProbability(true);
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel = oldIndentLevel;

            EditorGUI.indentLevel--;
        }
         
         
        void AddPlusButton() {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUI.indentLevel * IndentSize);
            if (GUILayout.Button("+",  GUILayout.Width(ValueButtonWidth))) {
                int lastIndex = valuesProperty.arraySize;
                if (lastIndex < 0) lastIndex = 0;

                valuesProperty.InsertArrayElementAtIndex(lastIndex);
                if (probabilitiesProperty != null) {
                    probabilitiesProperty.arraySize++;
                    CheckForProbabilityArrayErrors();
                }
                
                

                //make last one input equal to zero;
                if (probabilitiesProperty?.arraySize > 1)
                    probabilitiesProperty.GetArrayElementAtIndex(probabilitiesProperty.arraySize - 2).floatValue = 0;
            }
            EditorGUILayout.EndHorizontal();
        }

        void CheckForProbabilityArrayErrors() {
            while (probabilitiesProperty.arraySize < valuesProperty.arraySize) {
                probabilitiesProperty.arraySize++;
            }
        }


        bool AddMinusButton(int i) {
            if (GUILayout.Button("-", GUILayout.Width(ValueButtonWidth))) {
                valuesProperty.DeleteArrayElementAtIndex(i);
                if (probabilitiesProperty?.arraySize > 0) probabilitiesProperty?.DeleteArrayElementAtIndex(i);
                return true;
            }
            return false;
        }


        void AddCustomProbabilities(bool customProb, int i) {
            
            SerializedProperty prob = probabilitiesProperty.GetArrayElementAtIndex(i);
            if (customProb && probabilitiesProperty.arraySize >= 1) {
                if (i == valuesProperty.arraySize - 1) {
                    float runningTotalWithoutLast = GetRunningTotal(true);
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
        
        void AddTotalProbability(bool customProb) {
            
            if (!customProb || probabilitiesProperty.arraySize == 0) return;

            float runningTotal = GetRunningTotal();
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