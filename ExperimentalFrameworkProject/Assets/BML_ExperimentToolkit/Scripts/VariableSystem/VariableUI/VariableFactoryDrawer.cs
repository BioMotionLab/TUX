using BML_Utilities;
using UnityEditor;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.VariableUI {
    [CustomPropertyDrawer(typeof(VariableFactory))]
    public class VariableFactoryDrawer : PropertyDrawer {

        const float LineHeight = 20f;
        
        readonly GuiLayoutRect layoutRect = new GuiLayoutRect(LineHeight);

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUI.GetPropertyHeight(property) + layoutRect.FinalHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty mainProperty, GUIContent label) {
            
            //property.serializedObject.Update();
            int oldIndentLevel = EditorGUI.indentLevel;

            layoutRect.NewSetup(position);
            
            Color oldColor = GUI.backgroundColor;
            
            GUI.backgroundColor = Color.white;

            EditorGUI.LabelField(layoutRect.NextLine, "Variable Creation:", EditorStyles.boldLabel);
            AddPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.DataTypesToCreate));
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

            AddPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.IntIVs));
            AddPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.FloatIVs));
            AddPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.StringIVs));
            AddPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.BoolIVs));
            AddPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.GameObjectIVs));
            AddPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.Vector3IVs));
            AddPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.CustomDataTypeIVs));

            //ADD DVs
            EditorGUI.LabelField(layoutRect.NextLine, "--------");
            EditorGUI.LabelField(layoutRect.NextLine, "Dependent Variables:", EditorStyles.boldLabel);

            AddPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.IntDVs));
            AddPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.FloatDVs));
            AddPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.StringDVs));
            AddPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.BoolDVs));
            AddPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.GameObjectDVs));
            AddPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.Vector3DVs));
            AddPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.CustomDataTypeDVs));
            
            //Add Participant variables
            EditorGUI.LabelField(layoutRect.NextLine, "--------");
            EditorGUI.LabelField(layoutRect.NextLine, "Participant Variables:", EditorStyles.boldLabel);

            AddPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.IntParticipantVariables));
            AddPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.FloatParticipantVariables));
            AddPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.StringParticipantVariables));
            AddPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.BoolParticipantVariables));
            AddPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.GameObjectParticipantVariables));
            AddPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.Vector3ParticipantVariables));
            AddPropertyFromName(layoutRect, mainProperty, nameof(VariableFactory.CustomDataParticipantVariables));

            EditorGUI.LabelField(layoutRect.NextLine, "--------");
            EditorGUI.LabelField(layoutRect.NextLine, "Settings:", EditorStyles.boldLabel);
        
            EditorGUI.indentLevel = oldIndentLevel;
            
            mainProperty.serializedObject.ApplyModifiedProperties();
        }


        static void AddPropertyFromName(GuiLayoutRect layoutRect, SerializedProperty property, string name) {
            SerializedProperty propertyFromName = property.FindPropertyRelative(name);
            AddPropertyFromList(layoutRect, propertyFromName);
        }
        
        static void AddPropertyFromList(GuiLayoutRect layoutRect, SerializedProperty valueProperty) {
            
            for (int i = 0; i < valueProperty.arraySize; i++) {
                
                SerializedProperty item = valueProperty.GetArrayElementAtIndex(i);
                
                EditorGUI.PropertyField(layoutRect.NextLine, item, GUIContent.none);
                layoutRect.AddHeight(EditorGUI.GetPropertyHeight(item, GUIContent.none));

                Rect deleteButtonRect = layoutRect.NextLine;
                if (DeleteButton(layoutRect, valueProperty, deleteButtonRect, i)) break;
            }

            valueProperty.serializedObject.ApplyModifiedProperties();
            
        }

        static bool DeleteButton(GuiLayoutRect layoutRect, 
                                 SerializedProperty valueProperty, 
                                 Rect rect,
                                 int i) {
            
            const float deleteButtonHeight = 0.75f;
            float yPadding = (rect.height - (layoutRect.CurrentLine.height * deleteButtonHeight)) / 2f;
            
            Rect smallerDeleteButtonRect = new Rect(rect.x + 40, 
                                                    rect.y + yPadding, 
                                                    125,
                                                    deleteButtonHeight * rect.height);

            // ReSharper disable once InvertIf
            if (GUI.Button(smallerDeleteButtonRect, "Delete Variable")) {
                valueProperty.DeleteArrayElementAtIndex(i);
                return true;
            }

            return false;
        }
    }
}