using System;

namespace BML_TUX.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class IndependentVariableInt : IndependentVariable<int> {
        public IndependentVariableInt() {
            DataType = SupportedDataType.Int;
        }
    }
}