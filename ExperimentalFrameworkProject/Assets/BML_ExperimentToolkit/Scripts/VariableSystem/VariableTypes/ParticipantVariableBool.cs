using System;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class ParticipantVariableBool : ParticipantVariable<bool> {
        public ParticipantVariableBool() {
            DataType = SupportedDataTypes.Bool;
        }

        public override void SelectValue(string value) {
            Value = Convert.ToBoolean(value);
        }
    }
}