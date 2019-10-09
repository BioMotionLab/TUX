using System;
using System.Data;
using BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes;
using BML_ExperimentToolkit.Scripts.VariableSystem.VariableValueAddingStrategies;

namespace BML_ExperimentToolkit.Scripts.VariableSystem {
    [Serializable]
    public abstract class DependentVariable : Variable {
    }


    [Serializable]
    public abstract class DependentVariable<T> : DependentVariable {
        public          T    Value;
        public          T    DefaultValue;
        public override Type Type => typeof(T);

        protected DependentVariable() {
            Name = $"Unnamed DependentVariable Variable (type:{typeof(T)})";
            TypeOfVariable = VariableType.Dependent;

        }

        public override DataTable AddValuesTo(DataTable table) {
            return variableValuesAdderStrategy.AddValuesToCopyOf(table, this);
        }

        readonly DependentVariableValuesAdderStrategy<T> variableValuesAdderStrategy = new DependentVariableValuesAdderStrategy<T>();

    }
}