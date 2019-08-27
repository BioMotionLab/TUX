using BML_Utilities;
using UnityEditor;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.VariableUI {
    [CustomPropertyDrawer(typeof(VariableFactory))]
    public class VariableFactoryDrawer : PropertyDrawer {

        const float LineHeight = 20f;
        float height;
        

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUI.GetPropertyHeight(property) + height;
        }

        public override void OnGUI(Rect mainPosition, SerializedProperty mainProperty, GUIContent label) {
            
            //property.serializedObject.Update();
            int oldIndentLevel = EditorGUI.indentLevel;
            
            GuiLayoutRect layoutRect = new GuiLayoutRect(LineHeight, mainPosition);
            
            Color oldColor = GUI.backgroundColor;
            
            GUI.backgroundColor = Color.white;

            EditorGUI.LabelField(layoutRect.NextLine, "Variable Creation:", EditorStyles.boldLabel);
            AddPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.DataTypeToCreate));
            AddPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.VariableTypeToCreate));
            
            if (GUI.Button(layoutRect.NextLine, "Create Variable")) {
                VariableFactory factory =
                    fieldInfo.GetValue(mainProperty.serializedObject.targetObject) as VariableFactory;
                factory?.AddNew();
                
            }
            
            GUI.backgroundColor = oldColor;

            mainProperty.serializedObject.ApplyModifiedProperties();


            //ADD IVs
            EditorGUI.LabelField(layoutRect.NextLine, "--------");
            EditorGUI.LabelField(layoutRect.NextLine, "Independent Variables:", EditorStyles.boldLabel);

            AddListPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.IntIVs));
            AddListPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.FloatIVs));
            AddListPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.StringIVs));
            AddListPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.BoolIVs));
            AddListPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.GameObjectIVs));
            AddListPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.Vector2IVs));
            AddListPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.Vector3IVs));
            AddListPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.CustomDataTypeIVs));

            //ADD DVs
            EditorGUI.LabelField(layoutRect.NextLine, "--------");
            EditorGUI.LabelField(layoutRect.NextLine, "Dependent Variables:", EditorStyles.boldLabel);

            AddListPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.IntDVs));
            AddListPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.FloatDVs));
            AddListPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.StringDVs));
            AddListPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.BoolDVs));
            AddListPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.GameObjectDVs));
            AddListPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.Vector2DVs));
            AddListPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.Vector3DVs));
            AddListPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.CustomDataTypeDVs));
            
            //Add Participant variables
            EditorGUI.LabelField(layoutRect.NextLine, "--------");
            EditorGUI.LabelField(layoutRect.NextLine, "Participant Variables:", EditorStyles.boldLabel);

            AddListPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.IntParticipantVariables));
            AddListPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.FloatParticipantVariables));
            AddListPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.StringParticipantVariables));
            AddListPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.BoolParticipantVariables));
            AddListPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.GameObjectParticipantVariables));
            AddListPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.Vector2ParticipantVariables));
            AddListPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.Vector3ParticipantVariables));
            AddListPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.CustomDataParticipantVariables));

            EditorGUI.LabelField(layoutRect.NextLine, "--------");
            EditorGUI.LabelField(layoutRect.NextLine, "[Advanced] Generate Design File Manually");
            if (GUI.Button(layoutRect.NextLine, "Generate...")) {
                DesignSaverWindow.ShowWindow();
            }
            
            EditorGUI.LabelField(layoutRect.NextLine, "--------");
            EditorGUI.LabelField(layoutRect.NextLine, "Settings:", EditorStyles.boldLabel);
        
            
            
            
            EditorGUI.indentLevel = oldIndentLevel;
            height = layoutRect.FinalHeight;
            mainProperty.serializedObject.ApplyModifiedProperties();
        }


        static void AddListPropertyFromName(GuiLayoutRect layoutRect, SerializedProperty property, string name) {
            SerializedProperty propertyFromName = property.FindPropertyRelative(name);
            AddPropertyFromList(layoutRect, propertyFromName);
        }
        
        static void AddPropertyFromName(GuiLayoutRect layoutRect, SerializedProperty property, string name) {
            SerializedProperty propertyFromName = property.FindPropertyRelative(name);
            EditorGUI.PropertyField(layoutRect.NextLine, propertyFromName, GUIContent.none);
        }
        
        static void AddPropertyFromList(GuiLayoutRect layoutRect, SerializedProperty valueProperty) {
            
            for (int i = 0; i < valueProperty.arraySize; i++) {
                
                SerializedProperty item = valueProperty.GetArrayElementAtIndex(i);
                
                EditorGUI.PropertyField(layoutRect.NextLine, item, GUIContent.none);
                layoutRect.AddHeight(EditorGUI.GetPropertyHeight(item, GUIContent.none));

                if (DeleteButton(layoutRect, valueProperty, i)) break;
            }

            valueProperty.serializedObject.ApplyModifiedProperties();
            
        }

        static bool DeleteButton(GuiLayoutRect layoutRect, 
                                 SerializedProperty valueProperty, 
                                 int i) {

            Rect deleteBaseRect = layoutRect.NextLine;
            const float deleteButtonHeight = 0.75f;
            float yPadding = (deleteBaseRect.height - (layoutRect.CurrentLine.height * deleteButtonHeight)) / 2f;
            
            Rect smallerDeleteButtonRect = new Rect(deleteBaseRect.x + 40, 
                                                    deleteBaseRect.y + yPadding, 
                                                    125,
                                                    deleteButtonHeight * deleteBaseRect.height);

            // ReSharper disable once InvertIf
            if (GUI.Button(smallerDeleteButtonRect, "Delete Variable")) {
                valueProperty.DeleteArrayElementAtIndex(i);
                return true;
            }

            return false;
        }
    }
}