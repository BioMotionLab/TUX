using System;
using System.Collections.Generic;
using BML_ExperimentToolkit.Scripts.VariableSystem;
using BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes;

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
                }
                else if (variable.TypeOfVariable == VariableType.Dependent) {
                    DependentVariable dependentVariable = (DependentVariable) variable;
                    DependentVariables.Add(dependentVariable);
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
