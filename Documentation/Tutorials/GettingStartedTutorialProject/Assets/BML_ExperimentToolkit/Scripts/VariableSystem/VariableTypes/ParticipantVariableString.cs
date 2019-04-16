using System;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class ParticipantVariableString : ParticipantVariable<string> {
        public ParticipantVariableString() {
            DataType = SupportedDataTypes.String;
        }
    }
}