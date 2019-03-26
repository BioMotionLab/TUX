using System;
using UnityEditor;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.VariableSystem {



    /// <inheritdoc />
    /// <summary>
    /// A custom drawer to edit variables
    /// </summary>
    public class VariableDrawer : PropertyDrawer {
        protected float CustomPropertyHeight;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            float propertyBaseHeight = EditorGUI.GetPropertyHeight(property, GUIContent.none);
            float totalPropertyHeight = propertyBaseHeight + CustomPropertyHeight;
            return totalPropertyHeight;
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// A custom drawer to edit dependent variables
    /// </summary>
    public class DependentVariableDrawer : VariableDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            CustomPropertyHeight = VariableDrawerHelpers.AddAllDependentVariableProperties(position, property);
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// A custom drawer to edit independent variables
    /// </summary>
    public class IndependentVariableDrawer : VariableDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            CustomPropertyHeight = VariableDrawerHelpers.AddAllIndependentVariableProperties(position, property);
        }
    }

    /// <summary>
    /// A collection of functions to help draw variable drawers
    /// </summary>
    public static class VariableDrawerHelpers {

        const float LineHeight = 20f;

        /// <summary>
        /// Adds all properties to independent variables
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static float AddAllIndependentVariableProperties(Rect position, SerializedProperty property) {
            property.serializedObject.Update();
            Rect currentRect = new Rect(position.x, position.y + LineHeight, position.width, LineHeight);
            float oldY = currentRect.y;

            int oldIndentLevel = EditorGUI.indentLevel;

            currentRect = AddVariableProperties(property, currentRect);
            currentRect = AddIndependentVariableProperties(property, currentRect);
            currentRect = AddIndependentVariableValueProperties(property, currentRect);

            EditorGUI.indentLevel = oldIndentLevel;
            float propertyHeight = currentRect.y - oldY;
            property.serializedObject.ApplyModifiedProperties();
            return propertyHeight;
        }

        /// <summary>
        /// Adds all properties to Dependent variables
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static float AddAllDependentVariableProperties(Rect position, SerializedProperty property) {
            property.serializedObject.Update();
            Rect currentRect = new Rect(position.x, position.y + LineHeight, position.width, LineHeight);
            float oldY = currentRect.y;

            int oldIndentLevel = EditorGUI.indentLevel;

            currentRect = AddVariableProperties(property, currentRect);
            currentRect = AddDependentVariableValueProperties(property, currentRect);

            EditorGUI.indentLevel = oldIndentLevel;
            float propertyHeight = currentRect.y - oldY;
            property.serializedObject.ApplyModifiedProperties();
            return propertyHeight;
        }

        /// <summary>
        /// Adds Value display to dependent variable properties
        /// </summary>
        /// <param name="property"></param>
        /// <param name="currentRect"></param>
        /// <returns></returns>
        static Rect AddDependentVariableValueProperties(SerializedProperty property, Rect currentRect) {


            var defaultValueProperty = property.FindPropertyRelative("DefaultValue");

            EditorGUI.PropertyField(currentRect, defaultValueProperty);


            currentRect.y += LineHeight;
            return currentRect;
        }


        /// <summary>
        /// Adds the properties for all variables
        /// </summary>
        /// <param name="property"></param>
        /// <param name="currentRect"></param>
        /// <returns></returns>
        public static Rect AddVariableProperties(SerializedProperty property, Rect currentRect) {

            const float typeWidth = 200f;
            const float nameWidth = 200f;
            const float namePad = 30f;

            Rect nameRect = new Rect(currentRect.x, currentRect.y, nameWidth, currentRect.height);
            SerializedProperty name = property.FindPropertyRelative(nameof(Variable.Name));
            EditorGUI.PropertyField(nameRect, name, GUIContent.none);

            Rect dataTypeRect = new Rect(nameWidth + namePad, currentRect.y, typeWidth, currentRect.height);
            SerializedProperty variableDataType = property.FindPropertyRelative(nameof(Variable.DataType));
            SupportedDataTypes dataType = (SupportedDataTypes) variableDataType.enumValueIndex;
            EditorGUI.LabelField(dataTypeRect, $"Data Type: {dataType.ToString()}");

            currentRect.y += LineHeight;

            EditorGUI.indentLevel++;

            SerializedProperty variableType = property.FindPropertyRelative(nameof(Variable.TypeOfVariable));
            VariableType varType = (VariableType) variableType.enumValueIndex;
            EditorGUI.LabelField(currentRect, $"Variable Type: {varType.ToString()}");
            currentRect.y += LineHeight;
            return currentRect;
        }

        /// <summary>
        /// Adds the properties for independent variables
        /// </summary>
        /// <param name="property"></param>
        /// <param name="currentRect"></param>
        /// <returns></returns>
        public static Rect AddIndependentVariableProperties(SerializedProperty property, Rect currentRect) {
            var block = property.FindPropertyRelative(nameof(IndependentVariable.Block));
            EditorGUI.PropertyField(currentRect, block);
            currentRect.y += LineHeight;

            var mixType = property.FindPropertyRelative(nameof(IndependentVariable.MixingTypeOfVariable));
            EditorGUI.PropertyField(currentRect, mixType);
            currentRect.y += LineHeight;
            return currentRect;
        }

        /// <summary>
        /// Adds display of values for independent variables
        /// </summary>
        /// <param name="property"></param>
        /// <param name="currentRect"></param>
        /// <returns></returns>
        public static Rect AddIndependentVariableValueProperties(SerializedProperty property, Rect currentRect) {
            var valuesProperty = property.FindPropertyRelative("Values");
            var probabilitiesProperty = property.FindPropertyRelative("Probabilities");


            EditorGUI.LabelField(currentRect, "Values");

            const float indentAmt = 40f;
            const float minusWidth = 20f;
            const float minusHeight = 14f;
            const float customProbabilityWidth = 180f;
            float x = indentAmt + currentRect.x;
            const float yPadding = (LineHeight - minusHeight) / 2;

            //Debug.Log($"enum value : {mixType.enumValueIndex} {(VariableMixingType)mixType.enumValueIndex}");

            float probValuesWidth = 0;
            var mixType = property.FindPropertyRelative(nameof(IndependentVariable.MixingTypeOfVariable));
            bool customProb = (VariableMixingType) mixType.enumValueIndex == VariableMixingType.CustomProbability;
            if (customProb) {
                //Debug.Log("custom probabilities");
                probValuesWidth = customProbabilityWidth;
                Rect probLabel = new Rect(currentRect.width - probValuesWidth, currentRect.y, probValuesWidth,
                                          currentRect.height);
                EditorGUI.LabelField(probLabel, "Probability");
            }

            currentRect.y += LineHeight;

            for (int i = 0; i < valuesProperty.arraySize; i++) {
                Rect minusRect = new Rect(x, currentRect.y + yPadding, minusWidth, minusHeight);
                Rect valuesRect = new Rect(x + minusWidth, currentRect.y, 0.5f * currentRect.width, currentRect.height);
                Rect customProbabilityValuesRect = new Rect(currentRect.width - probValuesWidth, currentRect.y,
                                                            probValuesWidth,
                                                            currentRect.height);

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

                currentRect.y += LineHeight;
            }

            //plus button
            Rect plusRect = new Rect(x, currentRect.y + yPadding, minusWidth, minusHeight);
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

                Rect totalProbRect = new Rect(currentRect.width - probValuesWidth, currentRect.y, probValuesWidth,
                                              currentRect.height);
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
                Rect noValueWarningRect = new Rect(x + 15f + minusWidth, currentRect.y,
                                                   currentRect.width - x - 15 - minusWidth - probValuesWidth,
                                                   currentRect.height);
                EditorGUI.HelpBox(noValueWarningRect, "No values", MessageType.Error);
            }

            currentRect.y += LineHeight;
            return currentRect;
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
                        ArgumentOutOfRangeException($"Can't have a ProbabilityIndependentVariables outside of range 0-1, prob: {prob.floatValue} ");
                }

                runningTotal += prob.floatValue;
            }

            return runningTotal;
        }


        public static float AddAllBoolVariableProperties(Rect position, SerializedProperty property) {
            property.serializedObject.Update();
            Rect currentRect = new Rect(position.x, position.y + LineHeight, position.width, LineHeight);
            float oldY = currentRect.y;

            int oldIndentLevel = EditorGUI.indentLevel;

            currentRect = AddVariableProperties(property, currentRect);
            currentRect = AddIndependentVariableProperties(property, currentRect);
            
            EditorGUI.indentLevel = oldIndentLevel;
            float propertyHeight = currentRect.y - oldY;
            property.serializedObject.ApplyModifiedProperties();
            return propertyHeight;
        }
    }


}