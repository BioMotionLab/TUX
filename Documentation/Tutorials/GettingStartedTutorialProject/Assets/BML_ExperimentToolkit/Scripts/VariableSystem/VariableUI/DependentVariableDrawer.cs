using UnityEditor;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.VariableUI {
    /// <inheritdoc />
    /// <summary>
    /// A custom drawer to edit dependent variables
    /// </summary>
    public class DependentVariableDrawer : VariableDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            CustomPropertyHeight = VariableDrawerHelpers.AddAllDependentVariableProperties(position, property);
        }
    }
}