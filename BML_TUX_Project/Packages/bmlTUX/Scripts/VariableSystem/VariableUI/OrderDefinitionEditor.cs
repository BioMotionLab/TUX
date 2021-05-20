using System;
using UnityEditor;
using UnityEditorInternal;
using VariableSystem;

namespace bmlTUX.Scripts.VariableSystem.VariableUI {
    [CustomEditor(typeof(BlockOrderDefinition))]
    public class OrderDefinitionEditor : Editor {

        ReorderableList    list;
        SerializedProperty randomize;
        SerializedProperty isValid;
        SerializedProperty linkedDesignFile;

        void OnEnable() {

            randomize = serializedObject.FindProperty(nameof(BlockOrderDefinition.Randomize));
            isValid = serializedObject.FindProperty(nameof(BlockOrderDefinition.isValid));
            //linkedDesignFile = serializedObject.FindProperty(nameof(BlockOrderDefinition.linkedDesignFile));
            list = new ReorderableList(serializedObject, serializedObject.FindProperty(nameof(BlockOrderDefinition.List)));

            list.drawHeaderCallback = rect => { EditorGUI.LabelField(rect, "Drag to define order (Top to Bottom)"); };
        
            list.drawElementCallback = (rect, index, isActive, isFocused) => {
                var element = list.serializedProperty.GetArrayElementAtIndex(index);
                var text = element.FindPropertyRelative(nameof(OrderRow.Text));
                EditorGUI.LabelField(rect, text.stringValue);
            };
            
        }

        public override void OnInspectorGUI() {
        
            serializedObject.Update();
            
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            BlockOrderDefinition orderDefinition = target as BlockOrderDefinition;
            if (orderDefinition == null) throw new NullReferenceException($"Can't convert to {nameof(BlockOrderDefinition)}");
            if (!orderDefinition.IsValid) { 
                EditorGUILayout.HelpBox("This file is currently invalid because the block variables of its associated designFile have been modified", MessageType.Error); 
            }
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(randomize);
            //EditorGUILayout.PropertyField(linkedDesignFile);
            EditorGUILayout.Space();
            if (!randomize.boolValue) {
                list.DoLayoutList();
            }
            EditorGUILayout.Space();
            
            
            serializedObject.ApplyModifiedProperties();
            
            
        }

    
    }
}