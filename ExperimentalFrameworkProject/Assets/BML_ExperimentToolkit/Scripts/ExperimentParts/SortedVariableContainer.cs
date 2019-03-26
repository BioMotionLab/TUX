using System;
using System.Collections.Generic;
using BML_ExperimentToolkit.Scripts.VariableSystem;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {


    public class SortedVariableContainer
    {
        public List<IndependentVariable> BalancedIndependentVariables    = new List<IndependentVariable>();
        public List<IndependentVariable> LoopedIndependentVariables      = new List<IndependentVariable>();
        public List<IndependentVariable> ProbabilityIndependentVariables = new List<IndependentVariable>();
        public List<DependentVariable>   DependentVariables              = new List<DependentVariable>();

        //Sort Independent variables into mixing categories so they go in order
        public SortedVariableContainer(List<Variable> allData, bool block) {
            int numberOfBlockIvs = 0;
            int numberOfNonBlockIvs = 0;
            foreach (Variable variable in allData) {
                if (variable.TypeOfVariable == VariableType.Independent) {
                    IndependentVariable ivVariable = (IndependentVariable) variable;
                    if (ivVariable.Block) {
                        numberOfBlockIvs++;
                    }
                    else {
                        numberOfNonBlockIvs++;
                    }

                    if (block && ivVariable.Block || !block && !ivVariable.Block) {

                        switch (ivVariable.MixingTypeOfVariable) {
                            case VariableMixingType.Balanced:
                                BalancedIndependentVariables.Add(ivVariable);
                                break;
                            case VariableMixingType.Looped:
                                LoopedIndependentVariables.Add(ivVariable);
                                break;

                            case VariableMixingType.EvenProbability:
                            case VariableMixingType.CustomProbability:
                                ProbabilityIndependentVariables.Add(ivVariable);
                                break;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
                else if (variable.TypeOfVariable == VariableType.Dependent) {
                    DependentVariable dVDatum = (DependentVariable) variable;
                    DependentVariables.Add(dVDatum);
                }
            }

            bool thereAreBlockIvsButNoNormalIvs = numberOfBlockIvs > 0 && numberOfNonBlockIvs == 0;
            if (!block && thereAreBlockIvsButNoNormalIvs) {

                throw new ExperimentDesign.InvalidExperimentDesignException($"You defined {numberOfBlockIvs} block variable(s), " +
                                                                            $"when there are {numberOfNonBlockIvs} unblocked independent variables." +
                                                                            $"You can safely unblock the variable " +
                                                                            $"to make it a normal variable");
            }
        }
    }
}
