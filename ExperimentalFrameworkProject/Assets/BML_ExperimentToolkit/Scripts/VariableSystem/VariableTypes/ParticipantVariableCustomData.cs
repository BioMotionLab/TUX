using System;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class ParticipantVariableCustomData : ParticipantVariable<CustomSupportedDataType> {
        public ParticipantVariableCustomData() {
            DataType = SupportedDataTypes.CustomDataType_NotYetImplemented;
        }
    }
}