using System;
using System.Data;
using bmlTUX.Scripts.VariableSystem.VariableTypes;

namespace bmlTUX.Scripts.VariableSystem {
    
    [Serializable]
    public abstract class Variable {
        public const string UnnamedName = "Unamed_Variable";
        public static int UnamedCounter = 0;

        public static string GetUniqueName() {
            string newName = UnnamedName + "_" + UnamedCounter.ToString("D4");
            UnamedCounter++;
            return newName;
        }

        protected Variable(VariableType variableType) {
            VariableType = variableType;
            name = GetUniqueName();
            Name = name;
        }

        string name;
        public string Name;

        public          VariableType       VariableType;
        public          SupportedDataType DataType;
        public abstract Type               Type { get; }
        public abstract DataTable AddValuesTo(DataTable table);

        public bool ExpandSettings = true;
    }
}