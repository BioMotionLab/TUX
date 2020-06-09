using System;

namespace bmlTUX.Scripts.VariableSystem.VariableTypes {

    [Serializable]
    public class IndependentVariableCustomDataType : IndependentVariable<CustomSupportedDataType> {
        public IndependentVariableCustomDataType() {
            DataType = SupportedDataType.CustomDataTypeNotYetImplemented;
        }
    }
}