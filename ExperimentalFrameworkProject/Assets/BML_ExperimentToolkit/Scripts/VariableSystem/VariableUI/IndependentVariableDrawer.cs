using UnityEditor;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.VariableUI {
    /// <inheritdoc />
    /// <summary>
    /// A custom drawer to edit independent variables
    /// </summary>
    public class IndependentVariableDrawer : VariableDrawer {
        public override void OnGUI(Rect mainPosition, SerializedProperty mainProperty, GUIContent label) {
            CustomPropertyHeight = VariableDrawerHelpers.AddAllIndependentVariableProperties(mainPosition, mainProperty);
        }
    }
}