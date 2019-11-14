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
        

        protected override bool DefaultValue => default;
    }
}