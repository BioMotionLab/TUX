using System;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class ParticipantVariableInt : ParticipantVariable<int> {
        public ParticipantVariableInt() {
            DataType = SupportedDataTypes.Int;
        }

        public override void SelectValue(string value) {
            Value = Convert.ToInt32(value);
        }
    }
}