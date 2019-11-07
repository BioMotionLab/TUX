using System.Data;
using BML_TUX.Scripts.ExperimentParts;
using BML_TUX.Scripts.UI.Runtime;
using BML_TUX.Scripts.VariableSystem;
using BML_Utilities.Extensions;
using UnityEditor;
using UnityEngine;

namespace BML_TUX.Scripts.UI.Editor {
    public class DesignPreviewer {
        readonly ExperimentDesignFile designFile;
        public int SelectedBlockOrderIndex;
        Vector2 scrollPos;
        readonly ExperimentDesign experimentDesign;
        DataTable previewTable;
        int lastDisplayedOrderIndex = -1 ;
        readonly BlockOrderData blockOrderData;

        bool SelectedBlockOrderChanged => SelectedBlockOrderIndex != lastDisplayedOrderIndex;

        public DesignPreviewer(ExperimentDesignFile designFile) {
            this.designFile = designFile;
            experimentDesign = ExperimentDesign.CreateFrom(designFile);
            blockOrderData = new BlockOrderData(experimentDesign);
        }

        bool DesignFileLinked() {
            bool linked = designFile != null;
            return linked;
        }

        public string ShowRuntimePreview() {
            if (!DesignFileLinked()) return null;
            
            if (designFile.BlockRandomization != BlockRandomizationMode.CompleteRandomization &&
                designFile.BlockRandomization != BlockRandomizationMode.PartialRandomization) {
                EditorGUILayout.LabelField(blockOrderData.BlockOrderText);

                SelectedBlockOrderIndex = blockOrderData.SelectionRequired
                    ? EditorGUILayout.Popup(SelectedBlockOrderIndex,
                                            experimentDesign.BlockPermutationsStrings.ToArray())
                    : SelectedBlockOrderIndex = blockOrderData.DefaultBlockOrderIndex;
            }
            else {
                SelectedBlockOrderIndex = 0;
            }
            
            if (SelectedBlockOrderChanged || previewTable == null) {
                previewTable = experimentDesign.GetFinalExperimentTable(SelectedBlockOrderIndex);
                lastDisplayedOrderIndex = SelectedBlockOrderIndex;
            }

            return previewTable.AsString(separator:"\t\t", truncateLength:5);
        }

        public void ReRandomizeTable() {
            previewTable = experimentDesign.GetFinalExperimentTable(SelectedBlockOrderIndex);
        }

        public DataTable ShowEditorPreview() {
            
            if (designFile != null) {
                EditorGUILayout.LabelField($"Design File Selected: {designFile.name}");
            }
            else {
                EditorGUILayout.HelpBox("Need to have a Design File Selected", MessageType.Warning);
                EditorGUILayout.Space();
            }
            
            
            if (!DesignFileLinked()) return null;

            
            
            
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, 
                                                        false, false, 
                                                        GUILayout.ExpandHeight(true));
            
            
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Preview:", EditorStyles.boldLabel);


            if (designFile.BlockRandomization != BlockRandomizationMode.CompleteRandomization &&
                designFile.BlockRandomization != BlockRandomizationMode.PartialRandomization) {
                EditorGUILayout.LabelField(blockOrderData.BlockOrderText);

                SelectedBlockOrderIndex = blockOrderData.SelectionRequired
                    ? EditorGUILayout.Popup(SelectedBlockOrderIndex,
                                            experimentDesign.BlockPermutationsStrings.ToArray())
                    : SelectedBlockOrderIndex = blockOrderData.DefaultBlockOrderIndex;
            }
            else {
                SelectedBlockOrderIndex = 0;
            }
            
            EditorGUILayout.Space();

            if (GUILayout.Button("Re-randomize")) {
                previewTable = experimentDesign.GetFinalExperimentTable(SelectedBlockOrderIndex);
            }
            
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
        
    }
    
    
    
}