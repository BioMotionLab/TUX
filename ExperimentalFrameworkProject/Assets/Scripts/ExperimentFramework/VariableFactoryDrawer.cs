using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(VariableFactory))]
public class VariableFactoryDrawer : PropertyDrawer {

    const float lineHeight = 20f;
    float height;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return EditorGUI.GetPropertyHeight(property) + height;
    }


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        property.serializedObject.Update();
        int oldIndentLevel = EditorGUI.indentLevel;
        
        Rect currentRect = new Rect(position.x, position.y + 20f, position.width, 20f);
        float oldy = currentRect.y;

        SerializedProperty dataTypeProperty = property.FindPropertyRelative(nameof(VariableFactory.DataTypesToCreate));
        EditorGUI.PropertyField(currentRect, dataTypeProperty);
        currentRect.y += lineHeight;

        SerializedProperty variableTypeProperty = property.FindPropertyRelative(nameof(VariableFactory.VariableTypeToCreate));
        EditorGUI.PropertyField(currentRect, variableTypeProperty);
        currentRect.y += lineHeight;

        if (GUI.Button(currentRect, "Create Variable")) {
            VariableFactory factory =
                fieldInfo.GetValue(property.serializedObject.targetObject) as VariableFactory;
            factory?.AddNew();
        }
        currentRect.y += lineHeight;


        property.serializedObject.ApplyModifiedProperties();

        SerializedProperty intsProperty = property.FindPropertyRelative(nameof(VariableFactory.intIVs));
        currentRect = AddPropertyFromList(property, currentRect, intsProperty);

        SerializedProperty floatsProperty = property.FindPropertyRelative(nameof(VariableFactory.floatIVs));
        currentRect = AddPropertyFromList(property, currentRect, floatsProperty);


        EditorGUI.LabelField(currentRect, "--------");
        currentRect.y += 20f;
        //var floatsProperty = property.FindPropertyRelative(nameof(VariableFactory.floatData));
        //EditorGUI.PropertyField(currentRect, floatsProperty, GUIContent.none, includeChildren: true);

        //currentRect.y += 20f;

        EditorGUI.indentLevel = oldIndentLevel;
        if (Math.Abs(height - currentRect.y) > 0.001f) {

            height = currentRect.y - oldy;
            //Debug.Log($"setting height of factory drawer to {height}");
        }

        property.serializedObject.ApplyModifiedProperties();
    }

    static Rect AddPropertyFromList(SerializedProperty property, Rect currentRect, SerializedProperty valueProperty) {
        const float deleteButtonHeight = 0.75f;
        for (int i = 0; i < valueProperty.arraySize; i++) {
            SerializedProperty item = valueProperty.GetArrayElementAtIndex(i);
            EditorGUI.PropertyField(currentRect, item, GUIContent.none);
            currentRect.y += EditorGUI.GetPropertyHeight(item, GUIContent.none) + lineHeight;

            float ypad = (currentRect.height - (currentRect.height * deleteButtonHeight)) / 2f;
            Rect deleteButtonRect = new Rect(currentRect.x + 40, currentRect.y + ypad, 125,
                                             deleteButtonHeight * currentRect.height);
            Color oldColor = GUI.color;
            //GUI.color = Color.red;
            //GUI.DrawTexture(currentRect, EditorGUIUtility.whiteTexture);
            if (GUI.Button(deleteButtonRect, "Delete Variable")) {
                valueProperty.DeleteArrayElementAtIndex(i);
                break;
            }

            GUI.color = oldColor;

            currentRect.y += lineHeight;
        }

        return currentRect;
    }
    
}
