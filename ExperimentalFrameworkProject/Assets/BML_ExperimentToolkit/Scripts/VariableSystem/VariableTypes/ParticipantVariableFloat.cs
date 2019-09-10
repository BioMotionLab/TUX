using System;
using UnityEditor;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class ParticipantVariableFloat : ParticipantVariable<float> {
        public ParticipantVariableFloat() {
            DataType = SupportedDataType.Float;
        }

        public override void SelectValue(string value) {
            Value = Convert.ToSingle(value);
        }

        public override void AddValueFieldInEditor() {
            Value = EditorGUILayout.FloatField(Value);
        }
    }
}