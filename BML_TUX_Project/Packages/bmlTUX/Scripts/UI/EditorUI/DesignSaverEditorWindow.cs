using System;
using System.IO;
using bmlTUX.Scripts.UI.RuntimeUI.UIUtilities;
using bmlTUX.Scripts.Utilities;
using bmlTUX.Scripts.Utilities.Extensions;
using bmlTUX.Scripts.VariableSystem;
using UnityEditor;
using UnityEngine;

namespace bmlTUX.Scripts.UI.EditorUI {
    public class DesignSaverEditorWindow : EditorWindow {
      
        string fileName = "experimentDesignSave";
        
        string folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        
        DesignPreviewer previewer;
        
        void OnGUI() {
            DesignPreviewEditorDisplay previewDisplay = new DesignPreviewEditorDisplay(previewer);
            var finalTable = previewDisplay.ShowEditorPreview();

            EditorGUILayout.LabelField("Save:", EditorStyles.boldLabel);
            ShowFileSelections();
            EditorGUILayout.Space();
            if (GUILayout.Button("Save")) {
                string fileNameWithExtension = fileName + ".csv";
                string path = Path.Combine(folder, fileNameWithExtension);
                File.WriteAllText(path, finalTable.AsString(truncateLength:-1, separator:Delimiter.Comma));
                Close();
            }
            
        }

        void ShowFileSelections() {
            EditorGUILayout.Space();
            fileName = EditorGUILayout.TextField("FileName:", fileName);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"folder: {folder}");
            if (GUILayout.Button("Change Folder")) {
                folder = EditorUtility.OpenFolderPanel("Select Folder", "", "");
            }

            EditorGUILayout.EndHorizontal();
        }

        public static void ShowWindow(IExperimentDesignFile configFile) {
            DesignSaverEditorWindow editorWindow = (DesignSaverEditorWindow) GetWindow(typeof(DesignSaverEditorWindow), false, "Design Saver");
            editorWindow.previewer = new DesignPreviewer(configFile);
            editorWindow.Show();
        }
    }
}