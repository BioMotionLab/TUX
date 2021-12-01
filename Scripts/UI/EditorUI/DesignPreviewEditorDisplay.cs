using System.Data;
using bmlTUX.Scripts.UI.RuntimeUI.UIUtilities;
using bmlTUX.Scripts.Utilities.Extensions;
using bmlTUX.Scripts.VariableSystem;
using UnityEditor;
using UnityEngine;

namespace bmlTUX.UI.EditorUI {
    public class DesignPreviewEditorDisplay {
        
        readonly DesignPreviewer previewer;

        Vector2 scrollPos;

        public DesignPreviewEditorDisplay(DesignPreviewer previewer) {
            this.previewer = previewer;
        }

        public  DataTable ShowEditorPreview() {
            
            if (previewer.DesignFile != null) {
                EditorGUILayout.LabelField($"Design File Selected: {previewer.DesignFile.GetName}");
            }
            else {
                EditorGUILayout.HelpBox("Need to have a Design File Selected", MessageType.Warning);
                EditorGUILayout.Space();
            }
            
            
            if (!previewer.DesignFileLinked()) return null;
            
            
            
            
            
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Preview:", EditorStyles.boldLabel);


            if (previewer.DesignFile.GetBlockRandomization != BlockRandomizationMode.CompleteRandomization &&
                previewer.DesignFile.GetBlockRandomization != BlockRandomizationMode.PartialRandomization) {
                EditorGUILayout.LabelField(previewer.BlockOrderData.BlockOrderText);

                previewer.SelectedBlockOrderIndex = previewer.BlockOrderData.SelectionRequired
                    ? EditorGUILayout.Popup(previewer.SelectedBlockOrderIndex,
                                            previewer.ExperimentDesign.BlockPermutationsStrings.ToArray())
                    : previewer.SelectedBlockOrderIndex = previewer.BlockOrderData.DefaultBlockOrderIndex;
            }
            else {
                previewer.SelectedBlockOrderIndex = 0;
            }
            
            EditorGUILayout.Space();

            if (GUILayout.Button("Re-randomize")) {
                previewer.ReRandomizeTable();
            }
            
            if (previewer.SelectedBlockOrderChanged || previewer.PreviewTable == null) {
                previewer.PreviewTable = previewer.ExperimentDesign.GetFinalExperimentTable(previewer.SelectedBlockOrderIndex);
                previewer.LastDisplayedOrderIndex = previewer.SelectedBlockOrderIndex;
            }

            EditorGUILayout.Space();
            EditorGUILayout.TextArea(previewer.PreviewTable.AsString(truncateLength:6));
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            return previewer.PreviewTable;
        }
    }
}