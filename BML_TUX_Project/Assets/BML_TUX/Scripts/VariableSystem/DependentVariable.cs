using System;
using System.Data;
using BML_TUX.Scripts.VariableSystem.VariableTypes;
using BML_TUX.Scripts.VariableSystem.VariableValueAddingStrategies;

namespace BML_TUX.Scripts.VariableSystem {
    [Serializable]
    public abstract class DependentVariable : Variable {
    }


    [Serializable]
    public abstract class DependentVariable<T> : DependentVariable {
        public          T    Value;
        public          T    DefaultValue;
        public override Type Type => typeof(T);

        protected DependentVariable() {
            Name = UnnamedColumn.Name;
            TypeOfVariable = VariableType.Dependent;

        }

        public override DataTable AddValuesTo(DataTable table) {
            return variableValuesAdderStrategy.AddValuesToCopyOf(table, this);
        }

        readonly DependentVariableValuesAdderStrategy<T> variableValuesAdderStrategy = new DependentVariableValuesAdderStrategy<T>();

    }
}