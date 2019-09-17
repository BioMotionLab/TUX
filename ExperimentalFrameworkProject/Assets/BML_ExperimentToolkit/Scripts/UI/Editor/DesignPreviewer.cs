using System.Data;
using BML_ExperimentToolkit.Scripts.ExperimentParts;
using BML_ExperimentToolkit.Scripts.VariableSystem;
using BML_Utilities.Extensions;
using UnityEditor;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.UI.Editor {
    public class DesignPreviewer {
        readonly VariableConfigurationFile configurationFile;
        public int SelectedBlockOrderIndex;
        Vector2 scrollPos;
        readonly ExperimentDesign experimentDesign;
        DataTable previewTable;
        int lastDisplayedOrderIndex = -1 ;
        readonly BlockOrderData blockOrderData;

        public DesignPreviewer(VariableConfigurationFile configurationFile) {
            this.configurationFile = configurationFile;
            experimentDesign = ExperimentDesign.CreateFrom(configurationFile);
            blockOrderData = new BlockOrderData(experimentDesign);
        }
        
        bool ConfigurationFileLinked() {
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
            
            if (!ConfigurationFileLinked()) return null;
            
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, 
                                                        false, false, 
                                                        GUILayout.ExpandHeight(true));
            
            
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Preview:", EditorStyles.boldLabel);

            EditorGUILayout.LabelField(blockOrderData.BlockOrderText);

            SelectedBlockOrderIndex = blockOrderData.SelectionRequired
                ? EditorGUILayout.Popup(SelectedBlockOrderIndex, experimentDesign.BlockPermutationsStrings.ToArray())
                : SelectedBlockOrderIndex = blockOrderData.DefaultBlockOrderIndex;
            
            EditorGUILayout.Space();
            
            if (SelectedBlockOrderChanged || previewTable == null) {
                previewTable = experimentDesign.GetFinalExperimentTable(SelectedBlockOrderIndex);
                lastDisplayedOrderIndex = SelectedBlockOrderIndex;
            }

            EditorGUILayout.Space();
            EditorGUILayout.TextArea(previewTable.AsString());
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            return previewTable;
        }

        bool SelectedBlockOrderChanged => SelectedBlockOrderIndex != lastDisplayedOrderIndex;
    }
}