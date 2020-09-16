using System;
using System.Data;
using bmlTUX.Scripts.VariableSystem.VariableTypes;
using bmlTUX.Scripts.VariableSystem.VariableValueAddingStrategies;

namespace bmlTUX.Scripts.VariableSystem {
    [Serializable]
    public abstract class DependentVariable : Variable {
        protected DependentVariable() : base(VariableType.Dependent) { }
    }


    [Serializable]
    public abstract class DependentVariable<T> : DependentVariable {
        public          T    Value;
        public          T    DefaultValue;
        public override Type Type => typeof(T);
        DependentVariableValuesAdderStrategy<T> variableValuesAdderStrategy;
        
        public override DataTable AddValuesTo(DataTable table) {
            if (variableValuesAdderStrategy == null) variableValuesAdderStrategy = new DependentVariableValuesAdderStrategy<T>();
            DataTable tableWithValues = variableValuesAdderStrategy.AddValuesToCopyOf(table, this);
            return tableWithValues;
        }

        

    }
}