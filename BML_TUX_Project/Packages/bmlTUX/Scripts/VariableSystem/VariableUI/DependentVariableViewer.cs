using bmlTUX.Scripts.VariableSystem.VariableTypes;
using UnityEditor;

namespace bmlTUX.Scripts.VariableSystem.VariableUI {
    public class DependentVariableViewer : VariableViewer {
        public DependentVariableViewer(SerializedProperty variableProperty) : base(variableProperty, VariableType.Dependent) { }

        protected override void DrawVariableSpecificInspector() {
            
            SerializedProperty defaultValueProperty = VariableProperty.FindPropertyRelative("DefaultValue");
            EditorGUILayout.PropertyField(defaultValueProperty);
            
        }
    }
}