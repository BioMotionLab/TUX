using System;
using UnityEditor;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.VariableUI {


    [CustomPropertyDrawer(typeof(VariableFactory))]
    public class VariableFactoryDrawer : PropertyDrawer {

        const float LineHeight = 20f;
        const float backgroundPadding = 5f;
        float       height;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUI.GetPropertyHeight(property) + height;
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            //property.serializedObject.Update();
            int oldIndentLevel = EditorGUI.indentLevel;

            Rect currentRect = new Rect(position.x, position.y + LineHeight, position.width, LineHeight);
            float oldY = currentRect.y;

            Color oldColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.white;

            SerializedProperty dataTypeProperty =
                property.FindPropertyRelative(nameof(VariableFactory.DataTypesToCreate));
            EditorGUI.PropertyField(currentRect, dataTypeProperty);
            currentRect.y += LineHeight;

            SerializedProperty variableTypeProperty =
                property.FindPropertyRelative(nameof(VariableFactory.VariableTypeToCreate));
            EditorGUI.PropertyField(currentRect, variableTypeProperty);
            currentRect.y += LineHeight;

            if (GUI.Button(currentRect, "Create Variable")) {
                VariableFactory factory =
                    fieldInfo.GetValue(property.serializedObject.targetObject) as VariableFactory;
                factory?.AddNew();
            }
            currentRect.y += LineHeight;
            GUI.backgroundColor = oldColor;

            property.serializedObject.ApplyModifiedProperties();

            //ADD IVs
            SerializedProperty intIvProperty = property.FindPropertyRelative(nameof(VariableFactory.IntIVs));
            currentRect = AddPropertyFromList(currentRect, intIvProperty);

            SerializedProperty floatsIvProperty = property.FindPropertyRelative(nameof(VariableFactory.FloatIVs));
            currentRect = AddPropertyFromList(currentRect, floatsIvProperty);

            SerializedProperty stringsIvProperty = property.FindPropertyRelative(nameof(VariableFactory.StringIVs));
            currentRect = AddPropertyFromList(currentRect, stringsIvProperty);

            SerializedProperty boolsIvProperty = property.FindPropertyRelative(nameof(VariableFactory.BoolIVs));
            currentRect = AddPropertyFromList(currentRect, boolsIvProperty);

            SerializedProperty gameObjectsIvProperty =
                property.FindPropertyRelative(nameof(VariableFactory.GameObjectIVs));
            currentRect = AddPropertyFromList(currentRect, gameObjectsIvProperty);

            SerializedProperty vector3IvProperty = property.FindPropertyRelative(nameof(VariableFactory.Vector3IVs));
            currentRect = AddPropertyFromList(currentRect, vector3IvProperty);

            SerializedProperty customDataTypesIvProperty =
                property.FindPropertyRelative(nameof(VariableFactory.CustomDataTypeIVs));
            currentRect = AddPropertyFromList(currentRect, customDataTypesIvProperty);

            //ADD DVs
            SerializedProperty intDvProperty = property.FindPropertyRelative(nameof(VariableFactory.IntDVs));
            currentRect = AddPropertyFromList(currentRect, intDvProperty);

            SerializedProperty floatsDvProperty = property.FindPropertyRelative(nameof(VariableFactory.FloatDVs));
            currentRect = AddPropertyFromList(currentRect, floatsDvProperty);

            SerializedProperty stringsDvProperty = property.FindPropertyRelative(nameof(VariableFactory.StringDVs));
            currentRect = AddPropertyFromList(currentRect, stringsDvProperty);

            SerializedProperty boolsDvProperty = property.FindPropertyRelative(nameof(VariableFactory.BoolDVs));
            currentRect = AddPropertyFromList(currentRect, boolsDvProperty);

            SerializedProperty gameObjectsDvProperty =
                property.FindPropertyRelative(nameof(VariableFactory.GameObjectDVs));
            currentRect = AddPropertyFromList(currentRect, gameObjectsDvProperty);

            SerializedProperty vector3DvProperty = property.FindPropertyRelative(nameof(VariableFactory.Vector3DVs));
            currentRect = AddPropertyFromList(currentRect, vector3DvProperty);

            SerializedProperty customDataTypesDvProperty =
                property.FindPropertyRelative(nameof(VariableFactory.CustomDataTypeDVs));
            currentRect = AddPropertyFromList(currentRect, customDataTypesDvProperty);



            EditorGUI.LabelField(currentRect, "--------");
            currentRect.y += 20f;
            //var floatsProperty = property.FindPropertyRelative(nameof(VariableFactory.floatData));
            //EditorGUI.PropertyField(currentRect, floatsProperty, GUIContent.none, includeChildren: true);

            //currentRect.y += 20f;

            EditorGUI.indentLevel = oldIndentLevel;
            if (Math.Abs(height - currentRect.y) > 0.001f) {

                height = currentRect.y - oldY;
                //Debug.Log($"setting height of factory drawer to {height}");
            }

            property.serializedObject.ApplyModifiedProperties();
        }

        static Rect AddPropertyFromList(Rect currentRect, SerializedProperty valueProperty) {
            const float deleteButtonHeight = 0.75f;
            for (int i = 0; i < valueProperty.arraySize; i++) {
                SerializedProperty item = valueProperty.GetArrayElementAtIndex(i);
                EditorGUI.PropertyField(currentRect, item, GUIContent.none);
                currentRect.y += EditorGUI.GetPropertyHeight(item, GUIContent.none) + LineHeight;

                float yPadding = (currentRect.height - (currentRect.height * deleteButtonHeight)) / 2f;
                Rect deleteButtonRect = new Rect(currentRect.x + 40, currentRect.y + yPadding, 125,
                                                 deleteButtonHeight * currentRect.height);
                Color oldColor = GUI.backgroundColor;
                //GUI.color = Color.red;
                //GUI.DrawTexture(currentRect, EditorGUIUtility.whiteTexture);
                GUI.backgroundColor = Color.red;
                if (GUI.Button(deleteButtonRect, "Delete Variable")) {
                    valueProperty.DeleteArrayElementAtIndex(i);
                    break;
                }
                
                GUI.backgroundColor = oldColor;

                currentRect.y += LineHeight;
            }

            valueProperty.serializedObject.ApplyModifiedProperties();
            return currentRect;
        }

    }
}