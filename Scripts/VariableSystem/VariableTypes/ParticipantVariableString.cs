using System;
using UnityEditor;

namespace bmlTUX.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class ParticipantVariableString : ParticipantVariable<string> {
        public ParticipantVariableString() {
            DataType = SupportedDataType.String;
        }

        public override void SelectValue(string value) {
            Value = value;
        }
        

        protected override string DefaultValue => "defaultValue";
    }
}