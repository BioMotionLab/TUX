using System;

namespace BML_ExperimentToolkit.Scripts.VariableSystem {
    [Serializable]
    public enum VariableMixingType {
        Balanced,
        Looped,
        EvenProbability,
        CustomProbability
    }
}