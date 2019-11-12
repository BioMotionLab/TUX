using System.Data;
using BML_Utilities.Extensions;
using bmlTUX.Scripts.ExperimentParts;
using bmlTUX.Scripts.VariableSystem;
using UnityEditor;
using UnityEngine;

namespace bmlTUX.Scripts.UI.Runtime {
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

        public DataTable GetPreview(int blockOrderIndex) {
            
            if (!DesignFileLinked()) return null;
            
            if (SelectedBlockOrderChanged || previewTable == null) {
                previewTable = experimentDesign.GetFinalExperimentTable(SelectedBlockOrderIndex);
                lastDisplayedOrderIndex = SelectedBlockOrderIndex;
            }
            
            return previewTable;
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
                ReRandomizeTable();
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