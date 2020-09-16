using System;
using bmlTUX.Scripts.ExperimentParts;
using bmlTUX.Scripts.VariableSystem.VariableTypes;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Analytics;

namespace bmlTUX.Scripts.VariableSystem.VariableUI {
    public class IndependentVariableViewer : VariableViewerWithValues {
        
        SerializedProperty probabilitiesProperty;
        bool waitingToClearAllValues;
        
        public IndependentVariableViewer(SerializedProperty variableProperty) 
            : base(variableProperty, VariableType.Independent) {
            probabilitiesProperty = variableProperty.FindPropertyRelative("Probabilities");
        }

        void AddProbability() {
            probabilitiesProperty.arraySize++;
        }
        
        void RemoveProbability(int index) {
            if (index < probabilitiesProperty.arraySize) probabilitiesProperty.DeleteArrayElementAtIndex(index);
        }
        
        protected override void DrawVariableSpecificInspector() {
            
            SerializedProperty blockProperty = variableProperty.FindPropertyRelative(nameof(IndependentVariable.Block));
            CheckMaxBlockPermutationsAllowed(blockProperty.boolValue);
            
            if (ExpandSettingsProp.boolValue) {
                
                EditorGUILayout.PropertyField(blockProperty);
                
                SerializedProperty mixType = variableProperty.FindPropertyRelative(nameof(IndependentVariable.MixingType));
                VariableMixingType op = (VariableMixingType) mixType.intValue;
                op = (VariableMixingType)EditorGUILayout.EnumPopup("Mixing Type", op);
                mixType.intValue = (int) op;

            }
            
            if (valuesProperty.arraySize == 0) {
                variableValidationResults.AddWarning("No values");
            }
            if (ExpandSettingsProp.boolValue) {
                bool isCustomProbability = CheckIfCustomProbability();
            
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                valuesList.DoLayoutList();
                EditorGUILayout.Space();
                
                if (isCustomProbability) {
                    CalculateFinalProbability();
                    CheckProbabilityErrors(true);
                }
            }

            if (waitingToClearAllValues) {
                int n = valuesProperty.arraySize;
                for (int i = 0; i < n; i++) {
                    RemoveValue(0);
                    RemoveProbability(0);
                }

                waitingToClearAllValues = false;
            }
        }
        
        
        void CheckMaxBlockPermutationsAllowed(bool isBlockVariable) {
            
            if (!isBlockVariable || valuesProperty.arraySize <= ExperimentDesign.MaxBlockPermutationsAllowed) return;

            
            IExperimentDesignFile iExperimentDesignFile = variableProperty.serializedObject.targetObject as IExperimentDesignFile;
            if (iExperimentDesignFile == null) return;
            
            if (iExperimentDesignFile.GetBlockOrderConfigurations.Count == 0) {
                variableValidationResults.AddError(
                                        "Too many Block Values for automatic permutation. " +
                                        "Must define possible Block orders manually using BlockOrderDefinition files. " +
                                        "Please see docs.");
            }

            if (!iExperimentDesignFile.GetBlockOrderIsValid) {
                variableValidationResults.AddWarning("A recent change has invalidated your manual block order configurations. " +
                                                   "Please update them before running your experiment"); 
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
        
        void CheckProbabilityErrors(bool customProb) {
            
            if (!customProb || probabilitiesProperty.arraySize == 0) return;

            float runningTotal = GetRunningTotal();
            string direction = "";
            float remainder = 1 - runningTotal;
            if (Math.Abs(remainder) > 0.01f && probabilitiesProperty.arraySize > 0) {
                if (runningTotal > 1) direction = " (too high)";
                if (runningTotal < 1) direction = " (too low)";

                variableValidationResults.AddError($"Custom Probabilities Error: Total = {runningTotal}{direction}");
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
        
        bool CheckIfCustomProbability() {
            bool customProb = false;
            
            VariableMixingType mixingType = (VariableMixingType)variableProperty.FindPropertyRelative(nameof(IndependentVariable.MixingType)).intValue;
            customProb = mixingType == VariableMixingType.CustomProbability;
            
            
            return customProb;
        }

        protected override void AddValueElement() {
            AddValue();
            AddProbability();
        }

        protected override void RemoveValueElement(ReorderableList list) {
            RemoveValue(list.index);
            RemoveProbability(list.index);
        }

        protected override void DrawValueElements(Rect rect, int index) {

            bool isCustomProbability = CheckIfCustomProbability();

            Rect leftRect = new Rect(rect.position, rect.size);
            if (isCustomProbability) leftRect.width *= 2f/3f;
            DrawValueElement(index, leftRect);

            if (isCustomProbability) {
                DrawProbabilityElement(index, leftRect);
            }
        }

        protected override void DrawValuesHeader(Rect rect) {
            bool isCustomProbability = CheckIfCustomProbability();
            Rect leftRect = new Rect(rect);
            leftRect.position = new Vector2(leftRect.position.x +10, leftRect.position.y);
            if (isCustomProbability) leftRect.width *= 2f/3f;
            EditorGUI.LabelField(leftRect, "Values");

            Rect clearRect = new Rect(leftRect);
            clearRect.width = 40;
            clearRect.position = new Vector2(leftRect.width-4, leftRect.position.y);
            if (GUI.Button(clearRect, "Clear")) {
                waitingToClearAllValues = true;
            }
            
            if (isCustomProbability) {
                Rect rightRect = new Rect(leftRect) {
                    x = leftRect.width + leftRect.position.x - 10
                };
                EditorGUI.LabelField(rightRect, $"Probs Tot:{GetRunningTotal()}");
            }

            
                
           
        }

        void DrawProbabilityElement(int index, Rect leftRect) {
            if (index > probabilitiesProperty.arraySize-1) {
                Debug.Log("index too big");
            }

            SerializedProperty probabilityElement = probabilitiesProperty.GetArrayElementAtIndex(index);
            
            Rect rightRect = new Rect(leftRect) {
                x = leftRect.width + leftRect.position.x,
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

    }
}