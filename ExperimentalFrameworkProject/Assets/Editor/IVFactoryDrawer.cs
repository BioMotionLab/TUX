using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(IndependentVariableFactory))]
public class IVFactoryDrawer : PropertyDrawer {

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return label != GUIContent.none && Screen.width < 333 ? (16f + 18f) : 16f;
    }


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        property.serializedObject.Update();
        int oldIndentLevel = EditorGUI.indentLevel;
        label = EditorGUI.BeginProperty(position, label, property);
        Rect contentPosition = EditorGUI.PrefixLabel(position, label);
        
        Rect currentRect = new Rect(position.x, position.y+20f, position.width, 20f);
        var typeProperty = property.FindPropertyRelative(nameof(IndependentVariableFactory.SelectType));
        EditorGUI.PropertyField(currentRect, typeProperty);

        currentRect.y += 20f;
        if (GUI.Button(currentRect, "Add New")) {
            IndependentVariableFactory factory =
                fieldInfo.GetValue(property.serializedObject.targetObject) as IndependentVariableFactory;
            factory?.CreateNew();
        }

        currentRect.y += 20f;
        var variablesProperty = property.FindPropertyRelative(nameof(IndependentVariableFactory.IVs));
        if (variablesProperty != null) {
            EditorGUI.PropertyField(currentRect, variablesProperty, GUIContent.none, includeChildren:true);
        }
        else {
            Debug.Log("null");
        }

        EditorGUI.EndProperty();
        EditorGUI.indentLevel = oldIndentLevel;
    }
}


   
