using System;
using System.Collections.Generic;
using System.Data;
using BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes;
using BML_ExperimentToolkit.Scripts.VariableSystem.VariableValueAddingStrategies;

namespace BML_ExperimentToolkit.Scripts.VariableSystem {
    [Serializable]
    public abstract class ParticipantVariable : Variable {
    }


    [Serializable]
    public abstract class ParticipantVariable<T> : ParticipantVariable {
        public T Value;
        public bool ConstrainValues;
        public List<T> PossibleValues = new List<T>();
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