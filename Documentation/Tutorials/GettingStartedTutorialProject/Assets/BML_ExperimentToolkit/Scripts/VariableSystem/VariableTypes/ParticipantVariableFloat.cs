using System;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class ParticipantVariableFloat : ParticipantVariable<float> {
        public ParticipantVariableFloat() {
            DataType = SupportedDataTypes.Float;
        }
    }
}