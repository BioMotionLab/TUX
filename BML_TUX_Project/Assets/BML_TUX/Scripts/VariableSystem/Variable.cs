using System;
using System.Data;
using BML_TUX.Scripts.VariableSystem.VariableTypes;

namespace BML_TUX.Scripts.VariableSystem {
    
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