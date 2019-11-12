using System;

namespace bmlTUX.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class ParticipantVariableCustomData : ParticipantVariable<CustomSupportedDataType> {
        public ParticipantVariableCustomData() {
            DataType = SupportedDataType.CustomDataTypeNotYetImplemented;
        }

        public override void SelectValue(string value) {
            throw new NotImplementedException();
        }

        public override void AddValueFieldInEditor() {
            throw new NotImplementedException();
        }

        protected override CustomSupportedDataType DefaultValue => throw new NotImplementedException("custom data type not yet implemented");
    }
}