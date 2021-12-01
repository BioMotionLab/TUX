using System;
using bmlTUX.Scripts.VariableSystem;

namespace bmlTUX.VariableTypes {
    [Serializable]
    public class IndependentVariableFloat : IndependentVariable<float> {
        public IndependentVariableFloat() {
            DataType = SupportedDataType.Float;
        }
    }
}