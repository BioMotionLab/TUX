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
        const float IndentAmt   = 40f;
        const float MinusWidth  = 20f;
        const float MinusHeight = 14f;
        const float YPadding    = (LineHeight - MinusHeight) / 2;
        const float CustomProbabilityWidth = 180f;
 
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

            if (mainProperty.isExpanded) {
                AddIndependentVariableProperties(layoutRect, mainProperty);
                AddIndependentVariableValueProperties(layoutRect, mainProperty);
            }

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

            if (mainProperty.isExpanded) {
                AddDependentVariableValueProperties(layoutRect, mainProperty);
            }
            
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

            if (mainProperty.isExpanded) {
                AddParticipantVariableValueProperties(layoutRect, mainProperty);
            }
            
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
                float x = IndentAmt + layoutRect.CurrentLine.x;
                
                AddValuePropertyField(layoutRect, valuesProperty, x);
                
                Rect valuesFooterBaseRect = layoutRect.NextLine;
                //plus button
                Rect plusRect = new Rect(x, valuesFooterBaseRect.y + YPadding, MinusWidth, MinusHeight);
                if (GUI.Button(plusRect, "+")) {
                    int lastIndex = valuesProperty.arraySize;
                    if (lastIndex < 0) lastIndex = 0;

                    valuesProperty.InsertArrayElementAtIndex(lastIndex);
                }
            }

            property.serializedObject.ApplyModifiedProperties();

        }

        static void AddValuePropertyField(GuiLayoutRect layoutRect, SerializedProperty valuesProperty, float x) {
            for (int i = 0; i < valuesProperty.arraySize; i++) {
                Rect valuesBaseRect = layoutRect.NextLine;
                Rect minusRect = new Rect(x, valuesBaseRect.y + YPadding, MinusWidth, MinusHeight);
                Rect valuesRect = new Rect(x + MinusWidth, valuesBaseRect.y, 0.5f * valuesBaseRect.width,
                                           valuesBaseRect.height);

                //Minus button
                if (GUI.Button(minusRect, "-")) {
                    valuesProperty.DeleteArrayElementAtIndex(i);
                    break;
                }

                SerializedProperty value = valuesProperty.GetArrayElementAtIndex(i);
                EditorGUI.PropertyField(valuesRect, value, GUIContent.none);
            }
        }


        /// <summary>
        /// Adds the properties for all variables
        /// </summary>
        /// <param name="layoutRect"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        static void AddVariableProperties(GuiLayoutRect layoutRect, SerializedProperty property) {

            Rect foldoutPos = layoutRect.CurrentLine;
            foldoutPos.width = 1;
            property.isExpanded = EditorGUI.Foldout(foldoutPos, property.isExpanded, GUIContent.none);

            Rect NameRect = layoutRect.CurrentLine;
            NameRect.width = EditorGUIUtility.labelWidth;
            SerializedProperty name = property.FindPropertyRelative(nameof(Variable.Name));
            EditorGUI.PropertyField(NameRect, name, GUIContent.none);

            Rect TypeRect = layoutRect.CurrentLine;
            TypeRect.width = 80;
            TypeRect.x = TypeRect.x + EditorGUIUtility.labelWidth; 
            SerializedProperty variableDataType = property.FindPropertyRelative(nameof(Variable.DataType));
            SupportedDataType dataType = (SupportedDataType) variableDataType.enumValueIndex;
            EditorGUI.LabelField(TypeRect, $"Type: {dataType.ToString()}");
            
            VariableNameValidator validator = new VariableNameValidator(name.stringValue);
            if (!validator.Valid) {
                Rect warningBoxRect = layoutRect.NextLines(3);
                EditorGUI.HelpBox(warningBoxRect, validator.InvalidReasons, MessageType.Error);
            }

//            SerializedProperty variableType = property.FindPropertyRelative(nameof(Variable.TypeOfVariable));
//            VariableType varType = (VariableType) variableType.enumValueIndex;
//            EditorGUI.LabelField(layoutRect.NextLine, $"Variable Type: {varType.ToString()}");
            
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
            
            float x = IndentAmt + layoutRect.CurrentLine.x;
            
            float probValuesWidth = 0;
            SerializedProperty mixType =
                property.FindPropertyRelative(nameof(IndependentVariable.MixingTypeOfVariable));
            bool customProb = (VariableMixingType) mixType.enumValueIndex == VariableMixingType.CustomProbability;
            
            
            if (customProb) {
                probValuesWidth = CustomProbabilityWidth;
                Rect probLabel = new Rect(valueLabelBaseRect.width - probValuesWidth, 
                                          valueLabelBaseRect.y, 
                                          probValuesWidth,
                                          valueLabelBaseRect.height);
                EditorGUI.LabelField(probLabel, "Probability");
            }
            
            AddIvValuePropertyFields(layoutRect, valuesProperty, x, probabilitiesProperty, customProb);
            
            Rect valuesFooterBaseRect = layoutRect.NextLine;
            AddPlusButton(x, valuesFooterBaseRect, valuesProperty, probabilitiesProperty);
            
            AddCustomProbabilityField(customProb, valuesFooterBaseRect, probValuesWidth, probabilitiesProperty);
            if (probabilitiesProperty.arraySize == 0) {
                Rect noValueWarningRect = new Rect(x + 15f + MinusWidth, valuesFooterBaseRect.y,
                                                   valuesFooterBaseRect.width - x - 15 - MinusWidth - probValuesWidth,
                                                   valuesFooterBaseRect.height);
                EditorGUI.HelpBox(noValueWarningRect, "No values", MessageType.Error);
            }

            SerializedProperty block = property.FindPropertyRelative(nameof(IndependentVariable.Block));
            CheckMaxBlockPermutationsAllowed(layoutRect, block, valuesProperty);
           
        }

        static void CheckMaxBlockPermutationsAllowed(GuiLayoutRect      layoutRect, SerializedProperty block,
                                                     SerializedProperty valuesProperty) {
            if (!block.boolValue || valuesProperty.arraySize <= ExperimentDesign.MaxBlockPermutationsAllowed) return;
            Rect tooManyBlockValuesWarningRect = layoutRect.NextLines(3);
            EditorGUI.HelpBox(tooManyBlockValuesWarningRect, "Too many Block Values for automatic permutation.\n" +
                                                             "Must define possible Block orders manually using OrderConfig ScriptableObjects.\n" +
                                                             "See Docs.",
                              MessageType.Warning);
        }

        static void AddCustomProbabilityField(bool               customProb, Rect valuesFooterBaseRect, float probValuesWidth,
                                              SerializedProperty probabilitiesProperty) {
            
            if (!customProb || probabilitiesProperty.arraySize == 0) return;
            
            Rect totalProbRect = new Rect(valuesFooterBaseRect.width - probValuesWidth, valuesFooterBaseRect.y,
                                          probValuesWidth,
                                          valuesFooterBaseRect.height);
            
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

        static void AddPlusButton(float x, 
                                  Rect valuesFooterBaseRect,
                                  SerializedProperty valuesProperty, 
                                  SerializedProperty probabilitiesProperty) {
            Rect plusRect = new Rect(x, valuesFooterBaseRect.y + YPadding, MinusWidth, MinusHeight);
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
        }

        static void AddIvValuePropertyFields(GuiLayoutRect layoutRect, 
                                             SerializedProperty valuesProperty,
                                             float x,
                                             SerializedProperty probabilitiesProperty, 
                                             bool customProb) {
            
            for (int i = 0; i < valuesProperty.arraySize; i++) {
                Rect valueBaseRect = layoutRect.NextLine;

                Rect minusRect = new Rect(x, valueBaseRect.y + YPadding, MinusWidth, MinusHeight);
                Rect valuesRect = new Rect(x + MinusWidth, valueBaseRect.y, 0.5f * valueBaseRect.width, valueBaseRect.height);
                Rect customProbabilityValuesRect = new Rect(valueBaseRect.width - CustomProbabilityWidth, valueBaseRect.y,
                                                            CustomProbabilityWidth,
                                                            valueBaseRect.height);

                //Minus button
                if (GUI.Button(minusRect, "-")) {
                    valuesProperty.DeleteArrayElementAtIndex(i);
                    probabilitiesProperty.DeleteArrayElementAtIndex(i);
                    break;
                }

                SerializedProperty value = valuesProperty.GetArrayElementAtIndex(i);
                EditorGUI.PropertyField(valuesRect, value, GUIContent.none);

                AddCustomProbabilities(valuesProperty, probabilitiesProperty, customProb, i, customProbabilityValuesRect);
            }
        }

        static void AddCustomProbabilities(SerializedProperty valuesProperty, SerializedProperty probabilitiesProperty,
                                           bool               customProb,     int                i,
                                           Rect               customProbabilityValuesRect) {
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