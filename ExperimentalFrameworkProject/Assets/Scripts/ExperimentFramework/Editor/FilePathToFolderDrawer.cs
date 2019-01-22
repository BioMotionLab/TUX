using System.IO;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(OutputDataFile), true)]
public class OutputDataFileDrawer : PropertyDrawer {
    SerializedProperty pathProperty;

    const float VerticalInspectorSpacing = 20;
    float totalPropertyHeight;
    float drawerHeight = 150;
    float boxHeight = 20;
    float currentYPosition;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return drawerHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

        this.pathProperty = property.FindPropertyRelative(nameof(OutputDataFile.FullPath));

        
        currentYPosition = position.y;

        currentYPosition += VerticalInspectorSpacing;
        var outputFolderLabelRect = new Rect(position.x, currentYPosition, position.width, boxHeight);
        currentYPosition += VerticalInspectorSpacing;
        var outputFolderPathLabelRect = new Rect(position.x, currentYPosition, position.width, boxHeight);
        currentYPosition += VerticalInspectorSpacing;
        var outputFolderButtonRect = new Rect(position.x, currentYPosition, position.width, boxHeight);
        
        

        EditorGUI.LabelField(outputFolderLabelRect, "Output File:");

        EditorGUI.BeginChangeCheck();
        EditorGUI.LabelField(outputFolderPathLabelRect, pathProperty.stringValue);
        EditorGUI.EndChangeCheck();
        
        if (GUI.Button(outputFolderButtonRect, "Configure output file")) {
            string path = EditorUtility.SaveFilePanel("Select output csv","", "", "csv");
            pathProperty.stringValue = path;

            if (File.Exists(pathProperty.stringValue)) {
                File.Delete(pathProperty.stringValue);
            }
        }
        
        
        
        
        

    }
}
