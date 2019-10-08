using System;
using System.IO;
using BML_ExperimentToolkit.Scripts.VariableSystem;
using BML_Utilities;
using BML_Utilities.Extensions;
using UnityEditor;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.UI.Editor {
    public class DesignSaverWindow : EditorWindow {
      
        string fileName = "experimentDesignSave";
        
        string folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        
        DesignPreviewer previewer;
        
        void OnGUI() {
            var finalTable = previewer.ShowPreview();

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

        public static void ShowWindow(ExperimentDesignFile configFile) {
            DesignSaverWindow window = (DesignSaverWindow) GetWindow(typeof(DesignSaverWindow), false, "Design Saver");
            window.previewer = new DesignPreviewer(configFile);
            window.Show();
        }
    }
}