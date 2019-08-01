using System;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class ParticipantVariableFloat : ParticipantVariable<float> {
        public ParticipantVariableFloat() {
            DataType = SupportedDataTypes.Float;
        }

        public override void SelectValue(string value) {
            Value = Convert.ToSingle(value);
        }
    }
}