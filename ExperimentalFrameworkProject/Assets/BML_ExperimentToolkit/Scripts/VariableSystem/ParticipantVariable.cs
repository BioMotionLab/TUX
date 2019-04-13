using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes;
using BML_ExperimentToolkit.Scripts.VariableSystem.VariableValueAddingStrategies;

namespace BML_ExperimentToolkit.Scripts.VariableSystem {
    [Serializable]
    public abstract class ParticipantVariable : Variable {
        public abstract int SelectedValue { get; set; }
        public abstract string[] PossibleValuesStringArray { get; }
        public abstract bool ValuesAreConstrained { get; }
    }


    [Serializable]
    public abstract class ParticipantVariable<T> : ParticipantVariable {
        public T Value;
        public bool ConstrainValues;
        public override bool ValuesAreConstrained => ConstrainValues;
        public List<T> PossibleValues = new List<T>();

        public override string[] PossibleValuesStringArray {
            get {
                List<string> strings = new List<string>();
                foreach (T possibleValue in PossibleValues) {
                    strings.Add(possibleValue.ToString());
                }
                return strings.ToArray();
            }
        }

        int selectedValue;
        public override int SelectedValue {
            get => selectedValue;
            set {
                selectedValue = value;
                Value = PossibleValues[value];
            }
        }

        public override Type Type => typeof(T);
        readonly ParticipantVariableValuesAdderStrategy<T> variableValuesAdderStrategy 
            = new ParticipantVariableValuesAdderStrategy<T>();

        protected ParticipantVariable() {
            Name = $"Unnamed ParticipantVariable";
            TypeOfVariable = VariableType.Participant;
        }

        public override DataTable AddValuesTo(DataTable table) {
            return variableValuesAdderStrategy.AddValuesToCopyOf(table, this);
        }


    }
}