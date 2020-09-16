using System;
using System.Collections.Generic;
using System.Data;
using bmlTUX.Scripts.VariableSystem.VariableTypes;
using bmlTUX.Scripts.VariableSystem.VariableValueAddingStrategies;

namespace bmlTUX.Scripts.VariableSystem {
    [Serializable]
    public abstract class ParticipantVariable : Variable {
        public abstract int SelectedValue { get; set; }

        public abstract void SelectValue(string value);
        public abstract string[] PossibleValuesStringArray { get; }
        public abstract bool ValuesAreConstrained { get; }

        public abstract void SetValueDefaultValue();

        public abstract void AddValue();


        public abstract void RemoveValue(int index);


        protected ParticipantVariable() : base(VariableType.Participant) { }
    }


    [Serializable]
    public abstract class ParticipantVariable<T> : ParticipantVariable {
        public T Value;
        public bool ConstrainValues;
        public override bool ValuesAreConstrained => ConstrainValues;
        public List<T> Values = new List<T>();


        public override void SetValueDefaultValue() {
            Value = Values.Count > 0 ? Values[0] : DefaultValue;
        }

        protected abstract T DefaultValue { get; }

        public override string[] PossibleValuesStringArray {
            get {
                List<string> strings = new List<string>();
                foreach (T possibleValue in Values) {
                    strings.Add(possibleValue.ToString());
                }
                return strings.ToArray();
            }
        }

        public override void AddValue() {
            Values.Add(Values[Values.Count-1]);
        }

        public override void RemoveValue(int index) {
            Values.RemoveAt(index);
        }

        int selectedValue;
        public override int SelectedValue {
            get => selectedValue;
            set {
                selectedValue = value;
                Value = Values[value];
            }
        }
        

        public override Type Type => typeof(T);
        ParticipantVariableValuesAdderStrategy<T> variableValuesAdderStrategy;
            
        

        public override DataTable AddValuesTo(DataTable table) {
            if (variableValuesAdderStrategy == null) variableValuesAdderStrategy = new ParticipantVariableValuesAdderStrategy<T>();
            return variableValuesAdderStrategy.AddValuesToCopyOf(table, this);
        }


    }
}