using UnityEditor;
using UnityEngine;

namespace BML_TUX.Scripts.VariableSystem.VariableUI {
    /// <inheritdoc />
    /// <summary>
    /// A custom drawer to edit dependent variables
    /// </summary>
    public class DependentVariableDrawer : VariableDrawer {
        public override void OnGUI(Rect position, SerializedProperty mainProperty, GUIContent label) {
            CustomPropertyHeight = VariableDrawerHelpers.AddAllDependentVariableProperties(position, mainProperty);
        }
    }
}