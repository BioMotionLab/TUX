using System;
using System.Data;
using BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.VariableSystem {
    
    [Serializable]
    public abstract class Variable {
        public string Name;

        public          VariableType       TypeOfVariable;
        public          SupportedDataTypes DataType;
        public abstract Type               Type { get; }
        public abstract DataTable AddValuesTo(DataTable table);
    }
}