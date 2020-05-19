using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SampleBuilder))]
[CanEditMultipleObjects]
public class SampleBuilderCustomEditor : Editor {

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        
        SampleBuilder targetPackageBuilder = (SampleBuilder) target;
        if (GUILayout.Button("Build Package")) {
            targetPackageBuilder.BuildSamples();
        }
    }
}