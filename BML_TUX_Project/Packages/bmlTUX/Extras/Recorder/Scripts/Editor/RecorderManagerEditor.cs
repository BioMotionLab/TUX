using System.Collections;
using System.Collections.Generic;
using bmlTUX.Recorder;
using UnityEditor;
using UnityEngine;

namespace bmlTUX
{
    [CustomEditor(typeof(RecordingManager))] 
    public class RecorderManagerEditor : Editor
    {
        SerializedProperty recordOnStartupProperty;
        SerializedProperty recordModeProperty;
        SerializedProperty recordingOutputFolderProperty;

        void OnEnable()
        {
            recordOnStartupProperty = serializedObject.FindProperty(nameof(RecordingManager.recordOnStartup));
            recordModeProperty = serializedObject.FindProperty(nameof(RecordingManager.recordingMode));
            recordingOutputFolderProperty = serializedObject.FindProperty(nameof(RecordingManager.recordingFolder));
            
        }

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {

            
            if (Application.isPlaying)
            {
                RecordingManager recordingManager = target as RecordingManager;
                string recordingText = "Unknown";
                if (recordingManager != null)
                {
                    EditorGUILayout.Space(20);
                    recordingText = recordingManager.IsRecording ? "Yes" : "No";
                    EditorGUILayout.LabelField($"Currently Recording? {recordingText}");
                    
                    if (!recordingManager.IsRecording)
                    {
                        if (GUILayout.Button("Start Recording"))
                        {
                            recordingManager.StartRecordingInFolderSelectedInInspector();
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Stop Recording"))
                        {
                            recordingManager.StopRecording();
                        }
                    }
                    EditorGUILayout.Space(20);
                }
            }
            
            
            EditorGUILayout.PropertyField(recordOnStartupProperty);
            EditorGUILayout.PropertyField(recordModeProperty);

            EditorGUILayout.LabelField($"Output Folder: {recordingOutputFolderProperty.stringValue}");
            if (GUILayout.Button("Select Output Folder"))
            {
                string folderPath = EditorUtility.OpenFolderPanel("Select output folder",
                    recordingOutputFolderProperty.stringValue, "bmlTUX_Rec_Output");
                recordingOutputFolderProperty.stringValue = folderPath;
            }

            serializedObject.ApplyModifiedProperties();

        }
    }
}
