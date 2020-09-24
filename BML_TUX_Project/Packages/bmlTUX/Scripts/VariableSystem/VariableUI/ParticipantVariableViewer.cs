using bmlTUX.Scripts.VariableSystem.VariableTypes;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace bmlTUX.Scripts.VariableSystem.VariableUI {
    public class ParticipantVariableViewer : VariableViewerWithValues {

        public ParticipantVariableViewer(SerializedProperty variableProperty) 
            : base(variableProperty,
            VariableType.Participant) {
            
        }

        protected override void DrawVariableSpecificInspector() {
            

                EditorGUI.indentLevel++;
                
                SerializedProperty constrainProperty = VariableProperty.FindPropertyRelative("ConstrainValues");
                EditorGUILayout.PropertyField(constrainProperty);

                if (constrainProperty.boolValue) {
                    EditorGUILayout.LabelField("Possible Values");
                    valuesList.DoLayoutList();
                }
                EditorGUI.indentLevel--;
            
        }

        protected override void AddValueElement() {
            AddValue();
        }

        protected override void RemoveValueElement(ReorderableList list) {
            RemoveValue(list.index);
        }

        protected override void DrawValueElements(Rect rect, int index) {
            DrawValueElement(index, rect);
        }

        protected override void DrawValuesHeader(Rect rect) {
            Rect leftRect = new Rect(rect.position, rect.size);
            EditorGUI.LabelField(leftRect, "Values");
        }
    }

    
    
}