using System;
using System.Collections.Generic;
using BML_ExperimentToolkit.Scripts.ExperimentParts;
using BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes;

namespace BML_ExperimentToolkit.Scripts.VariableSystem {


    public class SortedVariableContainer
    {
        public readonly List<IndependentVariable> BalancedIndependentVariables    = new List<IndependentVariable>();
        public readonly List<IndependentVariable> LoopedIndependentVariables      = new List<IndependentVariable>();
        public readonly List<IndependentVariable> ProbabilityIndependentVariables = new List<IndependentVariable>();
        public readonly List<DependentVariable>   DependentVariables              = new List<DependentVariable>();
        public readonly List<ParticipantVariable> ParticipantVariables = new List<ParticipantVariable>();

        //Sort Independent variables into mixing categories so they go in order
        public SortedVariableContainer(List<Variable> allData, bool block) {
            int numberOfBlockIvs = 0;
            int numberOfNonBlockIvs = 0;
            foreach (Variable variable in allData) {
                switch (variable.TypeOfVariable) {
                    case VariableType.Independent: {
                        IndependentVariable independentVariable = (IndependentVariable) variable;

                        if (independentVariable.Block) {
                            numberOfBlockIvs++;
                        }
                        else {
                            numberOfNonBlockIvs++;
                        }

                        if (block && independentVariable.Block || !block && !independentVariable.Block) {

                            switch (independentVariable.MixingTypeOfVariable) {
                                case VariableMixingType.Balanced:
                                    BalancedIndependentVariables.Add(independentVariable);
                                    break;
                                case VariableMixingType.Looped:
                                    LoopedIndependentVariables.Add(independentVariable);
                                    break;

                                case VariableMixingType.EvenProbability:
                                case VariableMixingType.CustomProbability:
                                    ProbabilityIndependentVariables.Add(independentVariable);
                                    break;

                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }

                        break;
                    }
                    case VariableType.Dependent: {
                        DependentVariable dependentVariable = (DependentVariable) variable;
                        DependentVariables.Add(dependentVariable);
                        break;
                    }
                    case VariableType.Participant: {
                        ParticipantVariable participantVariable = (ParticipantVariable) variable;
                        ParticipantVariables.Add(participantVariable);
                        break;
                    }
                    case VariableType.ChooseType:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            bool thereAreBlockIvsButNoNormalIvs = numberOfBlockIvs > 0 && numberOfNonBlockIvs == 0;
            if (!block && thereAreBlockIvsButNoNormalIvs) {

                throw new ExperimentDesign.InvalidExperimentDesignException($"You defined {numberOfBlockIvs} block variable(s), " +
                                                                            $"when there are {numberOfNonBlockIvs} unblocked independent variables." +
                                                                            "You can safely unblock the variable " +
                                                                            "to make it a normal variable");
            }
        }

       
    }
}
