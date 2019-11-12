using UnityEditor;
using UnityEngine;

namespace BML_TUX.Scripts.VariableSystem.VariableUI {
    /// <inheritdoc />
    /// <summary>
    /// A custom drawer to edit variables
    /// </summary>
    public class VariableDrawer : PropertyDrawer {
        
        protected float CustomPropertyHeight;

        public override float GetPropertyHeight(SerializedProperty mainProperty, GUIContent label) {
            float totalPropertyHeight =  CustomPropertyHeight;
            return totalPropertyHeight;
        }
    }
}