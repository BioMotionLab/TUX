using UnityEditor;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.VariableUI {
    /// <inheritdoc />
    /// <summary>
    /// A custom drawer to edit variables
    /// </summary>
    public class VariableDrawer : PropertyDrawer {
        
        protected float CustomPropertyHeight;
        protected bool ShowVariableFoldout = true;

        public override float GetPropertyHeight(SerializedProperty mainProperty, GUIContent label) {
            float propertyBaseHeight = EditorGUI.GetPropertyHeight(mainProperty, GUIContent.none);
            float totalPropertyHeight =  CustomPropertyHeight;
            return totalPropertyHeight;
        }
    }
}