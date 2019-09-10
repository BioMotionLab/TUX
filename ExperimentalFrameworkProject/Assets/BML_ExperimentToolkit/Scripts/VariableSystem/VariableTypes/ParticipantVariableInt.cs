using System;
using UnityEditor;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class ParticipantVariableInt : ParticipantVariable<int> {
        public ParticipantVariableInt() {
            DataType = SupportedDataType.Int;
        }

        public override void SelectValue(string value) {
            Value = Convert.ToInt32(value);
        }

        public override void AddValueFieldInEditor() {
            Value = EditorGUILayout.IntField(Value);
        }
    }
}