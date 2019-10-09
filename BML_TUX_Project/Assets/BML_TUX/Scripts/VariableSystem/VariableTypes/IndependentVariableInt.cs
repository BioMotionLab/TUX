using System;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class IndependentVariableInt : IndependentVariable<int> {
        public IndependentVariableInt() {
            DataType = SupportedDataType.Int;
        }
    }
}