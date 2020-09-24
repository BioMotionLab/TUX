using System;
using bmlTUX.Scripts.VariableSystem.VariableTypes;
using UnityEditor;
using UnityEngine;

namespace bmlTUX.Scripts.VariableSystem.VariableUI {
    public abstract class VariableViewer {
        
        const int DeleteButtonWidth     = 100;
        const int DataTypeWidth = 50;
        const int NameWidth = 125;
        const int NameLabelWidth = 40;
        
        protected readonly SerializedProperty VariableProperty;
        
        public VariableType VariableType { get; }

        public bool ReadyToDelete = false;
        public bool Deleted;
        protected VariableValidationResults VariableValidationResults;
        bool deleteDialogIsOpen = false;


        public VariableViewer(SerializedProperty variableProperty, VariableType variableType) {
            
            this.VariableProperty = variableProperty ?? throw new NullReferenceException("VariableProperty is null");
            this.VariableType = variableType;

        }
        
        protected abstract void DrawVariableSpecificInspector();
        
        public void DrawInspector() {
            
            if (Deleted) throw new NullReferenceException("Trying To Draw deleted variable inspector");
            if (deleteDialogIsOpen) return;
            
            VariableValidationResults = new VariableValidationResults();

            
            int oldIndentLevel = EditorGUI.indentLevel;
            
            EditorGUI.indentLevel = 0;
            
            
            EditorGUILayout.BeginVertical(EditorGuiHelper.AlternateColorBox);
            
            SerializedProperty expandSettingsProp = VariableProperty.FindPropertyRelative(nameof(Variable.ExpandSettings));
            bool expanded;
            try {
                expanded = expandSettingsProp.boolValue;
            }
            catch (InvalidOperationException) {
                // this can happen if variable is deleted from the dialog, and viewer still gets called. Should be fixed by deleteDialogIsOpen check above. but keeping just in case. 
                return;
            }
            
            DrawVariableHeader(expandSettingsProp);

            EditorGUI.indentLevel++;
            if (expanded) DrawVariableSpecificInspector();
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            if (!VariableValidationResults.IsValid) {
                DrawVariableErrors();
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            
            EditorGUI.indentLevel = oldIndentLevel;

            DrawVariableDeleteButton();
            
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            

        }

        void DrawVariableErrors() {
            EditorGUI.indentLevel++;
            foreach (string errorText in VariableValidationResults.Errors) {
                EditorGUILayout.HelpBox(errorText, MessageType.Error);
            }

            foreach (string warningText in VariableValidationResults.Warnings) {
                EditorGUILayout.HelpBox(warningText, MessageType.Warning);
            }
            EditorGUI.indentLevel--;
        }

        void DrawVariableHeader(SerializedProperty expandViewerProp) {
            
            SerializedProperty variableName = VariableProperty.FindPropertyRelative(nameof(Variable.Name));
            SupportedDataType dataType = (SupportedDataType) VariableProperty.FindPropertyRelative(nameof(Variable.DataType)).intValue;

            EditorGUILayout.BeginHorizontal();
            
            if (expandViewerProp.boolValue) {
                EditorGUILayout.PropertyField(variableName, GUIContent.none, GUILayout.Width(NameWidth));
            }
            else {
                EditorGUILayout.LabelField(variableName.stringValue, GUILayout.Width(NameWidth));
            }
            
            VariableNameValidator.Validate(variableName.stringValue, VariableValidationResults);
            
            
            EditorGUILayout.LabelField($"{dataType}", GUILayout.Width(DataTypeWidth));
            GUILayout.FlexibleSpace();
            
            if (expandViewerProp.boolValue) {
                if (GUILayout.Button("Hide Settings", GUILayout.Width(DeleteButtonWidth))) {
                    expandViewerProp.boolValue = false;
                }
            }
            else {
                if (GUILayout.Button("Edit Settings", GUILayout.Width(DeleteButtonWidth))) {
                    expandViewerProp.boolValue = true;
                }
            }

            EditorGUILayout.EndHorizontal();
            
        }


        void DrawVariableDeleteButton() {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Delete Variable", GUILayout.Width(DeleteButtonWidth))) {
                deleteDialogIsOpen = true;
                //Debug.Log("Showing popup");
                if (EditorUtility.DisplayDialog("Delete Variable", "Are you sure you want to delete variable?",
                    "Delete Variable", "Cancel")) {
                    if (Deleted) Debug.LogError("Trying to delete already deleted variable");
                    ReadyToDelete = true;
                    //Debug.Log("deleting...");
                }

                deleteDialogIsOpen = false;
                GUIUtility.ExitGUI(); 
            }
            EditorGUILayout.EndHorizontal();
        }
        

        
        
    }
    
    
    
}