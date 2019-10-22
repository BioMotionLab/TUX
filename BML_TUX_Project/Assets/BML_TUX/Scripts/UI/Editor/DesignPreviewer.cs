using System.Data;
using BML_ExperimentToolkit.Scripts.ExperimentParts;
using BML_ExperimentToolkit.Scripts.UI.Editor;
using BML_ExperimentToolkit.Scripts.VariableSystem;
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
            bool linked = false;
            if (designFile != null) {
                EditorGUILayout.LabelField($"Design File Selected: {designFile.name}");
                linked = true;
            }
            else {
                EditorGUILayout.HelpBox("Need to have a Design File Selected", MessageType.Warning);
                EditorGUILayout.Space();
            }

            return linked;
        }

        public DataTable ShowPreview() {
            
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