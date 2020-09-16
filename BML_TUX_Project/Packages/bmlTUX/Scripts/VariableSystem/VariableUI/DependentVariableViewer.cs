using bmlTUX.Scripts.VariableSystem.VariableTypes;
using UnityEditor;

namespace bmlTUX.Scripts.VariableSystem.VariableUI {
    public class DependentVariableViewer : VariableViewer {
        public DependentVariableViewer(SerializedProperty variableProperty) : base(variableProperty, VariableType.Dependent) { }

        protected override void DrawVariableSpecificInspector() {
            if (ExpandSettingsProp.boolValue) {
                SerializedProperty defaultValueProperty = variableProperty.FindPropertyRelative("DefaultValue");
                EditorGUILayout.PropertyField(defaultValueProperty);
            }
        }
    }
}