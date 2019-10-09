using BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes;
using UnityEditor;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.VariableUI {
    [CustomPropertyDrawer(typeof(IndependentVariableBool))]
    public class IndependentVariableBoolDrawer : IndependentVariableDrawer {
        public override void OnGUI(Rect mainPosition, SerializedProperty mainProperty, GUIContent label) {
            CustomPropertyHeight = VariableDrawerHelpers.AddAllBoolVariableProperties(mainPosition, mainProperty);
        }
    }
}