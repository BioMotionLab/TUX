using System;

namespace BML_TUX.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class DependentVariableCustomDataType : DependentVariable<CustomSupportedDataType> {
        public DependentVariableCustomDataType() {
            DataType = SupportedDataType.CustomDataTypeNotYetImplemented;
        }
    }
}