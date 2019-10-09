using System;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace BML_Utilities {
    
    /// <summary>
    /// from: http://www.brechtos.com/hiding-or-disabling-inspector-properties-using-propertydrawers-within-unity-5/
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
                    AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
    public class ConditionalHideAttribute : PropertyAttribute
    {
        //The name of the bool field that will be in control
        public readonly string ConditionalSourceField = default;
        
        //TRUE = Hide in inspector / FALSE = Disable in inspector 
        public readonly bool HideInInspector = default;

        [PublicAPI]
        public ConditionalHideAttribute(string conditionalSourceField)
        {
            ConditionalSourceField = conditionalSourceField;
            HideInInspector = false;
        }

        [PublicAPI]
        public ConditionalHideAttribute(string conditionalSourceField, bool hideInInspector)
        {
            ConditionalSourceField = conditionalSourceField;
            HideInInspector = hideInInspector;
        }
    }

    /// <summary>
    /// From http://www.brechtos.com/hiding-or-disabling-inspector-properties-using-propertydrawers-within-unity-5/
    /// </summary>
    [CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
    public class ConditionalHidePropertyDrawer : PropertyDrawer {
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            
            ConditionalHideAttribute conditionalHideAttribute = (ConditionalHideAttribute)attribute;
            bool enabled = GetConditionalHideAttributeResult(conditionalHideAttribute, property);
 
            if (!conditionalHideAttribute.HideInInspector || enabled) {
                return EditorGUI.GetPropertyHeight(property, label);
            }
            else {
                //The property is not being drawn
                //We want to undo the spacing added before and after the property
                return -EditorGUIUtility.standardVerticalSpacing;
            }
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            
            ConditionalHideAttribute conditionalHideAttribute = (ConditionalHideAttribute)attribute;
            
            bool enabled = GetConditionalHideAttributeResult(conditionalHideAttribute, property);

            //Enable/disable the property
            bool wasEnabled = GUI.enabled;
            GUI.enabled = enabled;

            //Check if we should draw the property
            if (!conditionalHideAttribute.HideInInspector || enabled) {
                EditorGUI.PropertyField(position, property, label, true);
            }

            //Ensure that the next property that is being drawn uses the correct settings
            GUI.enabled = wasEnabled;
        }
        
        bool GetConditionalHideAttributeResult(ConditionalHideAttribute conditionalHideAttribute, SerializedProperty property)  {
            bool enabled = true;
            
            string propertyPath = property.propertyPath; //returns the property path of the property we want to apply the attribute to
            string conditionPath = propertyPath.Replace(
                                                        property.name, 
                                                        conditionalHideAttribute.ConditionalSourceField); //changes the path to the conditionalsource property path
            
            SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);
 
            if (sourcePropertyValue != null) {
                enabled = sourcePropertyValue.boolValue;

                if (!sourcePropertyValue.boolValue) {
                    property.boolValue = false;
                }
                
            }
            else {
                Debug.LogWarning("Attempting to use a ConditionalHideAttribute but " +
                                 $"no matching SourcePropertyValue found in object: " +
                                 $"{conditionalHideAttribute.ConditionalSourceField}");
            }
 
            return enabled;
        }
    }

}

