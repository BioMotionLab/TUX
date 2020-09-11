using UnityEditor;
using UnityEditorInternal;

namespace bmlTUX.Scripts.VariableSystem.VariableUI {
    [CustomEditor(typeof(BlockOrderDefinition))]
    public class OrderDefinitionEditor : Editor {

        ReorderableList    list;
        SerializedProperty randomize;
        void OnEnable() {

            randomize = serializedObject.FindProperty(nameof(BlockOrderDefinition.Randomize));
        
            list = new ReorderableList(serializedObject, serializedObject.FindProperty(nameof(BlockOrderDefinition.List)));

            list.drawHeaderCallback = rect => { EditorGUI.LabelField(rect, "Drag to define order (Top to Bottom)"); };
        
            list.drawElementCallback = (rect, index, isActive, isFocused) => {
                var element = list.serializedProperty.GetArrayElementAtIndex(index);
                var text = element.FindPropertyRelative(nameof(OrderRow.Text));
                EditorGUI.LabelField(rect, text.stringValue);
            };
            
        }

        public override void OnInspectorGUI() {
        
            serializedObject.Update();

            if (!randomize.boolValue) {
                list.DoLayoutList();
            }
        
            EditorGUILayout.PropertyField(randomize);
        
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }

    
    }
}