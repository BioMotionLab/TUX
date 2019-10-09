using System;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes {

    [Serializable]
    public class IndependentVariableCustomDataType : IndependentVariable<CustomSupportedDataType> {
        public IndependentVariableCustomDataType() {
            DataType = SupportedDataType.CustomDataTypeNotYetImplemented;
        }
    }
}