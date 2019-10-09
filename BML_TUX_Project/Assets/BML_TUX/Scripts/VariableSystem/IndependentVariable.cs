using System;
using System.Collections.Generic;
using System.Data;
using BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes;
using BML_ExperimentToolkit.Scripts.VariableSystem.VariableValueAddingStrategies;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.VariableSystem {
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
                throw new ArgumentNullException($"Can't add values, none defined for variable: {Name}");
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
            Name = $"Unnamed Variable (types:{typeof(T)})";
            TypeOfVariable = VariableType.Independent;
        }

    }
}