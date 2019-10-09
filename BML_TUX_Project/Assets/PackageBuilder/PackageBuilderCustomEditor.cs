using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BML_Utilities.PackageBuilder {
    /// <summary>
    /// Partly inspired by
    /// https://forum.unity.com/threads/drag-and-drop-streaming-asset-to-inspector-to-get-file-path.499055/
    /// </summary>
    [CustomEditor(typeof(PackageBuilder))]
    [CanEditMultipleObjects]
    public class PackageBuilderCustomEditor : UnityEditor.Editor {

        SerializedProperty folders;
        SerializedProperty exportPath;
        SerializedProperty packageName;
        SerializedProperty version;

        void OnEnable() {
            folders = serializedObject.FindProperty(nameof(PackageBuilder.Folders));
            exportPath = serializedObject.FindProperty(nameof(PackageBuilder.ExportPath));
            packageName = serializedObject.FindProperty(nameof(PackageBuilder.PackageName));
            version = serializedObject.FindProperty(nameof(PackageBuilder.Version));
        }
        
        public override void OnInspectorGUI() {

            serializedObject.Update();
            PackageBuilder targetPackageBuilder = (PackageBuilder) target;
        
            EditorGUILayout.PropertyField(packageName);
        
        
            EditorGUILayout.PropertyField(version);
            string versionOfLastBuild = 
                targetPackageBuilder.VersionOfLastBuild != string.Empty ? 
                    targetPackageBuilder.VersionOfLastBuild : 
                    "No previous builds recorded";
            if (versionOfLastBuild == version.stringValue) {
                EditorGUILayout.HelpBox($"Current version same as version of previous build:{versionOfLastBuild}\n" +
                                        $"Suggest increasing version number. "
                                        , MessageType.Warning);
            }
            else if (targetPackageBuilder.IsOldVersion(version.stringValue)){
                EditorGUILayout.HelpBox($"Current version same as older built version!\n" +
                                        $"Suggest increasing version number beyond previous build:{versionOfLastBuild}. "
                                        , MessageType.Warning);
            }
            
            
            EditorGUILayout.HelpBox($"Version of last build: {versionOfLastBuild}", MessageType.Info);
            
        
        
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Destination: {exportPath.stringValue}");
            if (GUILayout.Button("Change")) {
                exportPath.stringValue = EditorUtility.OpenFolderPanel("Select ExportPackage Destination", "", "");
            }
            EditorGUILayout.EndHorizontal();

        
            targetPackageBuilder.FolderPaths = new List<string>();
            for (int i = 0; i < folders.arraySize; i++) {
                SerializedProperty streamingAsset = folders.GetArrayElementAtIndex(i);
                if (streamingAsset.objectReferenceValue == null) {
                    return;
                }
                string assetPath = AssetDatabase.GetAssetPath(streamingAsset.objectReferenceValue.GetInstanceID());
                targetPackageBuilder.FolderPaths.Add(assetPath);
            }
            EditorGUILayout.LabelField("Included Folders:", EditorStyles.boldLabel);
            foreach (string folderPath in targetPackageBuilder.FolderPaths) {
                EditorGUILayout.LabelField(folderPath);
            }
        
        
            EditorGUILayout.Space();
        
            if (GUILayout.Button("Build Package")) {
                targetPackageBuilder.ExportPackage();
            }

        
        
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("");
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("");
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("");
            EditorGUILayout.Space();
        
        
            EditorGUILayout.LabelField("Edit Included Folders:", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("(First entry will contain version text file)", MessageType.Info);
            EditorGUILayout.PropertyField(folders, includeChildren:true);
        
            EditorGUILayout.Space();
        
            serializedObject.ApplyModifiedProperties();
        }
    
    }
}