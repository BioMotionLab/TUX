using BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes;
using UnityEditor;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.VariableUI {
    [CustomPropertyDrawer(typeof(IndependentVariableBool))]
    public class IndependentVariableBoolDrawer : IndependentVariableDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            CustomPropertyHeight = VariableDrawerHelpers.AddAllBoolVariableProperties(position, property);
        }
    }
}