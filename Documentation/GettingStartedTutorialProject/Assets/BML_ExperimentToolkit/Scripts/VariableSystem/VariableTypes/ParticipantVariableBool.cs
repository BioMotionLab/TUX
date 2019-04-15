using System;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class ParticipantVariableBool : ParticipantVariable<bool> {
        public ParticipantVariableBool() {
            DataType = SupportedDataTypes.Bool;
        }
    }
}