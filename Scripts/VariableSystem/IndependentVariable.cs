using System;
using System.Collections.Generic;
using System.Data;
using bmlTUX.Scripts.VariableSystem.VariableTypes;
using bmlTUX.Scripts.VariableSystem.VariableValueAddingStrategies;
using UnityEngine;

namespace bmlTUX.Scripts.VariableSystem {
    [Serializable]
    public abstract class IndependentVariable : Variable {
        public VariableMixingType MixingType;
        public bool               Block;
    }

    [Serializable]
    public abstract class IndependentVariable<T> : IndependentVariable {

        public override Type Type => typeof(T);
        public override DataTable AddValuesTo(DataTable table) {

            if (Values.Count == 0) {
                throw new NullReferenceException($"No values defined for variable: {Name}");
            }

            IndependentVariableValuesAdderStrategy<T> independentVariableValuesAdderStrategy = IndependentValuesStrategyFactory.Create<T>(MixingType);
            return independentVariableValuesAdderStrategy.AddVariableValuesToTable(table, this);
        }

        [SerializeField]
        public List<T> Values;

        [SerializeField]
        public List<float> Probabilities;

        protected IndependentVariable() {
            Values = new List<T>();
            Probabilities = new List<float>();
            Name = UnnamedColumn.Name;
            TypeOfVariable = VariableType.Independent;
        }

    }
}