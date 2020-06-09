using System;

namespace bmlTUX.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class IndependentVariableString : IndependentVariable<string> {
        public IndependentVariableString() {
            DataType = SupportedDataType.String;
        }
    }
}