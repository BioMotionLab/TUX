using System;
using System.Collections.Generic;
using System.Linq;
using BML_ExperimentToolkit.Scripts.ExperimentParts;
using BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes;
using BML_Utilities;
using UnityEditor;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.VariableUI {
    /// <summary>
    /// A collection of functions to help draw variable drawers
    /// </summary>
    public static class VariableDrawerHelpers {
        const float LineHeight = 20f;

        /// <summary>
        /// Adds all properties to independent variables
        /// </summary>
        /// <param name="mainPosition"></param>
        /// <param name="mainProperty"></param>
        /// <returns></returns>
        public static float AddAllIndependentVariableProperties(Rect mainPosition, SerializedProperty mainProperty) {
            mainProperty.serializedObject.Update();
            
            GuiLayoutRect layoutRect = new GuiLayoutRect(LineHeight, mainPosition);
            
            int oldIndentLevel = EditorGUI.indentLevel;

            AddVariableProperties(layoutRect, mainProperty);
            AddIndependentVariableProperties(layoutRect, mainProperty);
            AddIndependentVariableValueProperties(layoutRect, mainProperty);

            EditorGUI.indentLevel = oldIndentLevel;
 
            mainProperty.serializedObject.ApplyModifiedProperties();
            
            return layoutRect.FinalHeight;
        }

        /// <summary>
        /// Adds all properties to Dependent variables
        /// </summary>
        /// <param name="mainPosition"></param>
        /// <param name="mainProperty"></param>
        /// <returns></returns>
        public static float AddAllDependentVariableProperties(Rect mainPosition, SerializedProperty mainProperty) {
            mainProperty.serializedObject.Update();
            
            GuiLayoutRect layoutRect = new GuiLayoutRect(LineHeight, mainPosition);

            int oldIndentLevel = EditorGUI.indentLevel;

            AddVariableProperties(layoutRect, mainProperty);
            AddDependentVariableValueProperties(layoutRect, mainProperty);

            EditorGUI.indentLevel = oldIndentLevel;
            
            mainProperty.serializedObject.ApplyModifiedProperties();
            
            return layoutRect.FinalHeight;
        }

        /// <summary>
        /// Adds all properties to Dependent variables
        /// </summary>
        /// <param name="mainPosition"></param>
        /// <param name="mainProperty"></param>
        /// <returns></returns>
        public static float AddAllParticipantVariableProperties(Rect mainPosition, SerializedProperty mainProperty) {
            mainProperty.serializedObject.Update();
            
            GuiLayoutRect layoutRect = new GuiLayoutRect(LineHeight, mainPosition);
            
            int oldIndentLevel = EditorGUI.indentLevel;

            AddVariableProperties(layoutRect, mainProperty);
            AddParticipantVariableValueProperties(layoutRect, mainProperty);

            EditorGUI.indentLevel = oldIndentLevel;
            
            mainProperty.serializedObject.ApplyModifiedProperties();
            return layoutRect.FinalHeight;
        }

        /// <summary>
        /// Adds Value display to dependent variable properties
        /// </summary>
        /// <param name="layoutRect"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        static void AddDependentVariableValueProperties(GuiLayoutRect layoutRect, SerializedProperty property) {
            SerializedProperty defaultValueProperty = property.FindPropertyRelative("DefaultValue");
            EditorGUI.PropertyField(layoutRect.NextLine, defaultValueProperty);
        }

        
        static void AddParticipantVariableValueProperties(GuiLayoutRect layoutRect, SerializedProperty property) {
            SerializedProperty valuesProperty = property.FindPropertyRelative("PossibleValues");
            SerializedProperty constrainProperty = property.FindPropertyRelative("ConstrainValues");

            EditorGUI.PropertyField(layoutRect.NextLine, constrainProperty);


            if (constrainProperty.boolValue) {
                EditorGUI.LabelField(layoutRect.NextLine, "Possible Values");

                const float indentAmt = 40f;
                const float minusWidth = 20f;
                const float minusHeight = 14f;
                float x = indentAmt + layoutRect.CurrentLine.x;
                const float yPadding = (LineHeight - minusHeight) / 2;


                for (int i = 0; i < valuesProperty.arraySize; i++) {

                    Rect valuesBaseRect = layoutRect.NextLine;
                    
                    Rect minusRect = new Rect(x, valuesBaseRect.y + yPadding, minusWidth, minusHeight);
                    Rect valuesRect = new Rect(x + minusWidth, valuesBaseRect.y, 0.5f * valuesBaseRect.width,
                                               valuesBaseRect.height);

                    //Minus button
                    if (GUI.Button(minusRect, "-")) {
                        valuesProperty.DeleteArrayElementAtIndex(i);
                        break;
                    }

                    SerializedProperty value = valuesProperty.GetArrayElementAtIndex(i);
                    EditorGUI.PropertyField(valuesRect, value, GUIContent.none);

                }


                Rect valuesFooterBaseRect = layoutRect.NextLine;
                //plus button
                Rect plusRect = new Rect(x, valuesFooterBaseRect.y + yPadding, minusWidth, minusHeight);
                if (GUI.Button(plusRect, "+")) {
                    int lastIndex = valuesProperty.arraySize;
                    if (lastIndex < 0) lastIndex = 0;

                    valuesProperty.InsertArrayElementAtIndex(lastIndex);
                }
            }

            property.serializedObject.ApplyModifiedProperties();

        }


        /// <summary>
        /// Adds the properties for all variables
        /// </summary>
        /// <param name="layoutRect"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        static void AddVariableProperties(GuiLayoutRect layoutRect, SerializedProperty property) {
            
            SerializedProperty name = property.FindPropertyRelative(nameof(Variable.Name));
            EditorGUI.PropertyField(layoutRect.NextLine, name, GUIContent.none);

            VariableNameValidator validator = new VariableNameValidator(name.stringValue);
            if (!validator.Valid) {
                Rect warningBoxRect = layoutRect.NextLines(3);
                EditorGUI.HelpBox(warningBoxRect, validator.InvalidReasons, MessageType.Error);
                
            }
            
            SerializedProperty variableDataType = property.FindPropertyRelative(nameof(Variable.DataType));
            SupportedDataTypes dataType = (SupportedDataTypes) variableDataType.enumValueIndex;
            EditorGUI.LabelField(layoutRect.NextLine, $"Data Type: {dataType.ToString()}");

            EditorGUI.indentLevel++;

            SerializedProperty variableType = property.FindPropertyRelative(nameof(Variable.TypeOfVariable));
            VariableType varType = (VariableType) variableType.enumValueIndex;
            EditorGUI.LabelField(layoutRect.NextLine, $"Variable Type: {varType.ToString()}");
            
        }

        /// <summary>
        /// Adds the properties for independent variables
        /// </summary>
        /// <param name="layoutRect"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        static void AddIndependentVariableProperties(GuiLayoutRect layoutRect, SerializedProperty property) {
            
            SerializedProperty block = property.FindPropertyRelative(nameof(IndependentVariable.Block));
            EditorGUI.PropertyField(layoutRect.NextLine, block);
            
            
            

            SerializedProperty mixType =
                property.FindPropertyRelative(nameof(IndependentVariable.MixingTypeOfVariable));
            EditorGUI.PropertyField(layoutRect.NextLine, mixType);
        }

        /// <summary>
        /// Adds display of values for independent variables
        /// </summary>
        /// <param name="layoutRect"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        static void AddIndependentVariableValueProperties(GuiLayoutRect layoutRect, SerializedProperty property) {
            SerializedProperty valuesProperty = property.FindPropertyRelative("Values");
            SerializedProperty probabilitiesProperty = property.FindPropertyRelative("Probabilities");


            Rect valueLabelBaseRect = layoutRect.NextLine;
            
            EditorGUI.LabelField(valueLabelBaseRect, "Values");

            
            
            const float indentAmt = 40f;
            const float minusWidth = 20f;
            const float minusHeight = 14f;
            const float customProbabilityWidth = 180f;
            float x = indentAmt + layoutRect.CurrentLine.x;
            const float yPadding = (LineHeight - minusHeight) / 2;

            //Debug.Log($"enum value : {mixType.enumValueIndex} {(VariableMixingType)mixType.enumValueIndex}");

            float probValuesWidth = 0;
            SerializedProperty mixType =
                property.FindPropertyRelative(nameof(IndependentVariable.MixingTypeOfVariable));
            bool customProb = (VariableMixingType) mixType.enumValueIndex == VariableMixingType.CustomProbability;
            
            
            
            
            if (customProb) {
                //Debug.Log("custom probabilities");
                probValuesWidth = customProbabilityWidth;

                
                
                Rect probLabel = new Rect(valueLabelBaseRect.width - probValuesWidth, 
                                          valueLabelBaseRect.y, 
                                          probValuesWidth,
                                          valueLabelBaseRect.height);
                EditorGUI.LabelField(probLabel, "Probability");
            }


            for (int i = 0; i < valuesProperty.arraySize; i++) {

                Rect valueBaseRect = layoutRect.NextLine;
                
                Rect minusRect = new Rect(x, valueBaseRect.y + yPadding, minusWidth, minusHeight);
                Rect valuesRect = new Rect(x + minusWidth, valueBaseRect.y, 0.5f * valueBaseRect.width, valueBaseRect.height);
                Rect customProbabilityValuesRect = new Rect(valueBaseRect.width - probValuesWidth, valueBaseRect.y,
                    probValuesWidth,
                    valueBaseRect.height);

                //Minus button
                if (GUI.Button(minusRect, "-")) {
                    valuesProperty.DeleteArrayElementAtIndex(i);
                    probabilitiesProperty.DeleteArrayElementAtIndex(i);
                    //Debug.Log($"Deleted element. Size now {valuesProperty.arraySize}");
                    break;
                }

                SerializedProperty value = valuesProperty.GetArrayElementAtIndex(i);
                EditorGUI.PropertyField(valuesRect, value, GUIContent.none);

                SerializedProperty prob = probabilitiesProperty.GetArrayElementAtIndex(i);
                if (customProb && probabilitiesProperty.arraySize >= 2) {
                    if (i == valuesProperty.arraySize - 1) {
                        float runningTotalWithoutLast = GetRunningTotal(probabilitiesProperty, true);
                        float remainder = 1 - runningTotalWithoutLast;
                        if (remainder >= 0) {
                            prob.floatValue = remainder;
                        }
                        else {
                            prob.floatValue = 0;
                        }

                        EditorGUI.LabelField(customProbabilityValuesRect, prob.floatValue + " (Auto)");
                    }
                    else {
                        EditorGUI.PropertyField(customProbabilityValuesRect, prob, GUIContent.none);
                    }
                }

                
            }


            Rect valuesFooterBaseRect = layoutRect.NextLine;
            
            //plus button
            Rect plusRect = new Rect(x, valuesFooterBaseRect.y + yPadding, minusWidth, minusHeight);
            if (GUI.Button(plusRect, "+")) {
                int lastIndex = valuesProperty.arraySize;
                if (lastIndex < 0) lastIndex = 0;
                probabilitiesProperty.InsertArrayElementAtIndex(lastIndex);
                valuesProperty.InsertArrayElementAtIndex(lastIndex);

                //make last one input equal to zero;
                if (probabilitiesProperty.arraySize > 1)
                    probabilitiesProperty.GetArrayElementAtIndex(probabilitiesProperty.arraySize - 2).floatValue = 0;
                //Debug.Log($"Added element, size now {valuesProperty.arraySize}");
            }

            if (customProb) {
                Rect totalProbRect = new Rect(valuesFooterBaseRect.width - probValuesWidth, valuesFooterBaseRect.y, probValuesWidth,
                                              valuesFooterBaseRect.height);
                if (probabilitiesProperty.arraySize != 0) {
                    float runningTotal = GetRunningTotal(probabilitiesProperty);
                    string direction = "";
                    float remainder = 1 - runningTotal;
                    if (Math.Abs(remainder) > 0.01f && probabilitiesProperty.arraySize > 0) {
                        if (runningTotal > 1) direction = " (too high)";
                        if (runningTotal < 1) direction = " (too low)";

                        EditorGUI.HelpBox(totalProbRect, $"Total = {runningTotal}{direction}", MessageType.Error);
                    }
                    else {
                        EditorGUI.LabelField(totalProbRect, $"Total = {runningTotal}{direction}");
                    }
                }
            }

            if (probabilitiesProperty.arraySize == 0) {
                Rect noValueWarningRect = new Rect(x + 15f + minusWidth, valuesFooterBaseRect.y,
                                                   valuesFooterBaseRect.width - x - 15 - minusWidth - probValuesWidth,
                                                   valuesFooterBaseRect.height);
                EditorGUI.HelpBox(noValueWarningRect, "No values", MessageType.Error);
            }
            
            
            SerializedProperty block = property.FindPropertyRelative(nameof(IndependentVariable.Block));
            if (block.boolValue && valuesProperty.arraySize > ExperimentDesign.MaxBlockPermutationsAllowed) {
                Rect tooManyBlockValuesWarningRect = layoutRect.NextLines(3);
                EditorGUI.HelpBox(tooManyBlockValuesWarningRect, "Too many Block Values for automatic permutation.\n" +
                                                                 "Must define possible Block orders manually using OrderConfig ScriptableObjects.\n" +
                                                                 "See Docs.", 
                                  MessageType.Warning);
            }
           
        }

        /// <summary>
        /// Calculates the running total for custom probabilities for use in filling in last slot
        /// </summary>
        /// <param name="probabilitiesProperty"></param>
        /// <param name="skipLast"></param>
        /// <returns></returns>
        static float GetRunningTotal(SerializedProperty probabilitiesProperty, bool skipLast = false) {
            float runningTotal = 0;

            int n = probabilitiesProperty.arraySize;
            if (skipLast) n--;

            for (int i = 0; i < n; i++) {
                SerializedProperty prob = probabilitiesProperty.GetArrayElementAtIndex(i);
                if (prob.floatValue < 0 || prob.floatValue > 1) {
                    throw new
                        ArgumentOutOfRangeException(
                            $"Can't have a Probability outside of range 0-1, prob: {prob.floatValue} ");
                }

                runningTotal += prob.floatValue;
            }

            return runningTotal;
        }


        public static float AddAllBoolVariableProperties(Rect mainPosition, SerializedProperty mainProperty) {
            mainProperty.serializedObject.Update();
            
            GuiLayoutRect layoutRect = new GuiLayoutRect(LineHeight, mainPosition);
            
            int oldIndentLevel = EditorGUI.indentLevel;

            AddVariableProperties(layoutRect, mainProperty);
            AddIndependentVariableProperties(layoutRect, mainProperty);

            EditorGUI.indentLevel = oldIndentLevel;
            
            mainProperty.serializedObject.ApplyModifiedProperties();
            return layoutRect.FinalHeight;
        }
    }

    internal class VariableNameValidator {

        public bool Valid => reasonsInvalid.Count == 0;

        public string InvalidReasons {
            get {
                string reasons = "";
                foreach (string reason in reasonsInvalid) {
                    reasons += reason + ", ";
                }

                return reasons;
            }
        }

        readonly List<string> reasonsInvalid = new List<string>();

        const string InvalidName =
            "Name Contains Illegal Characters. Name must be one word of letters or numbers only";

        public VariableNameValidator(string nameStringValue) {
            if (!nameStringValue.All(c => Char.IsLetterOrDigit(c) || c == '_')) {
                reasonsInvalid.Add(InvalidName);
            }
        }
    }
}