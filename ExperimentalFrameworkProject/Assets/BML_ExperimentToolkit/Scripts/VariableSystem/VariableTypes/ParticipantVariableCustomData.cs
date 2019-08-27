using System;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class ParticipantVariableCustomData : ParticipantVariable<CustomSupportedDataType> {
        public ParticipantVariableCustomData() {
            DataType = SupportedDataType.CustomDataTypeNotYetImplemented;
        }

        public override void SelectValue(string value) {
            throw new NotImplementedException();
        }
    }
}