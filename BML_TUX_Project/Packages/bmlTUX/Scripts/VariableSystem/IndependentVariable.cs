using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using bmlTUX.Scripts.VariableSystem.VariableTypes;
using bmlTUX.Scripts.VariableSystem.VariableValueAddingStrategies;
using UnityEngine;

namespace bmlTUX.Scripts.VariableSystem {
    [Serializable]
    public abstract class IndependentVariable : Variable {
        public VariableMixingType MixingType;
        public bool               Block;
        public abstract int NumValues { get; }

        public abstract void AddValue();

        public abstract void RemoveValue(int index);
    }

    [Serializable]
    public abstract class IndependentVariable<T> : IndependentVariable{

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
            TypeOfVariable = VariableType.Independent;
        }

        public override int NumValues {
            get {
                foreach (T value in Values) {
                    Debug.Log($"\t{value.ToString()}");
                }
                return Values.Count;
            }
        }

        public override void AddValue() {
            T newValue = Values.Count>0 ? Values[Values.Count-1] : Activator.CreateInstance<T>();
            
            Values.Add(newValue);
            Probabilities.Add(0);
            Debug.Log($"Added value {newValue.ToString()}, values:{Values.Count}, probs:{Probabilities.Count}");
        }

        public override void RemoveValue(int index) {
            T t = Values[index];
            Values.RemoveAt(index);
            Probabilities.RemoveAt(index);
            Debug.Log($"Removed value {t}, values:{Values.Count}, probs:{Probabilities.Count}");
        }

    }
}