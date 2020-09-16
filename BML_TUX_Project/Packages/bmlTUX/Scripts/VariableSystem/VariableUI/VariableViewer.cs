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
        
        protected readonly SerializedProperty variableProperty;
        protected readonly SerializedProperty ExpandSettingsProp;

        public VariableType VariableType => variableType;
        protected readonly VariableType variableType;

        public bool ReadyToDelete = false;
        public bool Deleted;
        protected VariableValidationResults variableValidationResults;


        public VariableViewer(SerializedProperty variableProperty, VariableType variableType) {
            
            this.variableProperty = variableProperty ?? throw new NullReferenceException("VariableProperty is null");
            this.variableType = variableType;
            
            ExpandSettingsProp = variableProperty.FindPropertyRelative(nameof(Variable.ExpandSettings));
            
        }
        
        protected abstract void DrawVariableSpecificInspector();
        
        public void DrawInspector() {
            
            if (Deleted) throw new NullReferenceException("Trying To Draw deleted variable inspector");
            variableValidationResults = new VariableValidationResults();

            int oldIndentLevel = EditorGUI.indentLevel;
            
            EditorGUI.indentLevel = 0;
            
            
            EditorGUILayout.BeginVertical(EditorGuiHelper.AlternateColorBox);
            
            DrawVariableHeader();

            EditorGUI.indentLevel++;
            DrawVariableSpecificInspector();
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            
            
         
            
            
            if (!variableValidationResults.IsValid) {
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
            foreach (string errorText in variableValidationResults.Errors) {
                EditorGUILayout.HelpBox(errorText, MessageType.Error);
            }

            foreach (string warningText in variableValidationResults.Warnings) {
                EditorGUILayout.HelpBox(warningText, MessageType.Warning);
            }
            EditorGUI.indentLevel--;
        }

        void DrawVariableHeader() {
            
            SerializedProperty variableName = variableProperty.FindPropertyRelative(nameof(Variable.Name));
            SupportedDataType dataType = (SupportedDataType) variableProperty.FindPropertyRelative(nameof(Variable.DataType)).intValue;

            EditorGUILayout.BeginHorizontal();
            
            if (ExpandSettingsProp.boolValue) {
                EditorGUILayout.PropertyField(variableName, GUIContent.none, GUILayout.Width(NameWidth));
            }
            else {
                EditorGUILayout.LabelField(variableName.stringValue, GUILayout.Width(NameWidth));
            }
            
            VariableNameValidator.Validate(variableName.stringValue, variableValidationResults);
            
            
            EditorGUILayout.LabelField($"{dataType}", GUILayout.Width(DataTypeWidth));
            
            if (ExpandSettingsProp.boolValue) {
                if (GUILayout.Button("Hide Settings", GUILayout.Width(DeleteButtonWidth))) {
                    ExpandSettingsProp.boolValue = false;
                }
            }
            else {
                if (GUILayout.Button("Edit Settings", GUILayout.Width(DeleteButtonWidth))) {
                    ExpandSettingsProp.boolValue = true;
                }
            }

            EditorGUILayout.EndHorizontal();
            
        }


        void DrawVariableDeleteButton() {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Delete Variable", GUILayout.Width(DeleteButtonWidth))) {
                if (EditorUtility.DisplayDialog("Delete Variable", "Are you sure you want to delete variable?",
                    "Delete Variable", "Cancel")) {
                    if (Deleted) Debug.LogError("Trying to delete already deleted variable");
                    ReadyToDelete = true;
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        

        
        
    }
    
    
    
}