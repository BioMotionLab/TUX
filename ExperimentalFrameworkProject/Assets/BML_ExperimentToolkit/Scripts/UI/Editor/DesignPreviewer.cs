using System.Data;
using BML_ExperimentToolkit.Scripts.ExperimentParts;
using BML_ExperimentToolkit.Scripts.VariableSystem;
using BML_Utilities.Extensions;
using UnityEditor;

namespace BML_ExperimentToolkit.Scripts.UI.Editor {
    public class DesignPreviewer {
        readonly VariableConfigurationFile configurationFile;
        public int                                OrderIndex;
        public DesignPreviewer(VariableConfigurationFile configurationFile) {
            this.configurationFile = configurationFile;
        }
        
        bool ConfigurationFileLinked() {
            if (configurationFile != null) {
                EditorGUILayout.LabelField($"Config File Selected: {configurationFile.name}");
                return true;
            }
            else {
                EditorGUILayout.HelpBox("Need to have a Variable Config File Selected", MessageType.Warning);
                EditorGUILayout.Space();
                return false; 
            }
            
        }
        public DataTable ShowPreview() {

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
            EditorGUILayout.EndVertical();
            return finalTable;
        }
    }
}