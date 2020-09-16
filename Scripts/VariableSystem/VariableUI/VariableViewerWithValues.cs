using bmlTUX.Scripts.VariableSystem.VariableTypes;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace bmlTUX.Scripts.VariableSystem.VariableUI {
    public abstract class VariableViewerWithValues : VariableViewer {
        
        protected ReorderableList valuesList;
        protected SerializedProperty valuesProperty;
        
        protected VariableViewerWithValues(SerializedProperty variableProperty, VariableType variableType) :
            base(variableProperty, variableType) {
            valuesProperty = variableProperty.FindPropertyRelative("Values");
            InitValuesList();
        }
        
        protected void AddValue() {
            valuesProperty.arraySize++;
        }
        
        protected void RemoveValue(int index) {
            valuesProperty.DeleteArrayElementAtIndex(index);
        }

        protected void DrawValueElement(int index, Rect rect) {
            var element = valuesProperty.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, element, GUIContent.none);
        }
        
        void InitValuesList() {
            valuesList = new ReorderableList(valuesProperty.serializedObject, valuesProperty, true, true, true, true);

            valuesList.drawHeaderCallback = rect => {
                DrawValuesHeader(rect);
            };

            valuesList.drawElementCallback = (rect, index, isActive, isFocused) => {
                DrawValueElements(rect, index);
            };
            
            valuesList.onRemoveCallback = list => { RemoveValueElement(list); };
            valuesList.onAddCallback = list => { AddValueElement(); };

        }

        protected abstract void AddValueElement();
        protected abstract void RemoveValueElement(ReorderableList list);
        protected abstract void DrawValueElements(Rect rect, int index);
        protected abstract void DrawValuesHeader(Rect rect);
    }
}