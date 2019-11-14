using System.Data;
using bmlTUX.Scripts.UI.Runtime;
using bmlTUX.Scripts.Utilities.Extensions;
using bmlTUX.Scripts.VariableSystem;
using UnityEditor;
using UnityEngine;

namespace bmlTUX.Scripts.UI.EditorUI {
    public class DesignPreviewEditorDisplay {
        
        readonly DesignPreviewer previewer;

        Vector2 scrollPos;

        public DesignPreviewEditorDisplay(DesignPreviewer previewer) {
            this.previewer = previewer;
        }

        public  DataTable ShowEditorPreview() {
            
            if (previewer.designFile != null) {
                EditorGUILayout.LabelField($"Design File Selected: {previewer.designFile.name}");
            }
            else {
                EditorGUILayout.HelpBox("Need to have a Design File Selected", MessageType.Warning);
                EditorGUILayout.Space();
            }
            
            
            if (!previewer.DesignFileLinked()) return null;
            
            
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, 
                                                        false, false, 
                                                        GUILayout.ExpandHeight(true));
            
            
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Preview:", EditorStyles.boldLabel);


            if (previewer.designFile.BlockRandomization != BlockRandomizationMode.CompleteRandomization &&
                previewer.designFile.BlockRandomization != BlockRandomizationMode.PartialRandomization) {
                EditorGUILayout.LabelField(previewer.blockOrderData.BlockOrderText);

                previewer.SelectedBlockOrderIndex = previewer.blockOrderData.SelectionRequired
                    ? EditorGUILayout.Popup(previewer.SelectedBlockOrderIndex,
                                            previewer.experimentDesign.BlockPermutationsStrings.ToArray())
                    : previewer.SelectedBlockOrderIndex = previewer.blockOrderData.DefaultBlockOrderIndex;
            }
            else {
                previewer.SelectedBlockOrderIndex = 0;
            }
            
            EditorGUILayout.Space();

            if (GUILayout.Button("Re-randomize")) {
                previewer.ReRandomizeTable();
            }
            
            if (previewer.SelectedBlockOrderChanged || previewer.previewTable == null) {
                previewer.previewTable = previewer.experimentDesign.GetFinalExperimentTable(previewer.SelectedBlockOrderIndex);
                previewer.lastDisplayedOrderIndex = previewer.SelectedBlockOrderIndex;
            }

            EditorGUILayout.Space();
            EditorGUILayout.TextArea(previewer.previewTable.AsString());
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            return previewer.previewTable;
        }
    }
}