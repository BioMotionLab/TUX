using System;
using System.Data;
using bmlTUX.Scripts.VariableSystem.VariableTypes;

namespace bmlTUX.Scripts.VariableSystem {
    
    [Serializable]
    public abstract class Variable {
        public string Name;

        public          VariableType       TypeOfVariable;
        public          SupportedDataType DataType;
        public abstract Type               Type { get; }
        public abstract DataTable AddValuesTo(DataTable table);

        public bool ExpandSettings = true;
    }
}