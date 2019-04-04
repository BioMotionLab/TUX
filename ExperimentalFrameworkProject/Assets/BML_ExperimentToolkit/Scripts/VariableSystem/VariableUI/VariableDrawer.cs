using UnityEditor;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.VariableUI {
    /// <inheritdoc />
    /// <summary>
    /// A custom drawer to edit variables
    /// </summary>
    public class VariableDrawer : PropertyDrawer {
        protected float CustomPropertyHeight;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            float propertyBaseHeight = EditorGUI.GetPropertyHeight(property, GUIContent.none);
            float totalPropertyHeight = propertyBaseHeight + CustomPropertyHeight;
            return totalPropertyHeight;
        }
    }
}