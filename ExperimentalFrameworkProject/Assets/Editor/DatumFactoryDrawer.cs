using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(DatumFactory))]
public class DatumFactoryDrawer : PropertyDrawer {

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return label != GUIContent.none && Screen.width < 333 ? (16f + 18f) : 16f;
    }


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        property.serializedObject.Update();
        int oldIndentLevel = EditorGUI.indentLevel;
        label = EditorGUI.BeginProperty(position, label, property);
        Rect contentPosition = EditorGUI.PrefixLabel(position, label);

        Rect currentRect = new Rect(position.x, position.y + 20f, position.width, 20f);
        var typeProperty = property.FindPropertyRelative(nameof(DatumFactory.TypeToCreate));
        EditorGUI.PropertyField(currentRect, typeProperty);

        currentRect.y += 20f;
        if (GUI.Button(currentRect, "Add New")) {
            DatumFactory factory =
                fieldInfo.GetValue(property.serializedObject.targetObject) as DatumFactory;
            factory?.New();
        }

        currentRect.y += 20f;
        var intsProperty = property.FindPropertyRelative(nameof(DatumFactory.intData));
        if (intsProperty != null) {
            EditorGUI.PropertyField(currentRect, intsProperty, GUIContent.none, includeChildren: true);
        }
        else {
            Debug.Log("null");
        }

        currentRect.y += 100f;
        var floatsProperty = property.FindPropertyRelative(nameof(DatumFactory.floatData));
        EditorGUI.PropertyField(currentRect, floatsProperty, GUIContent.none, includeChildren: true);


        EditorGUI.EndProperty();
        EditorGUI.indentLevel = oldIndentLevel;
    }
}
