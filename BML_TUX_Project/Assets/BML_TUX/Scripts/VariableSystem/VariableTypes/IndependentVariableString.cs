using System;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class IndependentVariableString : IndependentVariable<string> {
        public IndependentVariableString() {
            DataType = SupportedDataType.String;
        }
    }
}