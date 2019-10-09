using System;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class IndependentVariableFloat : IndependentVariable<float> {
        public IndependentVariableFloat() {
            DataType = SupportedDataType.Float;
        }
    }
}