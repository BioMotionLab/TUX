using System;

namespace bmlTUX.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class IndependentVariableFloat : IndependentVariable<float> {
        public IndependentVariableFloat() {
            DataType = SupportedDataType.Float;
        }
    }
}