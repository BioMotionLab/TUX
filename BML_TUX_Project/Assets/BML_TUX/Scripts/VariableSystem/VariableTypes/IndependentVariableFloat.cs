using System;

namespace BML_TUX.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class IndependentVariableFloat : IndependentVariable<float> {
        public IndependentVariableFloat() {
            DataType = SupportedDataType.Float;
        }
    }
}