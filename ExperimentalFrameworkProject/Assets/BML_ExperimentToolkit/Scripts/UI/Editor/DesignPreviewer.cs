using System.Data;
using BML_ExperimentToolkit.Scripts.ExperimentParts;
using BML_ExperimentToolkit.Scripts.VariableSystem;
using BML_Utilities.Extensions;
using UnityEditor;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.UI.Editor {
    public class DesignPreviewer {
        VariableConfigurationFile configurationFile;
        public int                                OrderIndex;
        Vector2 scrollPos;

        public DesignPreviewer(VariableConfigurationFile configurationFile) {
            this.configurationFile = configurationFile;
        }
        
        bool ConfigurationFileLinked() {
            configurationFile = Selection.activeObject as VariableConfigurationFile;
            bool linked = false;
            if (configurationFile != null) {
                EditorGUILayout.LabelField($"Config File Selected: {configurationFile.name}");
                linked = true;
            }
            else {
                EditorGUILayout.HelpBox("Need to have a Variable Config File Selected", MessageType.Warning);
                EditorGUILayout.Space();
            }

            return linked;
        }
        
        public DataTable ShowPreview() {
            
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, 
                                                        false, false, 
                                                        GUILayout.ExpandHeight(true));
            
            if (!ConfigurationFileLinked()) return null;
            
            EditorGUILayout.BeginVertical();
            
            EditorGUILayout.LabelField("Preview:", EditorStyles.boldLabel);
            ExperimentDesign design = ExperimentDesign.CreateFrom(configurationFile);

            if (design.HasBlocks) {
                EditorGUILayout.LabelField("Select A Block Order");
                string[] orderStrings = design.BlockPermutationsStrings.ToArray();
                OrderIndex = EditorGUILayout.Popup(OrderIndex, orderStrings);
            }
            else {
                EditorGUILayout.LabelField("No block variables");
                OrderIndex = 0;
            }
            EditorGUILayout.Space();

            var finalTable = design.GetFinalExperimentTable(OrderIndex);

            EditorGUILayout.Space();
            EditorGUILayout.TextArea(finalTable.AsString());
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            return finalTable;
        }
    }
}