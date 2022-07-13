using UnityEditor;
using UnityEngine;

namespace bmlTUX.Recorder.Editor
{
    [CustomEditor(typeof(PlaybackManager))] 
    public class PlaybackManagerEditor : UnityEditor.Editor
    {
        SerializedProperty playbackFolderProperty;

        void OnEnable()
        {

            playbackFolderProperty = serializedObject.FindProperty(nameof(PlaybackManager.PlaybackFolder));
            
        }

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {

            if (Application.isPlaying)
            {
                PlaybackManager playbackManager = target as PlaybackManager;
                RecordingManager recordingManager = playbackManager.gameObject.GetComponent<RecordingManager>();
                
                string playbackText = "Unknown";
                if (playbackManager != null && recordingManager != null && !recordingManager.IsRecording)
                {
                    EditorGUILayout.Space(20);
                    playbackText = playbackManager.IsPlayingBack ? "Yes" : "No";
                    EditorGUILayout.LabelField($"Currently Playing Back? {playbackText}");
                    
                    if (!playbackManager.IsPlayingBack)
                    {
                        if (GUILayout.Button("Start Playback"))
                        {
                            playbackManager.StartPlayBackFromSelectedFolder();
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Stop Playback"))
                        {
                            playbackManager.StopPlayback();
                        }
                    }
                    EditorGUILayout.Space(20);
                }
            }
            EditorGUILayout.LabelField($"Playback Folder: {playbackFolderProperty.stringValue}");
            if (GUILayout.Button("Select Playback Folder"))
            {
                string folderPath = EditorUtility.OpenFolderPanel("Select Playback Folder",
                    playbackFolderProperty.stringValue, "bmlTUX_Rec_Output");
                playbackFolderProperty.stringValue = folderPath;
            }

            serializedObject.ApplyModifiedProperties();

        }
    }
}