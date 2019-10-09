using System;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class IndependentVariableBool : IndependentVariable<bool> {
        public IndependentVariableBool() {
            DataType = SupportedDataType.Bool;
            Values.Add(true);
            Values.Add(false);
        }

        
    }
}