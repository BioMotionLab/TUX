using System;
using UnityEditor;

namespace bmlTUX.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class ParticipantVariableBool : ParticipantVariable<bool> {
        public ParticipantVariableBool() {
            DataType = SupportedDataType.Bool;
        }

        public override void SelectValue(string value) {
            Value = Convert.ToBoolean(value);
        }

        public override void AddValueFieldInEditor() {
            Value = EditorGUILayout.Toggle(Value);
        }

        protected override bool DefaultValue => default;
    }
}