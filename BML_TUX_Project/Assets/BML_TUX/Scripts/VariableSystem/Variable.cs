using System;
using System.Data;
using BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes;

namespace BML_ExperimentToolkit.Scripts.VariableSystem {
    
    [Serializable]
    public abstract class Variable {
        public string Name;

        public          VariableType       TypeOfVariable;
        public          SupportedDataType DataType;
        public abstract Type               Type { get; }
        public abstract DataTable AddValuesTo(DataTable table);
    }
}