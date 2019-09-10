using System;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class ParticipantVariableString : ParticipantVariable<string> {
        public ParticipantVariableString() {
            DataType = SupportedDataType.String;
        }

        public override void SelectValue(string value) {
            Value = value;
        }
    }
}