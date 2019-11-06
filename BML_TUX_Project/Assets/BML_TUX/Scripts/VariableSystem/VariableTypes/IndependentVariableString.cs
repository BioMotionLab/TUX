using System;

namespace BML_TUX.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class IndependentVariableString : IndependentVariable<string> {
        public IndependentVariableString() {
            DataType = SupportedDataType.String;
        }
    }
}